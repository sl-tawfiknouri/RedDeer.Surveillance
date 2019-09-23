namespace DataImport.MessageBusIO
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts.Email;

    using DataImport.MessageBusIO.Interfaces;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    public class EmailNotificationMessageSender : IEmailNotificationMessageSender
    {
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IAwsQueueClient _awsQueueClient;

        private readonly ILogger<EmailNotificationMessageSender> _logger;

        private readonly IMessageBusSerialiser _serialiser;

        public EmailNotificationMessageSender(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IMessageBusSerialiser serialiser,
            ILogger<EmailNotificationMessageSender> logger)
        {
            this._awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this._awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this._serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Send(SendSimpleEmailToRecipient message)
        {
            if (message == null)
            {
                this._logger.LogWarning("Asked to send a null message. Will not be sending anything.");
                return;
            }

            var messageBusCts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
            var serialisedMessage = this._serialiser.Serialise(message);

            try
            {
                this._logger.LogInformation($"Dispatching to {this._awsConfiguration.EmailServiceSendEmailQueueName}");
                await this._awsQueueClient.SendToQueue(
                    this._awsConfiguration.EmailServiceSendEmailQueueName,
                    serialisedMessage,
                    messageBusCts.Token);
                this._logger.LogInformation(
                    $"Finished dispatching to {this._awsConfiguration.EmailServiceSendEmailQueueName}");
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    $"Exception sending message '{serialisedMessage}' to bus on queue {this._awsConfiguration.EmailServiceSendEmailQueueName}. Error was {e.Message}");
            }
        }
    }
}