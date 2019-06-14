using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Surveillance.Scheduling;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Scheduler.Queues.Interfaces;

namespace Surveillance.Engine.Scheduler.Queues
{
    public class QueueScheduledRulePublisher : IQueueScheduledRulePublisher
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ILogger<IQueueScheduledRulePublisher> _logger;

        public QueueScheduledRulePublisher(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ILogger<IQueueScheduledRulePublisher> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Publish(AdHocScheduleRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.JsonSqsMessage))
            {
                _logger.LogInformation($"asked to publish a rescheduled request that was null or empty");
                return;
            }

            var messageBusCts = new CancellationTokenSource();

            try
            {
                _logger.LogInformation($"dispatching to {_awsConfiguration.ScheduledRuleQueueName}");
                await _awsQueueClient.SendToQueue(_awsConfiguration.ScheduledRuleQueueName, request.JsonSqsMessage, messageBusCts.Token);
                _logger.LogInformation($"finished dispatching to {_awsConfiguration.ScheduledRuleQueueName}");
            }
            catch (Exception e)
            {
                _logger.LogError($"exception sending message '{request.JsonSqsMessage}' to queue '{_awsConfiguration.ScheduledRuleQueueName}'. Error was {e.Message} {e.InnerException?.Message}", e);
            }
        }
    }
}
