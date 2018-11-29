using Domain.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.OrganisationalFactors.Interfaces;
using Surveillance.Universe.Subscribers.Interfaces;
using Utilities.Extensions;

namespace Surveillance.Universe.Subscribers
{
    public class HighVolumeSubscriber : IHighVolumeSubscriber
    {
        private readonly IHighVolumeRuleFactory _highVolumeRuleFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IOrganisationalFactorBrokerFactory _brokerFactory;
        private readonly ILogger<HighVolumeSubscriber> _logger;

        public HighVolumeSubscriber(
            IHighVolumeRuleFactory highVolumeRuleFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerFactory brokerFactory,
            ILogger<HighVolumeSubscriber> logger)
        {
            _highVolumeRuleFactory = highVolumeRuleFactory ?? throw new ArgumentNullException(nameof(highVolumeRuleFactory));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _brokerFactory = brokerFactory ?? throw new ArgumentNullException(nameof(brokerFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Domain.Scheduling.Rules.HighVolume) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .HighVolumes
                    .Where(hv => filteredParameters.Contains(hv.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var highVolumeParameters = _ruleParameterMapper.Map(dtos);

            var subscriptions = SubscribeToUniverse(execution, opCtx, alertStream, highVolumeParameters);

            return subscriptions;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IReadOnlyCollection<IHighVolumeRuleParameters> highVolumeParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (highVolumeParameters != null
                && highVolumeParameters.Any())
            {
                foreach (var param in highVolumeParameters)
                {
                    var paramSubscriptions = SubscribeToParams(execution, opCtx, alertStream, param);
                    var broker =
                        _brokerFactory.Build(
                            paramSubscriptions,
                            param.Factors,
                            param.AggregateNonFactorableIntoOwnCategory);

                    subscriptions.Add(broker);
                }
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a high volume rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a high volume rule execution with no parameters set");
            }

            return subscriptions;
        }

        private IUniverseCloneableRule SubscribeToParams(
            ScheduledExecution execution,
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

                return filteredUniverse;
            }
            else
            {
                return highVolume;
            }
        }
    }
}
