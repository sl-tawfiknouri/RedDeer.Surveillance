using System;
using System.Threading.Tasks;
using Domain.Scheduling;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Interfaces;
using Utilities.Extensions;

namespace Surveillance.Universe
{
    public class UniverseRuleSubscriber : IUniverseRuleSubscriber
    {
        private readonly ISpoofingRuleFactory _spoofingRuleFactory;
        private readonly ICancelledOrderRuleFactory _cancelledOrderRuleFactory;
        private readonly IHighProfitRuleFactory _highProfitRuleFactory;
        private readonly IHighVolumeRuleFactory _highVolumeRuleFactory;
        private readonly IMarkingTheCloseRuleFactory _markingTheCloseFactory;
        private readonly ILayeringRuleFactory _layeringRuleFactory;

        private readonly IRuleParameterApiRepository _ruleParameterApiRepository;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly ILogger _logger;

        public UniverseRuleSubscriber(
            ISpoofingRuleFactory spoofingRuleFactory,
            ICancelledOrderRuleFactory cancelledOrderRuleFactory,
            IHighProfitRuleFactory highProfitRuleFactory,
            IMarkingTheCloseRuleFactory markingTheCloseFactory,
            ILayeringRuleFactory layeringRuleFactory,
            IHighVolumeRuleFactory highVolumeRuleFactory,
            IRuleParameterApiRepository ruleParameterApiRepository,
            IRuleParameterToRulesMapper ruleParameterMapper,
            ILogger<UniverseRuleSubscriber> logger)
        {
            _spoofingRuleFactory = spoofingRuleFactory ?? throw new ArgumentNullException(nameof(spoofingRuleFactory));
            _cancelledOrderRuleFactory = cancelledOrderRuleFactory ?? throw new ArgumentNullException(nameof(cancelledOrderRuleFactory));
            _highProfitRuleFactory = highProfitRuleFactory ?? throw new ArgumentNullException(nameof(highProfitRuleFactory));
            _highVolumeRuleFactory =
                highVolumeRuleFactory
                ?? throw new ArgumentNullException(nameof(highVolumeRuleFactory));
            _markingTheCloseFactory = markingTheCloseFactory ?? throw new ArgumentNullException(nameof(markingTheCloseFactory));
            _layeringRuleFactory = layeringRuleFactory ?? throw new ArgumentNullException(nameof(layeringRuleFactory));
            _ruleParameterApiRepository = ruleParameterApiRepository
                ?? throw new ArgumentNullException(nameof(ruleParameterApiRepository));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SubscribeRules(
             ScheduledExecution execution,
             IUniversePlayer player,
             ISystemProcessOperationContext opCtx)
        {
            if (execution == null
                || player == null)
            {
                return;
            }

            var ruleParameters = await _ruleParameterApiRepository.Get();

            SpoofingRule(execution, player, ruleParameters, opCtx);
            CancelledOrdersRule(execution, player, ruleParameters, opCtx);
            HighProfitsRule(execution, player, ruleParameters, opCtx);
            MarkingTheCloseRule(execution, player, ruleParameters, opCtx);
            LayeringRule(execution, player, ruleParameters, opCtx);
            HighVolumeRule(execution, player, ruleParameters, opCtx);
        }

        private void SpoofingRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx)
        {
            if (!execution.Rules.Contains(Domain.Scheduling.Rules.Spoofing))
            {
                return;
            }

            var spoofingParameters = _ruleParameterMapper.Map(ruleParameters.Spoofing);

            if (spoofingParameters != null)
            {
                var ruleCtx = opCtx
                    .CreateAndStartRuleRunContext(
                        Domain.Scheduling.Rules.Spoofing.GetDescription(),
                        _spoofingRuleFactory.RuleVersion,
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime);

                var spoofingRule = _spoofingRuleFactory.Build(spoofingParameters, ruleCtx);
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
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx)
        {
            if (!execution.Rules.Contains(Domain.Scheduling.Rules.CancelledOrders))
            {
                return;
            }

            var cancelledOrderParameters = _ruleParameterMapper.Map(ruleParameters.CancelledOrder);

            if (cancelledOrderParameters != null)
            {
                var ruleCtx = opCtx
                    .CreateAndStartRuleRunContext(
                        Domain.Scheduling.Rules.CancelledOrders.GetDescription(),
                        _cancelledOrderRuleFactory.Version,
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime);

                var cancelledOrderRule = _cancelledOrderRuleFactory.Build(cancelledOrderParameters, ruleCtx);
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
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx)
        {
            if (!execution.Rules.Contains(Domain.Scheduling.Rules.HighProfits))
            {
                return;
            }

            var highProfitParameters = _ruleParameterMapper.Map(ruleParameters.HighProfits);

            if (highProfitParameters != null)
            {
                var ruleCtx = opCtx
                    .CreateAndStartRuleRunContext(
                        Domain.Scheduling.Rules.HighProfits.GetDescription(),
                        _highProfitRuleFactory.RuleVersion,
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime);

                var highProfitsRule = _highProfitRuleFactory.Build(highProfitParameters, ruleCtx);
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
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx)
        {
            if (!execution.Rules.Contains(Domain.Scheduling.Rules.MarkingTheClose))
            {
                return;
            }

            var markingTheCloseParameters = _ruleParameterMapper.Map(ruleParameters.MarkingTheClose);

            if (markingTheCloseParameters != null)
            {
                var ruleCtx = opCtx
                    .CreateAndStartRuleRunContext(
                        Domain.Scheduling.Rules.MarkingTheClose.GetDescription(),
                        _markingTheCloseFactory.RuleVersion,
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime);

                var markingTheClose = _markingTheCloseFactory.Build(markingTheCloseParameters, ruleCtx);
                player.Subscribe(markingTheClose);
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a marking the close rule execution with no parameters set");
            }
        }

        private void LayeringRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx)
        {
            if (!execution.Rules.Contains(Domain.Scheduling.Rules.Layering))
            {
                return;
            }

            var layeringParameters = _ruleParameterMapper.Map(ruleParameters.Layering);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (layeringParameters != null)
            {
                var ruleCtx = opCtx
                    .CreateAndStartRuleRunContext(
                        Domain.Scheduling.Rules.Layering.GetDescription(),
                        _layeringRuleFactory.RuleVersion,
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime);

                var layering = _layeringRuleFactory.Build(layeringParameters, ruleCtx);
                player.Subscribe(layering);
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a layering rule execution with no parameters set");
            }
        }

        private void HighVolumeRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx)
        {
            if (!execution.Rules.Contains(Domain.Scheduling.Rules.HighVolume))
            {
                return;
            }

            var highVolumeParameters = _ruleParameterMapper.Map(ruleParameters.HighVolume);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (highVolumeParameters != null)
            {
                var ruleCtx = opCtx
                    .CreateAndStartRuleRunContext(
                        Domain.Scheduling.Rules.HighVolume.GetDescription(),
                        _highVolumeRuleFactory.RuleVersion,
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime);

                var highVolume = _highVolumeRuleFactory.Build(highVolumeParameters, ruleCtx);
                player.Subscribe(highVolume);
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a high volume rule execution with no parameters set");
            }
        }
    }
}
