using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime.SharedInterfaces;
using Amazon.SimpleNotificationService;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Network.Aws
{
    public class AwsSnsClient : IAwsSnsClient
    {
        private readonly AmazonSimpleNotificationServiceClient _snsClient;
        private readonly ILogger _logger;

        public AwsSnsClient(ILogger<AwsSnsClient> logger)
        {
            _snsClient = new AmazonSimpleNotificationServiceClient(
                    new AmazonSimpleNotificationServiceConfig
                    {
                        RegionEndpoint = Amazon.RegionEndpoint.EUWest1,
                        ProxyCredentials = CredentialCache.DefaultCredentials
                    });

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> CreateSnsTopic(string topicName, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(topicName))
            {
                throw new ArgumentNullException(nameof(topicName));
            }

            _logger?.LogInformation($"create sns topic initiating for topic {topicName}");
            var topic = await _snsClient.CreateTopicAsync(topicName, ct);
            _logger?.LogInformation($"create sns topic completed for {topicName}");

            return topic.TopicArn;
        }

        public async Task SubscribeQueueToSnsTopic(string topicArn, ICoreAmazonSQS sqsClient, string sqsUrl)
        {
            if (string.IsNullOrWhiteSpace(topicArn))
            {
                throw new ArgumentNullException(nameof(topicArn));
            }

            if (sqsClient == null)
            {
                throw new ArgumentNullException(nameof(sqsClient));
            }

            if (string.IsNullOrWhiteSpace(sqsUrl))
            {
                throw new ArgumentNullException(nameof(sqsUrl));
            }

            _logger?.LogInformation($"subscribe queue to SNS topic {topicArn} with queue {sqsUrl}");
            await _snsClient.SubscribeQueueAsync(topicArn, sqsClient, sqsUrl);
            _logger?.LogInformation($"subscribe queue to SNS topic {topicArn} with queue {sqsUrl} completed");
        }
    }
}
