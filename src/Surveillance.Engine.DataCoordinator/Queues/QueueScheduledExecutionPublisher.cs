using System;
using System.Threading;
using System.Threading.Tasks;
using DomainV2.Scheduling;
using DomainV2.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.DataCoordinator.Queues.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Queues
{
    public class QueueScheduleRulePublisher : IQueueScheduleRulePublisher
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ILogger<QueueScheduleRulePublisher> _logger;
        private readonly IScheduledExecutionMessageBusSerialiser _serialiser;

        public QueueScheduleRulePublisher(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser serialiser,
            ILogger<QueueScheduleRulePublisher> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Send(ScheduledExecution message)
        {
            if (message == null)
            {
                _logger.LogWarning($"QueueScheduleRulePublisher was asked to send a null message. Will not be sending anything.");
                return;
            }

            var messageBusCts = new CancellationTokenSource();
            var serialisedMessage = _serialiser.SerialiseScheduledExecution(message);

            try
            {
                _logger.LogInformation($"QueueScheduleRulePublisher dispatching to {_awsConfiguration.ScheduledRuleQueueName}");
                await _awsQueueClient.SendToQueue(_awsConfiguration.ScheduledRuleQueueName, serialisedMessage, messageBusCts.Token);
                _logger.LogInformation($"QueueScheduleRulePublisher finished dispatching to {_awsConfiguration.ScheduledRuleQueueName}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception in QueueScheduleRulePublisher sending message '{message}' to bus on queue {_awsConfiguration.ScheduledRuleQueueName}. Error was {e.Message}");
            }
        }
    }
}
