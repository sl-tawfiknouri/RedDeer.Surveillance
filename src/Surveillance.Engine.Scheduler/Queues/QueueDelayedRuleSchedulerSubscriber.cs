using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Surveillance.Scheduling;
using Infrastructure.Network.Aws;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Interfaces;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Scheduler.Queues.Interfaces;

namespace Surveillance.Engine.Scheduler.Queues
{
    public class QueueDelayedRuleSchedulerSubscriber : IQueueDelayedRuleSchedulerSubscriber
    {
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly IMessageBusSerialiser _messageBusSerialiser;
        private readonly ILogger<QueueDelayedRuleSchedulerSubscriber> _logger;

        private CancellationTokenSource _messageBusCts;
        private AwsResusableCancellationToken _token;

        public QueueDelayedRuleSchedulerSubscriber(
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            ISystemProcessContext systemProcessContext,
            IMessageBusSerialiser messageBusSerialiser,
            ILogger<QueueDelayedRuleSchedulerSubscriber> logger)
        {
            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _systemProcessContext = systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            _messageBusSerialiser = messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _logger.LogInformation($"initiating");

            _messageBusCts = new CancellationTokenSource();
            _token = new AwsResusableCancellationToken();

            _awsQueueClient.SubscribeToQueueAsync(
                _awsConfiguration.ScheduleDelayedRuleRunQueueName,
                async (s1, s2) => { await ExecuteDelayedRuleMessage(s1, s2); },
                _messageBusCts.Token,
                _token);

            _logger.LogInformation($"completed initiation");
        }

        public void Terminate()
        {
            _logger.LogInformation($"terminating");

            _messageBusCts?.Cancel();
            _messageBusCts = null;

            _logger.LogInformation($"completed termination");
        }

        public async Task ExecuteDelayedRuleMessage(string messageId, string messageBody)
        {
            var opCtx = _systemProcessContext.CreateAndStartOperationContext();

            try
            {
                _logger.LogInformation($"initiating processing of execute delayed rule message for {messageId}");

                var request = _messageBusSerialiser.Deserialise<AdHocScheduleRequest>(messageBody);

                if (request == null)
                {
                    _logger?.LogError($"Deserialised {messageId} but was null {messageBody}");
                    return;
                }

                opCtx.EndEvent();
                _logger.LogInformation($"terminating processing of execute delayed rule message for {messageId}");
            }
            catch (Exception e)
            {
                _logger.LogError($"caught exception in execute delayed rule {messageBody}", e);
                opCtx.EndEventWithError(e.Message);
            }
        }
    }
}
