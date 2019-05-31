using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Surveillance.Scheduling;
using Infrastructure.Network.Extensions;
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
    public class LayeringEquitySubscriber : ILayeringEquitySubscriber
    {
        private readonly IEquityRuleLayeringFactory _equityRuleLayeringFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;
        private readonly ILogger<LayeringEquitySubscriber> _logger;

        public LayeringEquitySubscriber(
            IEquityRuleLayeringFactory equityRuleLayeringFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            ILogger<LayeringEquitySubscriber> logger)
        {
            _equityRuleLayeringFactory = equityRuleLayeringFactory ?? throw new ArgumentNullException(nameof(equityRuleLayeringFactory));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _brokerServiceFactory = brokerServiceFactory ?? throw new ArgumentNullException(nameof(brokerServiceFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Domain.Surveillance.Scheduling.Rules.Layering) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .Layerings
                    .Where(la => filteredParameters.Contains(la.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var layeringParameters = _ruleParameterMapper.Map(dtos);
            var subscriptions = SubscribeToUniverse(execution, opCtx, alertStream, layeringParameters);

            return subscriptions;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IReadOnlyCollection<ILayeringRuleEquitiesParameters> layeringParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (layeringParameters != null
                && layeringParameters.Any())
            {
                foreach (var param in layeringParameters)
                {
                    var paramSubscriptions = SubscribeToParameters(execution, opCtx, alertStream, param);
                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                const string errorMessage = "tried to schedule a layering rule execution with no parameters set";
                _logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }

        private IUniverseRule SubscribeToParameters(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            ILayeringRuleEquitiesParameters param)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Surveillance.Scheduling.Rules.Layering.GetDescription(),
                    EquityRuleLayeringFactory.Version,
                    param.Id,
                    (int)Domain.Surveillance.Scheduling.Rules.Layering,
                    execution.IsBackTest,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId,
                    execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var layeringRule = _equityRuleLayeringFactory.Build(param, ruleCtx, alertStream, runMode);

            var layeringRuleOrgFactors =
                _brokerServiceFactory.Build(
                    layeringRule,
                    param.Factors,
                    param.AggregateNonFactorableIntoOwnCategory);

            var layeringRuleFiltered = DecorateWithFilter(opCtx, param, layeringRuleOrgFactors);

            return layeringRuleFiltered;
        }

        private IUniverseRule DecorateWithFilter(
            ISystemProcessOperationContext opCtx,
            ILayeringRuleEquitiesParameters param,
            IUniverseRule layering)
        {
            if (param.HasFilters())
            {
                _logger.LogInformation($"parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");

                var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets, param.Funds, param.Strategies);
                filteredUniverse.Subscribe(layering);

                return filteredUniverse;
            }
            else
            {
                return layering;
            }
        }

    }
}
