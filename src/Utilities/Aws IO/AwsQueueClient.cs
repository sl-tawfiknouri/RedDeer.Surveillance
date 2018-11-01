using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Utilities.Aws_IO.Interfaces;

namespace Utilities.Aws_IO
{
    public class AwsQueueClient : IAwsQueueClient
    {
        private readonly AmazonSQSClient _sqsClient;
        private readonly ILogger<AwsQueueClient> _logger; // can be null

        public AwsQueueClient(
            IAwsConfiguration awsConfiguration,
            ILogger<AwsQueueClient> logger)
        {
            if (awsConfiguration == null)
            {
                throw new ArgumentNullException(nameof(awsConfiguration));
            }

            _sqsClient = new AmazonSQSClient(
                new AmazonSQSConfig
                {
                    RegionEndpoint = Amazon.RegionEndpoint.EUWest1,
                    ProxyCredentials = CredentialCache.DefaultCredentials
                });

            _logger = logger;
        }

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
                _logger?.LogInformation($"Got Queue Url (Name: {name}, Url: {getQueueUrlResponse.QueueUrl}).");

                return getQueueUrlResponse.QueueUrl;
            }
            catch (Exception e)
            {
                if (!retry)
                {
                    throw;
                }

                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
                var result = await _sqsClient.CreateQueueAsync(name, cts.Token);

                if (result.HttpStatusCode != HttpStatusCode.OK)
                {
                    _logger?.LogError($"AwsQueueClient tried to create new queue for {name} but failed with {result.HttpStatusCode}");
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
            _logger?.LogInformation($"Sent Message To Queue (Name: {name}, Size: {message.Length}, HttpCode: {sendMessageResponse.HttpStatusCode.ToString()}).");
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
                    _logger?.LogError(ex.ToString());
                }
            }

            return 0;
        }

        public async Task SubscribeToQueueAsync(string name, Func<string, string, Task> action, CancellationToken cancellationToken)
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
                        _logger?.LogInformation($"Received Message (Queue: {name}, MessageId: {message.MessageId}, Size: {message.Body.Length}).");

                        await action(message.MessageId, message.Body);

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
    }
}
