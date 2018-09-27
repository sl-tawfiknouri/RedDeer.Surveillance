using Domain.Scheduling;
using Domain.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.MessageBus_IO.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.MessageBus_IO
{
    public class ScheduleRuleMessageSender : IScheduleRuleMessageSender
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ILogger<ScheduleRuleMessageSender> _logger;
        private readonly IScheduledExecutionMessageBusSerialiser _serialiser;

        public ScheduleRuleMessageSender(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser serialiser,
            ILogger<ScheduleRuleMessageSender> logger)
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
                return;
            }

            var messageBusCts = new CancellationTokenSource();
            var serialisedMessage = _serialiser.SerialiseScheduledExecution(message);

            try
            {
                await _awsQueueClient.SendToQueue(_awsConfiguration.ScheduledRuleQueueName, serialisedMessage, messageBusCts.Token);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception in Schedule Rule Message Sender sending message '{message}' to bus on queue {_awsConfiguration.ScheduledRuleQueueName}. Error was {e.Message}");
            }
        }
    }
}
