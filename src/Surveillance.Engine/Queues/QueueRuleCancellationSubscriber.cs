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

namespace Surveillance.Engine.Rules.Queues
{
    public class QueueRuleCancellationSubscriber : IQueueRuleCancellationSubscriber
    {
        private readonly IRuleCancellation _ruleCancellation;
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly IMessageBusSerialiser _messageBusSerialiser;
        private readonly IQueueRuleCancellationInfrastructureBuilder _ruleCancellationInfrastructureBuilder;

        private readonly ILogger<QueueRuleCancellationSubscriber> _logger;
        private CancellationTokenSource _messageBusCts;
        private AwsResusableCancellationToken _token;

        public QueueRuleCancellationSubscriber(
            IRuleCancellation ruleCancellation,
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ISystemProcessContext systemProcessContext,
            IMessageBusSerialiser messageBusSerialiser,
            IQueueRuleCancellationInfrastructureBuilder ruleCancellationInfrastructureBuilder,
            ILogger<QueueRuleCancellationSubscriber> logger)
        {
            _ruleCancellation = ruleCancellation ?? throw new ArgumentNullException(nameof(ruleCancellation));
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _systemProcessContext = systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            _messageBusSerialiser = messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            _ruleCancellationInfrastructureBuilder = ruleCancellationInfrastructureBuilder ?? throw new ArgumentNullException(nameof(ruleCancellationInfrastructureBuilder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger?.LogInformation($"Initiating {nameof(QueueRuleCancellationSubscriber)}");

            _messageBusCts?.Cancel();
            _messageBusCts = new CancellationTokenSource();
            _token = new AwsResusableCancellationToken();

            _ruleCancellationInfrastructureBuilder.Setup().Wait();

            _awsQueueClient.SubscribeToQueueAsync(
                _awsConfiguration.ScheduleRuleCancellationQueueName,
                async (a, b) => { await ExecuteCancellationMessage(a, b); },
                _messageBusCts.Token,
                _token);

            _logger?.LogInformation($"Initiating {nameof(QueueRuleCancellationSubscriber)} completed");
        }

        public void Terminate()
        {
            _logger?.LogInformation($"Terminating {nameof(QueueRuleCancellationSubscriber)}");
            _messageBusCts?.Cancel();
            _messageBusCts = null;
            _logger?.LogInformation($"Terminating {nameof(QueueRuleCancellationSubscriber)} completed");
        }

        public async Task ExecuteCancellationMessage(string messageId, string messageBody)
        {
            var opCtx = _systemProcessContext.CreateAndStartOperationContext();

            try
            {
                _logger?.LogInformation($"ExecuteCancellationMessage {nameof(QueueRuleCancellationSubscriber)} initiated for {messageId}");

                var cancellationMessage = _messageBusSerialiser?.Deserialise<CancelScheduledExecutionMessage>(messageBody);
                _ruleCancellation?.Cancel(cancellationMessage?.RuleRunId ?? string.Empty);

                _logger?.LogInformation($"ExecuteCancellationMessage {nameof(QueueRuleCancellationSubscriber)} completed for {messageId}");
            }
            catch (Exception e)
            {
                _logger.LogError($"caught exception in execute cancellation message {e.Message} {e.InnerException}", e);
                opCtx.EndEventWithError(e.Message);
            }
        }
    }
}
