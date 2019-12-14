namespace Surveillance.Engine.Rules.Queues
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Infrastructure.Network.Aws;
    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService;
    using RedDeer.Contracts.SurveillanceService.Interfaces;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Queues.Interfaces;
    using Surveillance.Engine.Rules.Rules.Cancellation.Interfaces;

    public class QueueRuleCancellationSubscriber : IQueueRuleCancellationSubscriber
    {
        private readonly IAwsConfiguration _awsConfiguration;

        private readonly IAwsQueueClient _awsQueueClient;

        private readonly ILogger<QueueRuleCancellationSubscriber> _logger;

        private readonly IMessageBusSerialiser _messageBusSerialiser;

        private readonly IRuleCancellation _ruleCancellation;

        private readonly ISystemProcessContext _systemProcessContext;

        private CancellationTokenSource _messageBusCts;

        private AwsResusableCancellationToken _token;

        public QueueRuleCancellationSubscriber(
            IRuleCancellation ruleCancellation,
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ISystemProcessContext systemProcessContext,
            IMessageBusSerialiser messageBusSerialiser,
            ILogger<QueueRuleCancellationSubscriber> logger)
        {
            this._ruleCancellation = ruleCancellation ?? throw new ArgumentNullException(nameof(ruleCancellation));
            this._awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            this._awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this._systemProcessContext =
                systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            this._messageBusSerialiser =
                messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ExecuteCancellationMessage(string messageId, string messageBody)
        {
            var opCtx = this._systemProcessContext.CreateAndStartOperationContext();

            try
            {
                this._logger?.LogInformation(
                    $"ExecuteCancellationMessage {nameof(QueueRuleCancellationSubscriber)} initiated for {messageId}");

                var cancellationMessage =
                    this._messageBusSerialiser?.Deserialise<CancelScheduledExecutionMessage>(messageBody);
                this._ruleCancellation?.Cancel(cancellationMessage?.RuleRunId ?? string.Empty);

                this._logger?.LogInformation(
                    $"ExecuteCancellationMessage {nameof(QueueRuleCancellationSubscriber)} completed for {messageId}");

                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"caught exception in execute cancellation message");
                opCtx.EndEventWithError(e.Message);
            }
        }

        public void Initiate()
        {
            this._logger?.LogInformation($"Initiating {nameof(QueueRuleCancellationSubscriber)}");

            this._messageBusCts?.Cancel();
            this._messageBusCts = new CancellationTokenSource();
            this._token = new AwsResusableCancellationToken();

            this._awsQueueClient.SubscribeToQueueAsync(
                this._awsConfiguration.ScheduleRuleCancellationQueueName,
                async (a, b) => { await this.ExecuteCancellationMessage(a, b); },
                this._messageBusCts.Token,
                this._token);

            this._logger?.LogInformation($"Initiating {nameof(QueueRuleCancellationSubscriber)} completed");
        }

        public void Terminate()
        {
            this._logger?.LogInformation($"Terminating {nameof(QueueRuleCancellationSubscriber)}");
            this._messageBusCts?.Cancel();
            this._messageBusCts = null;
            this._logger?.LogInformation($"Terminating {nameof(QueueRuleCancellationSubscriber)} completed");
        }
    }
}