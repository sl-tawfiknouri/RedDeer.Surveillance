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
    /// The queue auto schedule subscriber.
    /// </summary>
    public class QueueAutoScheduleSubscriber : IQueueSubscriber
    {
        /// <summary>
        /// The auto schedule.
        /// </summary>
        private readonly IAutoSchedule autoSchedule;

        /// <summary>
        /// The aws configuration.
        /// </summary>
        private readonly IAwsConfiguration awsConfiguration;

        /// <summary>
        /// The aws queue client.
        /// </summary>
        private readonly IAwsQueueClient awsQueueClient;

        /// <summary>
        /// The data verifier.
        /// </summary>
        private readonly IDataVerifier dataVerifier;

        /// <summary>
        /// The serializer.
        /// </summary>
        private readonly IMessageBusSerialiser serializer;

        /// <summary>
        /// The system process context.
        /// </summary>
        private readonly ISystemProcessContext systemProcessContext;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<QueueAutoScheduleSubscriber> logger;

        /// <summary>
        /// The message bus cancellation token source.
        /// </summary>
        private CancellationTokenSource messageBusCancellationTokenSource;

        /// <summary>
        /// The token.
        /// </summary>
        private AwsResusableCancellationToken token;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueAutoScheduleSubscriber"/> class.
        /// </summary>
        /// <param name="dataVerifier">
        /// The data verifier.
        /// </param>
        /// <param name="autoSchedule">
        /// The auto schedule.
        /// </param>
        /// <param name="awsQueueClient">
        /// The aws queue client.
        /// </param>
        /// <param name="awsConfiguration">
        /// The aws configuration.
        /// </param>
        /// <param name="serialiser">
        /// The serializer.
        /// </param>
        /// <param name="systemProcessContext">
        /// The system process context.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public QueueAutoScheduleSubscriber(
            IDataVerifier dataVerifier,
            IAutoSchedule autoSchedule,
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IMessageBusSerialiser serialiser,
            ISystemProcessContext systemProcessContext,
            ILogger<QueueAutoScheduleSubscriber> logger)
        {
            this.dataVerifier = dataVerifier ?? throw new ArgumentNullException(nameof(dataVerifier));
            this.autoSchedule = autoSchedule ?? throw new ArgumentNullException(nameof(autoSchedule));
            this.awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this.awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this.serializer = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            this.systemProcessContext = systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The execute coordination message async.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="messageBody">
        /// The message body.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task ExecuteCoordinationMessageAsync(string messageId, string messageBody)
        {
            try
            {
                var operationContext = this.systemProcessContext.CreateAndStartOperationContext();

                this.logger.LogInformation(
                    $"QueueSubscriber read message {messageId} with body {messageBody} from {this.awsConfiguration.UploadCoordinatorQueueName} for operation {operationContext.Id}");

                var coordinateUpload = this.Deserialise(messageBody);

                if (coordinateUpload == null)
                {
                    this.logger.LogError($"QueueSubscriber was unable to deserialise the message {messageId}");
                    operationContext.EndEventWithError($"QueueSubscriber was unable to deserialise the message {messageId}");
                    return;
                }

                await this.dataVerifier.Scan();
                await this.autoSchedule.Scan();

                this.logger.LogInformation(
                    $"QueueSubscriber completed processing message {messageId} with body {messageBody} from {this.awsConfiguration.UploadCoordinatorQueueName} for operation {operationContext.Id}");
            }
            catch (Exception e)
            {
                this.logger.LogError(
                    "QueueSubscriber execute non distributed message encountered a top level exception.",
                    e);
            }
        }

        /// <summary>
        /// The initiate.
        /// </summary>
        public void Initiate()
        {
            this.logger.LogInformation("QueueSubscriber initiating");
            this.messageBusCancellationTokenSource?.Cancel();
            this.messageBusCancellationTokenSource = new CancellationTokenSource();
            this.token = new AwsResusableCancellationToken();

            this.awsQueueClient.SubscribeToQueueAsync(
                this.awsConfiguration.UploadCoordinatorQueueName,
                async (s1, s2) => { await this.ExecuteCoordinationMessageAsync(s1, s2); },
                this.messageBusCancellationTokenSource.Token,
                this.token);

            this.logger.LogInformation("QueueSubscriber completed initiating");
        }

        /// <summary>
        /// The terminate.
        /// </summary>
        public void Terminate()
        {
            this.logger.LogInformation(
                "QueueSubscriber sent terminate signal to cancellation token reading message bus");
            this.messageBusCancellationTokenSource?.Cancel();
            this.messageBusCancellationTokenSource = null;
        }

        /// <summary>
        /// The deserialize message to auto schedule message.
        /// </summary>
        /// <param name="messageBody">
        /// The message body.
        /// </param>
        /// <returns>
        /// The <see cref="AutoScheduleMessage"/>.
        /// </returns>
        private AutoScheduleMessage Deserialise(string messageBody)
        {
            try
            {
                return this.serializer.Deserialise<AutoScheduleMessage>(messageBody);
            }
            catch
            {
                return null;
            }
        }
    }
}