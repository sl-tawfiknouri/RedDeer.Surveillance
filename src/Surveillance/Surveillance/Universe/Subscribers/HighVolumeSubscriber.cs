using Domain.Scheduling;
using System;
using System.Linq;
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
    public class HighVolumeSubscriber : IHighVolumeSubscriber
    {
        private readonly IHighVolumeRuleFactory _highVolumeRuleFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly ILogger<HighVolumeSubscriber> _logger;

        public HighVolumeSubscriber(
            IHighVolumeRuleFactory highVolumeRuleFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            ILogger<HighVolumeSubscriber> logger)
        {
            _highVolumeRuleFactory = highVolumeRuleFactory ?? throw new ArgumentNullException(nameof(highVolumeRuleFactory));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void HighVolumeRule(
          ScheduledExecution execution,
          IUniversePlayer player,
          RuleParameterDto ruleParameters,
          ISystemProcessOperationContext opCtx,
          IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Domain.Scheduling.Rules.HighVolume) ?? true)
            {
                return;
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .HighVolumes
                    .Where(hv => filteredParameters.Contains(hv.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var highVolumeParameters = _ruleParameterMapper.Map(dtos);

            SubscribeToUniverse(execution, player, opCtx, alertStream, highVolumeParameters);
        }

        private void SubscribeToUniverse(
            ScheduledExecution execution,
            IUniversePlayer player,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            global::System.Collections.Generic.IReadOnlyCollection<IHighVolumeRuleParameters> highVolumeParameters)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (highVolumeParameters != null
                && highVolumeParameters.Any())
            {
                foreach (var param in highVolumeParameters)
                {
                    SubscribeToParams(execution, player, opCtx, alertStream, param);
                }
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a high volume rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a high volume rule execution with no parameters set");
            }
        }

        private void SubscribeToParams(
            ScheduledExecution execution,
            IUniversePlayer player,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IHighVolumeRuleParameters param)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Scheduling.Rules.HighVolume.GetDescription(),
                    HighVolumeRuleFactory.Version,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId);

            var highVolume = _highVolumeRuleFactory.Build(param, ruleCtx, alertStream);

            if (param.HasFilters())
            {
                var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets);
                filteredUniverse.Subscribe(highVolume);
                player.Subscribe(filteredUniverse);
            }
            else
            {
                player.Subscribe(highVolume);
            }
        }
    }
}
