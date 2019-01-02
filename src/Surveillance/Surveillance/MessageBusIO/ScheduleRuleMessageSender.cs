using System;
using System.Threading;
using System.Threading.Tasks;
using DomainV2.Scheduling;
using DomainV2.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.MessageBusIO.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.MessageBusIO
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
                _logger.LogError($"ScheduleRuleMessageSender received a null message to send. Returning.");
                return;
            }

            try
            {
                var messageBusCts = new CancellationTokenSource();
                var serialisedMessage = _serialiser.SerialiseScheduledExecution(message);

                _logger.LogInformation($"ScheduleRuleMessageSender sending a schedule rule request for {message.TimeSeriesInitiation} to {message.TimeSeriesTermination}");

                await _awsQueueClient.SendToQueue(_awsConfiguration.ScheduledRuleQueueName, serialisedMessage, messageBusCts.Token);

                _logger.LogInformation($"ScheduleRuleMessageSender sent a schedule rule request for {message.TimeSeriesInitiation} to {message.TimeSeriesTermination}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception in Schedule Rule Message Sender sending message '{message}' to bus on queue {_awsConfiguration.ScheduledRuleQueueName}. Error was {e.Message}");
            }
        }
    }
}
