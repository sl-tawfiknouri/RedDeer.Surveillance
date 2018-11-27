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
    public class SpoofingSubscriber : ISpoofingSubscriber
    {
        private readonly ISpoofingRuleFactory _spoofingRuleFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly ILogger _logger;

        public SpoofingSubscriber(
            ISpoofingRuleFactory spoofingRuleFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            ILogger<UniverseRuleSubscriber> logger)
        {
            _spoofingRuleFactory = spoofingRuleFactory ?? throw new ArgumentNullException(nameof(spoofingRuleFactory));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public void SpoofingRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ab => ab.Rule)?.ToList().Contains(Domain.Scheduling.Rules.Spoofing)
                ?? true)
            {
                return;
            }

            var filteredParameters =
                execution
                    .Rules
                    .SelectMany(ru => ru.Ids)
                    .Where(ru => ru != null)
                    .ToList();

            var dtos =
                ruleParameters
                    .Spoofings
                    .Where(sp => filteredParameters.Contains(sp.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var spoofingParameters = _ruleParameterMapper.Map(dtos);

            SubscribeToUniverse(execution, player, opCtx, alertStream, spoofingParameters);
        }

        private void SubscribeToUniverse(
            ScheduledExecution execution,
            IUniversePlayer player,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            global::System.Collections.Generic.IReadOnlyCollection<ISpoofingRuleParameters> spoofingParameters)
        {
            if (spoofingParameters != null
                && spoofingParameters.Any())
            {
                foreach (var param in spoofingParameters)
                {
                    SubscribeForParams(execution, player, opCtx, alertStream, param);
                }
            }
            else
            {
                _logger.LogError("Spoofing Rule Scheduler - tried to schedule a spoofing rule execution with no parameters set");
                opCtx.EventError("Spoofing Scheduler - tried to schedule a spoofing rule execution with no parameters set");
            }
        }

        private void SubscribeForParams(
            ScheduledExecution execution,
            IUniversePlayer player,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            ISpoofingRuleParameters param)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Scheduling.Rules.Spoofing.GetDescription(),
                    SpoofingRuleFactory.Version,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId);

            var spoofingRule = _spoofingRuleFactory.Build(param, ruleCtx, alertStream);

            if (param.HasFilters())
            {
                var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets);
                filteredUniverse.Subscribe(spoofingRule);
                player.Subscribe(filteredUniverse);
            }
            else
            {
                player.Subscribe(spoofingRule);
            }
        }
    }
}
