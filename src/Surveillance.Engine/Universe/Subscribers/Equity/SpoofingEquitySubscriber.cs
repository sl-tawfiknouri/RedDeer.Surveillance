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
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;
using Utilities.Extensions;

namespace Surveillance.Engine.Rules.Universe.Subscribers.Equity
{
    public class SpoofingEquitySubscriber : ISpoofingEquitySubscriber
    {
        private readonly IEquityRuleSpoofingFactory _equityRuleSpoofingFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IOrganisationalFactorBrokerFactory _brokerFactory;
        private readonly ILogger _logger;

        public SpoofingEquitySubscriber(
            IEquityRuleSpoofingFactory equityRuleSpoofingFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerFactory brokerFactory,
            ILogger<UniverseRuleSubscriber> logger)
        {
            _equityRuleSpoofingFactory = equityRuleSpoofingFactory ?? throw new ArgumentNullException(nameof(equityRuleSpoofingFactory));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _brokerFactory = brokerFactory ?? throw new ArgumentNullException(nameof(brokerFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ab => ab.Rule)?.ToList().Contains(Domain.Scheduling.Rules.Spoofing)
                ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
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
            var subscriptionRequests = SubscribeToUniverse(execution, opCtx, alertStream, spoofingParameters);

            return subscriptionRequests;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IReadOnlyCollection<ISpoofingRuleEquitiesParameters> spoofingParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (spoofingParameters != null
                && spoofingParameters.Any())
            {
                foreach (var param in spoofingParameters)
                {
                    var paramSubscriptions = SubscribeForParams(execution, opCtx, alertStream, param);
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
                _logger.LogError("Spoofing Rule Scheduler - tried to schedule a spoofing rule execution with no parameters set");
                opCtx.EventError("Spoofing Scheduler - tried to schedule a spoofing rule execution with no parameters set");
            }

            return subscriptions;
        }

        private IUniverseCloneableRule SubscribeForParams(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            ISpoofingRuleEquitiesParameters param)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Scheduling.Rules.Spoofing.GetDescription(),
                    EquityRuleSpoofingFactory.Version,
                    param.Id,
                    (int)Domain.Scheduling.Rules.Spoofing,
                    execution.IsBackTest,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId,
                    execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var spoofingRule = _equityRuleSpoofingFactory.Build(param, ruleCtx, alertStream, runMode);

            if (param.HasFilters())
            {
                _logger.LogInformation($"SpoofingSubscriber parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");

                var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets, param.Funds, param.Strategies);
                filteredUniverse.Subscribe(spoofingRule);

                return filteredUniverse;
            }

            return spoofingRule;
        }
    }
}