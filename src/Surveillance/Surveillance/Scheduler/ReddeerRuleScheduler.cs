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
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.Scheduler
{
    public class ReddeerRuleScheduler : IReddeerRuleScheduler
    {
        private readonly ISpoofingRuleFactory _spoofingRuleFactory;
        private readonly ICancelledOrderRuleFactory _cancelledOrderRuleFactory;
        private readonly IHighProfitRuleFactory _highProfitRuleFactory;
        private readonly IMarkingTheCloseRuleFactory _markingTheCloseFactory;
        private readonly IUniverseBuilder _universeBuilder;
        private readonly IUniversePlayerFactory _universePlayerFactory;
        private readonly IAwsQueueClient _awsQueueClient;
        private readonly IAwsConfiguration _awsConfiguration;
        private readonly IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;
        private readonly IRuleParameterApiRepository _ruleParameterApiRepository;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;

        private readonly ILogger<ReddeerRuleScheduler> _logger;
        private CancellationTokenSource _messageBusCts;

        public ReddeerRuleScheduler(
            ISpoofingRuleFactory spoofingRuleFactory,
            ICancelledOrderRuleFactory cancelledOrderRuleFactory,
            IHighProfitRuleFactory highProfitRuleFactory,
            IMarkingTheCloseRuleFactory markingTheCloseFactory,
            IUniverseBuilder universeBuilder,
            IUniversePlayerFactory universePlayerFactory,
            IAwsQueueClient awsQueueClient,
            IAwsConfiguration awsConfiguration,
            IScheduledExecutionMessageBusSerialiser messageBusSerialiser,
            IRuleParameterApiRepository ruleParameterApiRepository,
            IRuleParameterToRulesMapper ruleParameterMapper,
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
            _markingTheCloseFactory =
                markingTheCloseFactory
                ?? throw new ArgumentNullException(nameof(markingTheCloseFactory));

            _universeBuilder = universeBuilder ?? throw new ArgumentNullException(nameof(universeBuilder));

            _universePlayerFactory =
                universePlayerFactory
                ?? throw new ArgumentNullException(nameof(universePlayerFactory));

            _awsQueueClient = awsQueueClient ?? throw new ArgumentNullException(nameof(awsQueueClient));
            _awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            _messageBusSerialiser = messageBusSerialiser ?? throw new ArgumentNullException(nameof(messageBusSerialiser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _ruleParameterApiRepository =
                ruleParameterApiRepository
                ?? throw new ArgumentNullException(nameof(ruleParameterApiRepository));

            _ruleParameterMapper =
                ruleParameterMapper
                ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
        }
       
        public void Initiate()
        {
            _messageBusCts?.Cancel();

            _messageBusCts = new CancellationTokenSource();

            _awsQueueClient.SubscribeToQueueAsync(
                _awsConfiguration.ScheduleRuleDistributedWorkQueueName,
                async (s1, s2) => { await ExecuteDistributedMessage(s1, s2); },
                _messageBusCts.Token);
        }

        public void Terminate()
        {
            _messageBusCts?.Cancel();
            _messageBusCts = null;
        }

        public async Task ExecuteDistributedMessage(string messageId, string messageBody)
        {
            _logger.LogInformation($"ReddeerRuleScheduler read message {messageId} with body {messageBody} from {_awsConfiguration.ScheduleRuleDistributedWorkQueueName}");

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

            await SubscribeRules(execution, player);
            player.Play(universe);
        }

        private async Task SubscribeRules(
            ScheduledExecution execution,
            IUniversePlayer player)
        {
            if (execution == null
                || player == null)
            {
                return;
            }

            var ruleParameters = await _ruleParameterApiRepository.Get();

            SpoofingRule(execution, player, ruleParameters);
            CancelledOrdersRule(execution, player, ruleParameters);
            HighProfitsRule(execution, player, ruleParameters);
            MarkingTheCloseRule(execution, player, ruleParameters);
        }

        private void SpoofingRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters)
        {
            if (!execution.Rules.Contains(Domain.Scheduling.Rules.Spoofing))
            {
                return;
            }

            var spoofingParameters = _ruleParameterMapper.Map(ruleParameters.Spoofing);

            if (spoofingParameters != null)
            {
                var spoofingRule = _spoofingRuleFactory.Build(spoofingParameters);
                player.Subscribe(spoofingRule);
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a spoofing rule execution with no parameters set");
            }
        }

        private void CancelledOrdersRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters)
        {
            if (!execution.Rules.Contains(Domain.Scheduling.Rules.CancelledOrders))
            {
                return;
            }

            var cancelledOrderParameters = _ruleParameterMapper.Map(ruleParameters.CancelledOrder);

            if (cancelledOrderParameters != null)
            {
                var cancelledOrderRule = _cancelledOrderRuleFactory.Build(cancelledOrderParameters);
                player.Subscribe(cancelledOrderRule);
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a cancelled order rule execution with no parameters set");
            }
        }

        private void HighProfitsRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters)
        {
            if (!execution.Rules.Contains(Domain.Scheduling.Rules.HighProfits))
            {
                return;
            }
            
            var highProfitParameters = _ruleParameterMapper.Map(ruleParameters.HighProfits);

            if (highProfitParameters != null)
            {
                var highProfitsRule = _highProfitRuleFactory.Build(highProfitParameters);
                player.Subscribe(highProfitsRule);
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a high profit rule execution with no parameters set");
            }
        }

        private void MarkingTheCloseRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters)
        {
            if (!execution.Rules.Contains(Domain.Scheduling.Rules.MarkingTheClose))
            {
                return;
            }

            var markingTheCloseParameters = _ruleParameterMapper.Map(ruleParameters.MarkingTheClose);

            if (markingTheCloseParameters != null)
            {
                var markingTheClose = _markingTheCloseFactory.Build(markingTheCloseParameters);
                player.Subscribe(markingTheClose);
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a marking the close rule execution with no parameters set");
            }
        }
    }
}