using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Email;
using DataImport.MessageBusIO.Interfaces;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Interfaces;

namespace DataImport.MessageBusIO
{
    public class EmailNotificationMessageSender : IEmailNotificationMessageSender
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ILogger<EmailNotificationMessageSender> _logger;
        private readonly IMessageBusSerialiser _serialiser;

        public EmailNotificationMessageSender(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IMessageBusSerialiser serialiser,
            ILogger<EmailNotificationMessageSender> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Send(SendEmailToRecipient message)
        {
            if (message == null)
            {
                _logger.LogWarning($"Asked to send a null message. Will not be sending anything.");
                return;
            }

            var messageBusCts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
            var serialisedMessage = _serialiser.Serialise(message);

            try
            {
                _logger.LogInformation($"Dispatching to {_awsConfiguration.EmailServiceSendEmailQueueName}");
                await _awsQueueClient.SendToQueue(_awsConfiguration.EmailServiceSendEmailQueueName, serialisedMessage, messageBusCts.Token);
                _logger.LogInformation($"Finished dispatching to {_awsConfiguration.EmailServiceSendEmailQueueName}");
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception sending message '{serialisedMessage}' to bus on queue {_awsConfiguration.EmailServiceSendEmailQueueName}. Error was {e.Message}");
            }
        }
    }
}
