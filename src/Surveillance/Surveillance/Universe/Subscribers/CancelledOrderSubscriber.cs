using System;
using System.Linq;
using Domain.Scheduling;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.Subscribers.Interfaces;
using Utilities.Extensions;

namespace Surveillance.Universe.Subscribers
{
    public class CancelledOrderSubscriber : ICancelledOrderSubscriber
    {
        private readonly ICancelledOrderRuleFactory _cancelledOrderRuleFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly ILogger<CancelledOrderSubscriber> _logger;

        public CancelledOrderSubscriber(
            ICancelledOrderRuleFactory cancelledOrderRuleFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            ILogger<CancelledOrderSubscriber> logger)
        {
            _cancelledOrderRuleFactory = cancelledOrderRuleFactory;
            _ruleParameterMapper = ruleParameterMapper;
            _universeFilterFactory = universeFilterFactory;
            _logger = logger;
        }

        public void CancelledOrdersRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ab => ab.Rule)?.Contains(Domain.Scheduling.Rules.CancelledOrders) ?? true)
            {
                return;
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .CancelledOrders
                    .Where(co => filteredParameters.Contains(co.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var cancelledOrderParameters = _ruleParameterMapper.Map(dtos);

            SubscribeToUniverse(execution, player, opCtx, alertStream, cancelledOrderParameters);
        }

        private void SubscribeToUniverse(
            ScheduledExecution execution,
            IUniversePlayer player,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            global::System.Collections.Generic.IReadOnlyCollection<ICancelledOrderRuleParameters> cancelledOrderParameters)
        {
            if (cancelledOrderParameters != null
                && cancelledOrderParameters.Any())
            {
                foreach (var param in cancelledOrderParameters)
                {
                    SubscribeParamToUniverse(execution, player, opCtx, alertStream, param);
                }
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a cancelled order rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a cancelled order rule execution with no parameters set");
            }
        }

        private void SubscribeParamToUniverse(
            ScheduledExecution execution,
            IUniversePlayer player,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            ICancelledOrderRuleParameters param)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Scheduling.Rules.CancelledOrders.GetDescription(),
                    CancelledOrderRuleFactory.Version,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId);

            var cancelledOrderRule = _cancelledOrderRuleFactory.Build(param, ruleCtx, alertStream);

            if (param.HasFilters())
            {
                var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets);
                filteredUniverse.Subscribe(cancelledOrderRule);
                player.Subscribe(filteredUniverse);
            }
            else
            {
                player.Subscribe(cancelledOrderRule);
            }
        }
    }
}
