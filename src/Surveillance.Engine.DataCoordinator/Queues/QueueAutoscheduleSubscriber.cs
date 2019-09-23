namespace Surveillance.Engine.DataCoordinator.Queues
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Infrastructure.Network.Aws;
    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Interfaces;

    using SharedKernel.Contracts.Queues;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;
    using Surveillance.Engine.DataCoordinator.Queues.Interfaces;

    /// <summary>
    ///     Enable push based auto scheduling
    /// </summary>
    public class QueueAutoscheduleSubscriber : IQueueSubscriber
    {
        private readonly IAutoSchedule _autoSchedule;

        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IAwsQueueClient _awsQueueClient;

        private readonly IDataVerifier _dataVerifier;

        private readonly ILogger<QueueAutoscheduleSubscriber> _logger;

        private readonly IMessageBusSerialiser _serialiser;

        private readonly ISystemProcessContext _systemProcessContext;

        private CancellationTokenSource _messageBusCts;

        private AwsResusableCancellationToken _token;

        public QueueAutoscheduleSubscriber(
            IDataVerifier dataVerifier,
            IAutoSchedule autoSchedule,
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IMessageBusSerialiser serialiser,
            ISystemProcessContext systemProcessContext,
            ILogger<QueueAutoscheduleSubscriber> logger)
        {
            this._dataVerifier = dataVerifier ?? throw new ArgumentNullException(nameof(dataVerifier));
            this._autoSchedule = autoSchedule ?? throw new ArgumentNullException(nameof(autoSchedule));
            this._awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this._awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this._serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            this._systemProcessContext =
                systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ExecuteCoordinationMessage(string messageId, string messageBody)
        {
            try
            {
                var opCtx = this._systemProcessContext.CreateAndStartOperationContext();

                this._logger.LogInformation(
                    $"QueueSubscriber read message {messageId} with body {messageBody} from {this._awsConfiguration.UploadCoordinatorQueueName} for operation {opCtx.Id}");

                var coordinateUpload = this.Deserialise(messageBody);

                if (coordinateUpload == null)
                {
                    this._logger.LogError($"QueueSubscriber was unable to deserialise the message {messageId}");
                    opCtx.EndEventWithError($"QueueSubscriber was unable to deserialise the message {messageId}");
                    return;
                }

                await this._dataVerifier.Scan();
                await this._autoSchedule.Scan();

                this._logger.LogInformation(
                    $"QueueSubscriber completed processing message {messageId} with body {messageBody} from {this._awsConfiguration.UploadCoordinatorQueueName} for operation {opCtx.Id}");
            }
            catch (Exception e)
            {
                this._logger.LogError(
                    "QueueSubscriber execute non distributed message encountered a top level exception.",
                    e);
            }
        }

        public void Initiate()
        {
            this._logger.LogInformation("QueueSubscriber initiating");
            this._messageBusCts?.Cancel();
            this._messageBusCts = new CancellationTokenSource();
            this._token = new AwsResusableCancellationToken();

            this._awsQueueClient.SubscribeToQueueAsync(
                this._awsConfiguration.UploadCoordinatorQueueName,
                async (s1, s2) => { await this.ExecuteCoordinationMessage(s1, s2); },
                this._messageBusCts.Token,
                this._token);

            this._logger.LogInformation("QueueSubscriber completed initiating");
        }

        public void Terminate()
        {
            this._logger.LogInformation(
                "QueueSubscriber sent terminate signal to cancellation token reading message bus");
            this._messageBusCts?.Cancel();
            this._messageBusCts = null;
        }

        private AutoScheduleMessage Deserialise(string messageBody)
        {
            try
            {
                return this._serialiser.Deserialise<AutoScheduleMessage>(messageBody);
            }
            catch
            {
                return null;
            }
        }
    }
}