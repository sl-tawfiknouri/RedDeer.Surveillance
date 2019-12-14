namespace DataImport.MessageBusIO
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using DataImport.MessageBusIO.Interfaces;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Interfaces;

    using SharedKernel.Contracts.Queues;

    public class UploadCoordinatorMessageSender : IUploadCoordinatorMessageSender
    {
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IAwsQueueClient _awsQueueClient;

        private readonly ILogger<UploadCoordinatorMessageSender> _logger;

        private readonly IMessageBusSerialiser _serialiser;

        public UploadCoordinatorMessageSender(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IMessageBusSerialiser serialiser,
            ILogger<UploadCoordinatorMessageSender> logger)
        {
            this._awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this._awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this._serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Send(AutoScheduleMessage message)
        {
            if (message == null)
            {
                this._logger.LogWarning(
                    "UploadCoordinatorMessageSender was asked to send a null message. Will not be sending anything.");
                return;
            }

            var messageBusCts = new CancellationTokenSource();
            var serialisedMessage = this._serialiser.Serialise(message);

            try
            {
                this._logger.LogInformation(
                    $"UploadCoordinatorMessageSender dispatching to {this._awsConfiguration.UploadCoordinatorQueueName}");
                await this._awsQueueClient.SendToQueue(
                    this._awsConfiguration.UploadCoordinatorQueueName,
                    serialisedMessage,
                    messageBusCts.Token);
                this._logger.LogInformation(
                    $"UploadCoordinatorMessageSender finished dispatching to {this._awsConfiguration.UploadCoordinatorQueueName}");
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"Exception in UploadCoordinatorMessageSender sending message '{message}' to bus on queue {this._awsConfiguration.UploadCoordinatorQueueName}.");
            }
        }
    }
}