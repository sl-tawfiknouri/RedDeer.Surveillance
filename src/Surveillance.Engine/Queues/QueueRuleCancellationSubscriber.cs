using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Network.Aws;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Interfaces;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;

namespace Surveillance.Engine.Rules.Queues
{
    public class QueueRuleCancellationSubscriber : IQueueRuleCancellationSubscriber
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly IMessageBusSerialiser _messageBusSerialiser;

        private readonly ILogger<QueueRuleCancellationSubscriber> _logger;
        private CancellationTokenSource _messageBusCts;
        private AwsResusableCancellationToken _token;

        public QueueRuleCancellationSubscriber(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ISystemProcessContext systemProcessContext,
            IMessageBusSerialiser messageBusSerialiser,
            ILogger<QueueRuleCancellationSubscriber> logger)
        {
            _awsQueueClient = awsQueueClient;
            _awsConfiguration = awsConfiguration;
            _systemProcessContext = systemProcessContext;
            _messageBusSerialiser = messageBusSerialiser;
            _logger = logger;
        }

        public void Initiate()
        {
            _logger?.LogInformation($"Initiating {nameof(QueueRuleCancellationSubscriber)}");

            _messageBusCts?.Cancel();
            _messageBusCts = new CancellationTokenSource();
            _token = new AwsResusableCancellationToken();

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

                var cancellationMessage = _messageBusSerialiser?.Deserialise<string>(messageBody);


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
