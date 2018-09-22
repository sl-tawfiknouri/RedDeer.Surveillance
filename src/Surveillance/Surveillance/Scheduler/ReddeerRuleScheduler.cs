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
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.Scheduler
{
    public class ReddeerRuleScheduler : IReddeerRuleScheduler
    {
        private readonly ISpoofingRuleFactory _spoofingRuleFactory;
        private readonly ICancelledOrderRuleFactory _cancelledOrderRuleFactory;
        private readonly IHighProfitRuleFactory _highProfitRuleFactory;
        private readonly IUniverseBuilder _universeBuilder;
        private readonly IUniversePlayerFactory _universePlayerFactory;
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;

        private readonly ILogger<ReddeerRuleScheduler> _logger;
        private CancellationTokenSource _messageBusCts;

        public ReddeerRuleScheduler(
            ISpoofingRuleFactory spoofingRuleFactory,
            ICancelledOrderRuleFactory cancelledOrderRuleFactory,
            IHighProfitRuleFactory highProfitRuleFactory,
            IUniverseBuilder universeBuilder,
            IUniversePlayerFactory universePlayerFactory,
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser messageBusSerialiser,
            ILogger<ReddeerRuleScheduler> logger)
        {
            _spoofingRuleFactory = 
                spoofingRuleFactory
                ?? throw new ArgumentNullException(nameof(spoofingRuleFactory));
            _cancelledOrderRuleFactory =
                cancelledOrderRuleFactory
                ?? throw new ArgumentNullException(nameof(cancelledOrderRuleFactory));
            _highProfitRuleFactory =
                highProfitRuleFactory
                ?? throw new ArgumentNullException(nameof(highProfitRuleFactory));

            _universeBuilder = universeBuilder ?? throw new ArgumentNullException(nameof(universeBuilder));

            _universePlayerFactory =
                universePlayerFactory
                ?? throw new ArgumentNullException(nameof(universePlayerFactory));

            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _messageBusSerialiser = messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
       
        public void Initiate()
        {
            _messageBusCts?.Cancel();

            _messageBusCts = new CancellationTokenSource();

            _awsQueueClient.SubscribeToQueueAsync(
                _awsConfiguration.ScheduledRuleQueueName,
                async (s1, s2) => { await ExecuteMessage(s1, s2); },
                _messageBusCts.Token);
        }

        public void Terminate()
        {
            _messageBusCts?.Cancel();
            _messageBusCts = null;
        }

        public async Task ExecuteMessage(string messageId, string messageBody)
        {
            _logger.LogInformation($"ReddeerRuleScheduler read message {messageId} with body {messageBody}");

            var execution = _messageBusSerialiser.DeserialisedScheduledExecution(messageBody);

            if (execution == null)
            {
                _logger.LogError($"ReddeerRuleScheduler was unable to deserialise the message {messageId}");
            }

            await Execute(execution);
        }

        /// <summary>
        /// Once a message is picked up, deserialise the scheduled execution object
        /// and run execute
        /// </summary>
        public async Task Execute(ScheduledExecution execution)
        {
            if (execution?.Rules == null
                || !execution.Rules.Any())
            {
                return;
            }

            var universe = await _universeBuilder.Summon(execution);
            var player = _universePlayerFactory.Build();

            SubscribeRules(execution, player);
            player.Play(universe);
        }

        private void SubscribeRules(
            ScheduledExecution execution,
            IUniversePlayer player)
        {
            if (execution == null
                || player == null)
            {
                return;
            }

            if (execution.Rules.Contains(Domain.Scheduling.Rules.Spoofing))
            {
                var spoofingRule = _spoofingRuleFactory.Build();
                player.Subscribe(spoofingRule);
            }

            if (execution.Rules.Contains(Domain.Scheduling.Rules.CancelledOrders))
            {
                var cancelledOrderRule = _cancelledOrderRuleFactory.Build();
                player.Subscribe(cancelledOrderRule);
            }

            if (execution.Rules.Contains(Domain.Scheduling.Rules.HighProfits))
            {
                var highProfitsRule = _highProfitRuleFactory.Build();
                player.Subscribe(highProfitsRule);
            }
        }
    }
}
