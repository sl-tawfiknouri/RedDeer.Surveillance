using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime.SharedInterfaces;
using Amazon.SQS;
using Amazon.SQS.Model;
using Infrastructure.Network.Aws.Interfaces;
using Infrastructure.Network.Extensions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Network.Aws
{
    public class AwsQueueClient : IAwsQueueClient
    {
        private readonly AmazonSQSClient _sqsClient;
        private readonly ILogger _logger; // can be null

        public AwsQueueClient(ILogger<AwsQueueClient> logger)
        {
            _logger = logger;
            _sqsClient = BuildSqsClient();
        }

        /// <summary>
        /// We have temporary credentials for AWS on EC2 instances
        /// Using explicit config prevents credential recycling
        /// Therefore only use it where we need it - on developer instances
        /// </summary>
        private AmazonSQSClient BuildSqsClient()
        {
            AmazonSQSClient client;

            if (Amazon.Util.EC2InstanceMetadata.InstanceId != null)
            {
                _logger?.LogInformation($"{nameof(AwsQueueClient)} building sqs client. Detected local instance is on EC2 use aws environmental credentials only.");
                client = new AmazonSQSClient();
            }
            else
            {
                _logger?.LogInformation($"{nameof(AwsQueueClient)} building sqs client. Detected local instance is not on EC2 using hybrid of proxy/euwest1 and local machine config");

                client = new AmazonSQSClient(
                    new AmazonSQSConfig
                    {
                        RegionEndpoint = Amazon.RegionEndpoint.EUWest1,
                        ProxyCredentials = CredentialCache.DefaultCredentials
                    });
            }

            return client;
        }

        public ICoreAmazonSQS SqsClient => _sqsClient;

        public async Task PurgeQueue(string queueName, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                return;
            }

            var queueUrl = await GetQueueUrlAsync(queueName, token);

            if (token.IsCancellationRequested)
            {
                return;
            }

            var purgeRequest = new PurgeQueueRequest {QueueUrl = queueUrl};
            await _sqsClient.PurgeQueueAsync(purgeRequest, token);
        }

        public async Task<string> GetQueueUrlAsync(string name, CancellationToken cancellationToken, bool retry = true)
        {
            try
            {
                var getQueueUrlRequest = new GetQueueUrlRequest
                {
                    QueueName = name
                };

                var getQueueUrlResponse = await _sqsClient.GetQueueUrlAsync(getQueueUrlRequest, cancellationToken);
                _logger?.LogInformation($"Got Queue Url (Name: {name}, Url: {getQueueUrlResponse?.QueueUrl}).");

                return getQueueUrlResponse.QueueUrl;
            }
            catch (Exception)
            {
                if (!retry)
                {
                    throw;
                }

                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
                var result = await _sqsClient.CreateQueueAsync(name, cts.Token);

                if (result.HttpStatusCode != HttpStatusCode.OK)
                {
                    _logger?.LogError($"AwsQueueClient tried to create new queue for {name} but failed with {result?.HttpStatusCode}");
                    throw;
                }

                return await GetQueueUrlAsync(name, cancellationToken, false);
            }
        }

        public async Task SendToQueue(string name, string message, CancellationToken cancellationToken)
        {
            var queueUrl = await GetQueueUrlAsync(name, cancellationToken);

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = message
            };

            var sendMessageResponse = await _sqsClient.SendMessageAsync(sendMessageRequest, cancellationToken);

            _logger?.LogInformation($"Sent Message To Queue (Name: {name}, Size: {message?.Length}, HttpCode: {sendMessageResponse?.HttpStatusCode.ToString()}).");
        }

        public async Task<int> QueueMessageCount(string name, CancellationToken cancellationToken)
        {
            var queueUrl = await GetQueueUrlAsync(name, cancellationToken);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var response =
                        await _sqsClient.GetQueueAttributesAsync(
                            new GetQueueAttributesRequest
                            {
                                QueueUrl = queueUrl,
                                AttributeNames = new List<string> { "ApproximateNumberOfMessages" }
                            }, cancellationToken);

                    return response.ApproximateNumberOfMessages;
                }
                catch (Exception ex)
                {
                    _logger?.LogError("AwsQueueClient: " + ex?.Message + " " + ex?.InnerException?.Message);
                }
            }
            return 0;
        }

        public async Task SubscribeToQueueAsync(
            string name,
            Func<string, string, Task> action,
            CancellationToken cancellationToken,
            AwsResusableCancellationToken reusableToken)
        {
            var queueUrl = await GetQueueUrlAsync(name, cancellationToken);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var receiveMessageRequest = new ReceiveMessageRequest
                    {
                        QueueUrl = queueUrl,
                        MaxNumberOfMessages = 1,
                        WaitTimeSeconds = 20 // Long Polling
                    };

                    var result = await _sqsClient.ReceiveMessageAsync(receiveMessageRequest, cancellationToken);
                    foreach (var message in result.Messages)
                    {
                        var messageId = message?.MessageId ?? string.Empty;
                        _logger?.LogInformation($"Received Message (Queue: {name}, MessageId: {messageId})");

                        await action(message.MessageId, message.Body);

                        if (reusableToken?.Cancel == true)
                        {
                            _logger?.LogInformation($"Cancelling Processing for Message (Queue: {name}, MessageId: {messageId})");
                            reusableToken.Cancel = false;
                            continue;
                        }

                        var deleteMessageRequest = new DeleteMessageRequest
                        {
                            QueueUrl = queueUrl,
                            ReceiptHandle = message.ReceiptHandle
                        };

                        await _sqsClient.DeleteMessageAsync(deleteMessageRequest, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex.ToString());
                }
            }
        }

        public async Task<bool> DeleteQueue(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                return true;
            }

            var cts = new CancellationTokenSource(1000 * 60);
            var queueDeletion = await _sqsClient.DeleteQueueAsync(queueName, cts.Token);

            return queueDeletion.IsSuccessStatusCode();
        }

        public async Task<string> CreateQueue(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                return string.Empty;
            }

            var cts = new CancellationTokenSource(1000 * 60);
            var queueCreation = await _sqsClient.CreateQueueAsync(queueName, cts.Token);

            return queueCreation.QueueUrl;
        }

        public async Task<bool> ExistsQueue(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                return false;
            }

            var queues = await _sqsClient.ListQueuesAsync(new ListQueuesRequest(queueName));

            return queues?.QueueUrls?.Any() ?? false;
        }

        public async Task<string> UrlQueue(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                return string.Empty;
            }

            var queues = await _sqsClient.GetQueueUrlAsync(queueName);

            return queues?.QueueUrl ?? string.Empty;
        }
    }
}
