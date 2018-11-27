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
    public class MarkingTheCloseSubscriber : IMarkingTheCloseSubscriber
    {
        private readonly IMarkingTheCloseRuleFactory _markingTheCloseFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly ILogger<MarkingTheCloseSubscriber> _logger;

        public MarkingTheCloseSubscriber(
            IMarkingTheCloseRuleFactory markingTheCloseFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            ILogger<MarkingTheCloseSubscriber> logger)
        {
            _markingTheCloseFactory = markingTheCloseFactory ?? throw new ArgumentNullException(nameof(markingTheCloseFactory));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void MarkingTheCloseRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Domain.Scheduling.Rules.MarkingTheClose) ?? true)
            {
                return;
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .MarkingTheCloses
                    .Where(mtc => filteredParameters.Contains(mtc.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var markingTheCloseParameters = _ruleParameterMapper.Map(dtos);

            SubscribeToUniverse(execution, player, opCtx, alertStream, markingTheCloseParameters);
        }

        private void SubscribeToUniverse(
            ScheduledExecution execution,
            IUniversePlayer player,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            global::System.Collections.Generic.IReadOnlyCollection<Rules.MarkingTheClose.Interfaces.IMarkingTheCloseParameters> markingTheCloseParameters)
        {
            if (markingTheCloseParameters != null
                && markingTheCloseParameters.Any())
            {
                foreach (var param in markingTheCloseParameters)
                {
                    SubscribeToParams(execution, player, opCtx, alertStream, param);
                }
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a marking the close rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a marking the close rule execution with no parameters set");
            }
        }

        private void SubscribeToParams(
            ScheduledExecution execution,
            IUniversePlayer player,
            ISystemProcessOperationContext opCtx, 
            IUniverseAlertStream alertStream,
            Rules.MarkingTheClose.Interfaces.IMarkingTheCloseParameters param)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Scheduling.Rules.MarkingTheClose.GetDescription(),
                    MarkingTheCloseRuleFactory.Version,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId);

            var markingTheClose = _markingTheCloseFactory.Build(param, ruleCtx, alertStream);

            if (param.HasFilters())
            {
                var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets);
                filteredUniverse.Subscribe(markingTheClose);
                player.Subscribe(filteredUniverse);
            }
            else
            {
                player.Subscribe(markingTheClose);
            }
        }
    }
}
