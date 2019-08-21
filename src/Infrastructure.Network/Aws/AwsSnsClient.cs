namespace Infrastructure.Network.Aws
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Amazon;
    using Amazon.Runtime.SharedInterfaces;
    using Amazon.SimpleNotificationService;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    public class AwsSnsClient : IAwsSnsClient
    {
        private readonly ILogger _logger;

        private readonly AmazonSimpleNotificationServiceClient _snsClient;

        public AwsSnsClient(ILogger<AwsSnsClient> logger)
        {
            this._snsClient = new AmazonSimpleNotificationServiceClient(
                new AmazonSimpleNotificationServiceConfig
                    {
                        RegionEndpoint = RegionEndpoint.EUWest1, ProxyCredentials = CredentialCache.DefaultCredentials
                    });

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> CreateSnsTopic(string topicName, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(topicName)) throw new ArgumentNullException(nameof(topicName));

            this._logger?.LogInformation($"create sns topic initiating for topic {topicName}");
            var topic = await this._snsClient.CreateTopicAsync(topicName, ct);
            this._logger?.LogInformation($"create sns topic completed for {topicName}");

            return topic.TopicArn;
        }

        public async Task SubscribeQueueToSnsTopic(string topicArn, ICoreAmazonSQS sqsClient, string sqsUrl)
        {
            if (string.IsNullOrWhiteSpace(topicArn)) throw new ArgumentNullException(nameof(topicArn));

            if (sqsClient == null) throw new ArgumentNullException(nameof(sqsClient));

            if (string.IsNullOrWhiteSpace(sqsUrl)) throw new ArgumentNullException(nameof(sqsUrl));

            this._logger?.LogInformation($"subscribe queue to SNS topic {topicArn} with queue {sqsUrl}");
            await this._snsClient.SubscribeQueueAsync(topicArn, sqsClient, sqsUrl);
            this._logger?.LogInformation($"subscribe queue to SNS topic {topicArn} with queue {sqsUrl} completed");
        }
    }
}