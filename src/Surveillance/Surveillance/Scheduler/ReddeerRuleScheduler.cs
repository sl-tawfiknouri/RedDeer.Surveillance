using System;
using Surveillance.Factories.Interfaces;
using Surveillance.Scheduler.Interfaces;
using Surveillance.Universe.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Scheduling;
using Domain.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Subscriber.Factory;
using Surveillance.Analytics.Subscriber.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes;
using Surveillance.Utility.Interfaces;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.Scheduler
{
    public class ReddeerRuleScheduler : IReddeerRuleScheduler
    {
        private readonly IUniverseBuilder _universeBuilder;
        private readonly IUniversePlayerFactory _universePlayerFactory;
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;
        private readonly IApiHeartbeat _apiHeartbeat;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly IUniverseRuleSubscriber _ruleSubscriber;
        private readonly IUniverseAnalyticsSubscriberFactory _analyticsSubscriber;

        private readonly ILogger<ReddeerRuleScheduler> _logger;
        private CancellationTokenSource _messageBusCts;
        private AwsResusableCancellationToken _token;

        public ReddeerRuleScheduler(
            IUniverseBuilder universeBuilder,
            IUniversePlayerFactory universePlayerFactory,
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser messageBusSerialiser,
            IApiHeartbeat apiHeartbeat,
            ISystemProcessContext systemProcessContext,
            IUniverseRuleSubscriber ruleSubscriber,
            IUniverseAnalyticsSubscriberFactory analyticsSubscriber,
            ILogger<ReddeerRuleScheduler> logger)
        {
            _universeBuilder = universeBuilder ?? throw new ArgumentNullException(nameof(universeBuilder));

            _universePlayerFactory =
                universePlayerFactory
                ?? throw new ArgumentNullException(nameof(universePlayerFactory));

            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _messageBusSerialiser = messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            _apiHeartbeat = apiHeartbeat ?? throw new ArgumentNullException(nameof(apiHeartbeat));
            _systemProcessContext = systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            _ruleSubscriber = ruleSubscriber ?? throw new ArgumentNullException(nameof(ruleSubscriber));
            _analyticsSubscriber = analyticsSubscriber ?? throw new ArgumentNullException(nameof(analyticsSubscriber));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate()
        {
            _messageBusCts?.Cancel();

            _messageBusCts = new CancellationTokenSource();
            _token = new AwsResusableCancellationToken();

            _awsQueueClient.SubscribeToQueueAsync(
                _awsConfiguration.ScheduleRuleDistributedWorkQueueName,
                async (s1, s2) => { await ExecuteDistributedMessage(s1, s2); },
                _messageBusCts.Token,
                _token);
        }

        public void Terminate()
        {
            _messageBusCts?.Cancel();
            _messageBusCts = null;
        }

        public async Task ExecuteDistributedMessage(string messageId, string messageBody)
        {
            var opCtx = _systemProcessContext.CreateAndStartOperationContext();

            _logger.LogInformation($"ReddeerRuleScheduler read message {messageId} with body {messageBody} from {_awsConfiguration.ScheduleRuleDistributedWorkQueueName}");
            
            var servicesRunning = await _apiHeartbeat.HeartsBeating();

            if (!servicesRunning)
            {
                _logger.LogWarning("Reddeer Rule Scheduler asked to executed distributed message but was unable to reach api services");
                // set status here
                opCtx.UpdateEventState(OperationState.BlockedClientServiceDown);
                opCtx.EventError($"Reddeer Rule Scheduler asked to executed distributed message but was unable to reach api services");
            }

            int servicesDownMinutes = 0;
            var exitClientServiceBlock = servicesRunning;
            while (!exitClientServiceBlock)
            {
                Thread.Sleep(30 * 1000);
                var apiHeartBeat = _apiHeartbeat.HeartsBeating();
                apiHeartBeat.Wait();
                exitClientServiceBlock = apiHeartBeat.Result;
                servicesDownMinutes += 1;

                if (servicesDownMinutes == 60)
                {
                    _logger.LogError("Reddeer Rule Scheduler has been trying to process a message for over half an hour but the api services on the client service have been down");
                    opCtx.EventError($"Reddeer Rule Scheduler has been trying to process a message for over half an hour but the api services on the client service have been down");
                }
            }

            if (!servicesRunning)
            {
                _logger.LogWarning("Reddeer Rule Scheduler was unable to reach api services but is now able to");
                opCtx.UpdateEventState(OperationState.InProcess);
            }

            var execution = _messageBusSerialiser.DeserialisedScheduledExecution(messageBody);

            if (execution == null)
            {
                _logger.LogError($"ReddeerRuleScheduler was unable to deserialise the message {messageId}");
                opCtx.EndEventWithError($"ReddeerRuleScheduler was unable to deserialise the message {messageId}");
                return;
            }

            await Execute(execution, opCtx);
        }

        /// <summary>
        /// Once a message is picked up, deserialise the scheduled execution object
        /// and run execute
        /// </summary>
        public async Task Execute(ScheduledExecution execution, ISystemProcessOperationContext opCtx)
        {
            if (execution?.Rules == null
                || !execution.Rules.Any())
            {
                _logger.LogError($"ReddeerRuleScheduler was executing a schedule that did not specify any rules to run");
                opCtx.EndEventWithError($"ReddeerRuleScheduler was executing a schedule that did not specify any rules to run");
                return;
            }

            var universe = await _universeBuilder.Summon(execution, opCtx);
            var player = _universePlayerFactory.Build();
            var subscriber = _analyticsSubscriber.Build();

            await _ruleSubscriber.SubscribeRules(execution, player, opCtx);
            player.Subscribe(subscriber);

            player.Play(universe);
            opCtx.EndEvent();
        }
    }
}