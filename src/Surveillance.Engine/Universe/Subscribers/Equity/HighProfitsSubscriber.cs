using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Scheduling;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;
using Utilities.Extensions;

namespace Surveillance.Engine.Rules.Universe.Subscribers.Equity
{
    public class HighProfitsSubscriber : IHighProfitsSubscriber
    {
        private readonly IEquityRuleHighProfitFactory _equityRuleHighProfitFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IOrganisationalFactorBrokerFactory _brokerFactory;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly ILogger _logger;

        public HighProfitsSubscriber(
            IEquityRuleHighProfitFactory equityRuleHighProfitFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerFactory brokerFactor,
            ILogger<UniverseRuleSubscriber> logger)
        {
            _equityRuleHighProfitFactory = equityRuleHighProfitFactory ?? throw new ArgumentNullException(nameof(equityRuleHighProfitFactory));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _brokerFactory = brokerFactor ?? throw new ArgumentNullException(nameof(brokerFactor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Domain.Scheduling.Rules.HighProfits) ?? true)
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

            return SubscribeToUniverse(execution, opCtx, alertStream, dataRequestSubscriber, highProfitParameters);
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IReadOnlyCollection<IHighProfitsRuleParameters> highProfitParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (highProfitParameters != null
                && highProfitParameters.Any())
            {
                foreach (var param in highProfitParameters)
                {
                    var cloneableRule = SubscribeParameters(execution, opCtx, alertStream, dataRequestSubscriber, param);
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
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IHighProfitsRuleParameters param)
        {
            var ruleCtxStream = opCtx
                    .CreateAndStartRuleRunContext(
                        Domain.Scheduling.Rules.HighProfits.GetDescription(),
                        EquityRuleHighProfitFactory.Version,
                        param.Id,
                        (int)Domain.Scheduling.Rules.HighProfits,
                        execution.IsBackTest,
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime,
                        execution.CorrelationId,
                        execution.IsForceRerun);

            var ruleCtxMarketClosure = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Scheduling.Rules.HighProfits.GetDescription(),
                    EquityRuleHighProfitFactory.Version,
                    param.Id,
                    (int)Domain.Scheduling.Rules.HighProfits,
                    execution.IsBackTest,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId,
                    execution.IsForceRerun);

            var highProfitsRule = _equityRuleHighProfitFactory.Build(param, ruleCtxStream, ruleCtxMarketClosure, alertStream, dataRequestSubscriber, execution);

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
