using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Extensions;
using Domain.Surveillance.Scheduling;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Subscribers.Equity
{
    public class RampingEquitySubscriber : IRampingEquitySubscriber
    {
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;
        private readonly IEquityRuleRampingFactory _equityRuleRampingFactory;
        private readonly ILogger<RampingEquitySubscriber> _logger;

        public RampingEquitySubscriber(
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IEquityRuleRampingFactory equityRuleRampingFactory,
            ILogger<RampingEquitySubscriber> logger)
        {
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _brokerServiceFactory = brokerServiceFactory ?? throw new ArgumentNullException(nameof(brokerServiceFactory));
            _equityRuleRampingFactory = equityRuleRampingFactory ?? throw new ArgumentNullException(nameof(equityRuleRampingFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(_ => _.Rule)?.Contains(Domain.Surveillance.Scheduling.Rules.Ramping) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .Rampings
                    .Where(_ => filteredParameters.Contains(_.Id, StringComparer.OrdinalIgnoreCase))
                    .ToList();

            var rampingParameters = _ruleParameterMapper.Map(dtos);

            return SubscribeToUniverse(execution, opCtx, alertStream, rampingParameters, dataRequestSubscriber);
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IReadOnlyCollection<IRampingRuleEquitiesParameters> rampingParameters,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (rampingParameters != null
                && rampingParameters.Any())
            {
                foreach (var param in rampingParameters)
                {
                    var baseSubscriber = SubscribeParamToUniverse(execution, opCtx, alertStream, param, dataRequestSubscriber);
                    var broker = _brokerServiceFactory.Build(baseSubscriber, param.Factors, param.AggregateNonFactorableIntoOwnCategory);
                    subscriptions.Add(broker);
                }
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a cancelled order rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a cancelled order rule execution with no parameters set");
            }

            return subscriptions;
        }

        private IUniverseCloneableRule SubscribeParamToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IRampingRuleEquitiesParameters param,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Surveillance.Scheduling.Rules.Ramping.GetDescription(),
                    EquityRuleRampingFactory.Version,
                    param.Id,
                    (int)Domain.Surveillance.Scheduling.Rules.Ramping,
                    execution.IsBackTest,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId,
                    execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var rampingRule = _equityRuleRampingFactory.Build(param, ruleCtx, alertStream, runMode, dataRequestSubscriber);

            if (param.HasFilters())
            {
                _logger.LogInformation($"RampingEquitySubscriber parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");
                var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets, param.Funds, param.Strategies);
                filteredUniverse.Subscribe(rampingRule);

                return filteredUniverse;
            }

            return rampingRule;
        }
    }
}
