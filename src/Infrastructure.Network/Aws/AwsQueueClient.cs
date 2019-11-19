namespace Infrastructure.Network.Aws
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.Runtime.SharedInterfaces;
    using Amazon.SQS;
    using Amazon.SQS.Model;
    using Amazon.Util;

    using Infrastructure.Network.Aws.Interfaces;
    using Infrastructure.Network.Extensions;

    using Microsoft.Extensions.Logging;

    public class AwsQueueClient : IAwsQueueClient
    {
        private readonly ILogger _logger; // can be null

        private readonly AmazonSQSClient _sqsClient;

        public AwsQueueClient(ILogger<AwsQueueClient> logger)
        {
            this._logger = logger;
            this._sqsClient = this.BuildSqsClient();
        }

        public ICoreAmazonSQS SqsClient => this._sqsClient;

        public async Task<string> CreateQueue(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName)) return string.Empty;

            var cts = new CancellationTokenSource(1000 * 60);
            var queueCreation = await this._sqsClient.CreateQueueAsync(queueName, cts.Token);

            return queueCreation.QueueUrl;
        }

        public async Task<bool> DeleteQueue(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName)) return true;

            var cts = new CancellationTokenSource(1000 * 60);
            var queueDeletion = await this._sqsClient.DeleteQueueAsync(queueName, cts.Token);

            return queueDeletion.IsSuccessStatusCode();
        }

        public async Task<bool> ExistsQueue(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName)) return false;

            var queues = await this._sqsClient.ListQueuesAsync(new ListQueuesRequest(queueName));

            return queues?.QueueUrls?.Any() ?? false;
        }

        public async Task<string> GetQueueUrlAsync(string name, CancellationToken cancellationToken, bool retry = true)
        {
            try
            {
                var getQueueUrlRequest = new GetQueueUrlRequest { QueueName = name };

                var getQueueUrlResponse = await this._sqsClient.GetQueueUrlAsync(getQueueUrlRequest, cancellationToken);
                this._logger?.LogInformation($"Got Queue Url (Name: {name}, Url: {getQueueUrlResponse?.QueueUrl}).");

                return getQueueUrlResponse?.QueueUrl ?? string.Empty;
            }
            catch (Exception ex)
            {
                this._logger?.LogError(ex, $"Exception while getting queue url for {name}");


                if (!retry) throw;

                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
                var result = await this._sqsClient.CreateQueueAsync(name, cts.Token);

                if (result.HttpStatusCode != HttpStatusCode.OK)
                {
                    this._logger?.LogError(
                        $"AwsQueueClient tried to create new queue for {name} but failed with {result?.HttpStatusCode}");
                    throw;
                }

                return await this.GetQueueUrlAsync(name, cancellationToken, false);
            }
        }

        public async Task PurgeQueue(string queueName, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(queueName)) return;

            var queueUrl = await this.GetQueueUrlAsync(queueName, token);

            if (token.IsCancellationRequested) return;

            var purgeRequest = new PurgeQueueRequest { QueueUrl = queueUrl };
            await this._sqsClient.PurgeQueueAsync(purgeRequest, token);
        }

        public async Task<int> QueueMessageCount(string name, CancellationToken cancellationToken)
        {
            var queueUrl = await this.GetQueueUrlAsync(name, cancellationToken);
            while (!cancellationToken.IsCancellationRequested)
                try
                {
                    var response = await this._sqsClient.GetQueueAttributesAsync(
                                       new GetQueueAttributesRequest
                                           {
                                               QueueUrl = queueUrl,
                                               AttributeNames = new List<string> { "ApproximateNumberOfMessages" }
                                           },
                                       cancellationToken);

                    return response.ApproximateNumberOfMessages;
                }
                catch (Exception ex)
                {
                    this._logger?.LogError("AwsQueueClient: " + ex?.Message + " " + ex?.InnerException?.Message);
                }

            return 0;
        }

        public async Task SendToQueue(string name, string message, CancellationToken cancellationToken)
        {
            var queueUrl = await this.GetQueueUrlAsync(name, cancellationToken);

            var sendMessageRequest = new SendMessageRequest { QueueUrl = queueUrl, MessageBody = message };

            var sendMessageResponse = await this._sqsClient.SendMessageAsync(sendMessageRequest, cancellationToken);

            this._logger?.LogInformation(
                $"Sent Message To Queue (Name: {name}, Size: {message?.Length}, HttpCode: {sendMessageResponse?.HttpStatusCode.ToString()}).");
        }

        public async Task SubscribeToQueueAsync(
            string name,
            Func<string, string, Task> action,
            CancellationToken cancellationToken,
            AwsResusableCancellationToken reusableToken)
        {
            _logger?.LogInformation($"Subscribing to queue {name}");
            var queueUrl = await this.GetQueueUrlAsync(name, cancellationToken);
            _logger?.LogInformation($"Subscribing to queue {name} found queue url {queueUrl}");
            while (!cancellationToken.IsCancellationRequested)
                try
                {
                    var receiveMessageRequest = new ReceiveMessageRequest
                    {
                        QueueUrl = queueUrl,
                        MaxNumberOfMessages = 1,
                        WaitTimeSeconds = 20 // Long Polling
                    };

                    var result = await this._sqsClient.ReceiveMessageAsync(receiveMessageRequest, cancellationToken);
                    foreach (var message in result.Messages)
                    {
                        if (message == null) continue;

                        var messageId = message?.MessageId ?? string.Empty;
                        this._logger?.LogInformation($"Received Message (Queue: {name}, MessageId: {messageId})");

                        await action(message.MessageId, message.Body);

                        if (reusableToken?.Cancel == true)
                        {
                            this._logger?.LogInformation(
                                $"Cancelling Processing for Message (Queue: {name}, MessageId: {messageId})");
                            reusableToken.Cancel = false;
                            continue;
                        }

                        var deleteMessageRequest = new DeleteMessageRequest
                                                       {
                                                           QueueUrl = queueUrl, ReceiptHandle = message.ReceiptHandle
                                                       };

                        await this._sqsClient.DeleteMessageAsync(deleteMessageRequest, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    this._logger?.LogError(ex, $"Failed while processing queue {name} message");
                }
        }

        public async Task<string> UrlQueue(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName)) return string.Empty;

            var queues = await this._sqsClient.GetQueueUrlAsync(queueName);

            return queues?.QueueUrl ?? string.Empty;
        }

        /// <summary>
        ///     We have temporary credentials for AWS on EC2 instances
        ///     Using explicit config prevents credential recycling
        ///     Therefore only use it where we need it - on developer instances
        /// </summary>
        private AmazonSQSClient BuildSqsClient()
        {
            AmazonSQSClient client;

            if (EC2InstanceMetadata.InstanceId != null)
            {
                this._logger?.LogInformation(
                    $"{nameof(AwsQueueClient)} building sqs client. Detected local instance is on EC2 use aws environmental credentials only.");
                client = new AmazonSQSClient();
            }
            else
            {
                this._logger?.LogInformation(
                    $"{nameof(AwsQueueClient)} building sqs client. Detected local instance is not on EC2 using hybrid of proxy/euwest1 and local machine config");

                client = new AmazonSQSClient(
                    new AmazonSQSConfig
                        {
                            RegionEndpoint = RegionEndpoint.EUWest1,
                            ProxyCredentials = CredentialCache.DefaultCredentials
                        });
            }

            return client;
        }
    }
}