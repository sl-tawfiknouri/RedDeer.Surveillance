using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts.SurveillanceService.Interfaces;
using DataImport.MessageBusIO.Interfaces;
using DomainV2.Contracts;
using Microsoft.Extensions.Logging;
using Utilities.Aws_IO.Interfaces;

namespace DataImport.MessageBusIO
{
    public class UploadCoordinatorMessageSender : IUploadCoordinatorMessageSender
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ILogger<ScheduleRuleMessageSender> _logger;
        private readonly IMessageBusSerialiser _serialiser;

        public UploadCoordinatorMessageSender(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IMessageBusSerialiser serialiser,
            ILogger<ScheduleRuleMessageSender> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Send(UploadCoordinatorMessage message)
        {
            if (message == null)
            {
                _logger.LogWarning($"UploadCoordinatorMessageSender was asked to send a null message. Will not be sending anything.");
                return;
            }

            var messageBusCts = new CancellationTokenSource();
            var serialisedMessage = _serialiser.Serialise(message);

            try
            {
                _logger.LogInformation($"UploadCoordinatorMessageSender dispatching to {_awsConfiguration.UploadCoordinatorQueueName}");
                await _awsQueueClient.SendToQueue(_awsConfiguration.UploadCoordinatorQueueName, serialisedMessage, messageBusCts.Token);
                _logger.LogInformation($"UploadCoordinatorMessageSender finished dispatching to {_awsConfiguration.UploadCoordinatorQueueName}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception in UploadCoordinatorMessageSender sending message '{message}' to bus on queue {_awsConfiguration.UploadCoordinatorQueueName}. Error was {e.Message}");
            }
        }
    }
}
