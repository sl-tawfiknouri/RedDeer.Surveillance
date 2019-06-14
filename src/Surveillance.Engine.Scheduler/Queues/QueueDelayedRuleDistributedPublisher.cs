using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Surveillance.Scheduling;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Scheduler.Queues.Interfaces;

namespace Surveillance.Engine.Scheduler.Queues
{
    public class QueueDelayedRuleDistributedPublisher : IQueueDelayedRuleDistributedPublisher
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ILogger<QueueDelayedRuleDistributedPublisher> _logger;

        public QueueDelayedRuleDistributedPublisher(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ILogger<QueueDelayedRuleDistributedPublisher> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Publish(AdHocScheduleRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.JsonSqsMessage))
            {
                _logger?.LogInformation($"asked to publish a null or empty request message");
                return;
            }

            var cancellationToken = new CancellationTokenSource();

            try
            {
                _logger.LogInformation($"publishing message to {_awsConfiguration.ScheduleRuleDistributedWorkQueueName}");
                await _awsQueueClient.SendToQueue(_awsConfiguration.ScheduleRuleDistributedWorkQueueName, request.JsonSqsMessage, cancellationToken.Token);
                _logger.LogInformation($"published message to {_awsConfiguration.ScheduleRuleDistributedWorkQueueName}");
            }
            catch (Exception e)
            {
                _logger.LogError($"exception when publishing '{request.JsonSqsMessage}' to queue {_awsConfiguration.ScheduleRuleDistributedWorkQueueName} {e.Message} {e.InnerException?.Message}", e);
            }
        }
    }
}
