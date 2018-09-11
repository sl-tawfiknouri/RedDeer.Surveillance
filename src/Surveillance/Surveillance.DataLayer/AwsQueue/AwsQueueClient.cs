using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.AwsQueue.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.AwsQueue
{
    public class AwsQueueClient : IAwsQueueClient
    {
        private readonly AmazonSQSClient _sqsClient;
        private readonly ILogger<AwsQueueClient> _logger;

        public AwsQueueClient(
            IDataLayerConfiguration dataLayerConfiguration,
            ILogger<AwsQueueClient> logger)
        {
            _sqsClient = dataLayerConfiguration.IsEc2Instance ?
                 new AmazonSQSClient(
                     new AmazonSQSConfig
                     {
                         RegionEndpoint = Amazon.RegionEndpoint.EUWest1,
                         ProxyCredentials = CredentialCache.DefaultCredentials
                     }) :
                 new AmazonSQSClient(
                     dataLayerConfiguration.AwsAccessKey,
                     dataLayerConfiguration.AwsSecretKey,
                     new AmazonSQSConfig
                     {
                         RegionEndpoint = Amazon.RegionEndpoint.EUWest1,
                         ProxyCredentials = CredentialCache.DefaultCredentials
                     });

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetQueueUrlAsync(string name, CancellationToken cancellationToken)
        {
            var getQueueUrlRequest = new GetQueueUrlRequest
            {
                QueueName = name
            };
            var getQueueUrlResponse = await _sqsClient.GetQueueUrlAsync(getQueueUrlRequest, cancellationToken);
            _logger.LogInformation($"Got Queue Url (Name: {name}, Url: {getQueueUrlResponse.QueueUrl}).");

            return getQueueUrlResponse.QueueUrl;
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

            _logger.LogInformation($"Sent Message To Queue (Name: {name}, Size: {message.Length}, HttpCode: {sendMessageResponse.HttpStatusCode.ToString()}).");
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
                        _logger.LogInformation($"Received Message (Queue: {name}, MessageId: {message.MessageId}, Size: {message.Body.Length}).");

                        await action(message.MessageId, message.Body);

                        var deleteMessageRequest = new DeleteMessageRequest
                        {
                            QueueUrl = queueUrl,
                            ReceiptHandle = message.ReceiptHandle
                        };
                        await _sqsClient.DeleteMessageAsync(deleteMessageRequest);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }
    }
}
