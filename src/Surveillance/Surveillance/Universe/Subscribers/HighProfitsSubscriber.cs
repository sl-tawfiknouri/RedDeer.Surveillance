using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Scheduling;
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
    public class HighProfitsSubscriber : IHighProfitsSubscriber
    {
        private readonly IHighProfitRuleFactory _highProfitRuleFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IOrganisationalFactorBrokerFactory _brokerFactory;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly ILogger _logger;

        public HighProfitsSubscriber(
            IHighProfitRuleFactory highProfitRuleFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerFactory brokerFactor,
            ILogger<UniverseRuleSubscriber> logger)
        {
            _highProfitRuleFactory = highProfitRuleFactory ?? throw new ArgumentNullException(nameof(highProfitRuleFactory));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _brokerFactory = brokerFactor ?? throw new ArgumentNullException(nameof(brokerFactor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(DomainV2.Scheduling.Rules.HighProfits) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .HighProfits
                    .Where(hp => filteredParameters.Contains(hp.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var highProfitParameters = _ruleParameterMapper.Map(dtos);

            return SubscribeToUniverse(execution, opCtx, alertStream, highProfitParameters);
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IReadOnlyCollection<IHighProfitsRuleParameters> highProfitParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (highProfitParameters != null
                && highProfitParameters.Any())
            {
                foreach (var param in highProfitParameters)
                {
                    var cloneableRule = SubscribeParameters(execution, opCtx, alertStream, param);
                    var broker =
                        _brokerFactory.Build(
                            cloneableRule,
                            param.Factors,
                            param.AggregateNonFactorableIntoOwnCategory);

                    subscriptions.Add(broker);
                }
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a high profit rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a high profit rule execution with no parameters set");
            }

            return subscriptions;
        }

        private IUniverseCloneableRule SubscribeParameters(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IHighProfitsRuleParameters param)
        {
            var ruleCtxStream = opCtx
                    .CreateAndStartRuleRunContext(
                        DomainV2.Scheduling.Rules.HighProfits.GetDescription(),
                        HighProfitRuleFactory.Version,
                        param.Id,
                        (int)DomainV2.Scheduling.Rules.HighProfits,
                        execution.IsBackTest,
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime,
                        execution.CorrelationId,
                        execution.IsForceRerun);

            var ruleCtxMarketClosure = opCtx
                .CreateAndStartRuleRunContext(
                    DomainV2.Scheduling.Rules.HighProfits.GetDescription(),
                    HighProfitRuleFactory.Version,
                    param.Id,
                    (int)DomainV2.Scheduling.Rules.HighProfits,
                    execution.IsBackTest,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId,
                    execution.IsForceRerun);

            var highProfitsRule = _highProfitRuleFactory.Build(param, ruleCtxStream, ruleCtxMarketClosure, alertStream, execution);

            if (param.HasFilters())
            {
                _logger.LogInformation($"HighProfitsSubscriber parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");

                var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets);
                filteredUniverse.Subscribe(highProfitsRule);

                return filteredUniverse;
            }
            else
            {
                return highProfitsRule;
            }
        }
    }
}
