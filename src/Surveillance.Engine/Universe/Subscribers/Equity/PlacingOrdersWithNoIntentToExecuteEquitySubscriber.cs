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
    public class PlacingOrdersWithNoIntentToExecuteEquitySubscriber : IPlacingOrdersWithNoIntentToExecuteEquitySubscriber
    {
        private readonly IEquityRulePlacingOrdersWithoutIntentToExecuteFactory _equityRulePlacingOrdersFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;
        private readonly ILogger<PlacingOrdersWithNoIntentToExecuteEquitySubscriber> _logger;
        
        public PlacingOrdersWithNoIntentToExecuteEquitySubscriber(
            IEquityRulePlacingOrdersWithoutIntentToExecuteFactory equityRulePlacingOrdersFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            ILogger<PlacingOrdersWithNoIntentToExecuteEquitySubscriber> logger)
        {
            _equityRulePlacingOrdersFactory = equityRulePlacingOrdersFactory ?? throw new ArgumentNullException(nameof(equityRulePlacingOrdersFactory));
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
            if (!execution.Rules?.Select(_ => _.Rule)?.Contains(Domain.Surveillance.Scheduling.Rules.PlacingOrderWithNoIntentToExecute) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters =
                execution
                    .Rules
                    .SelectMany(_ => _.Ids)
                    .Where(_ => _ != null)
                    .ToList();

            var dtos =
                ruleParameters
                    .PlacingOrders
                    .Where(_ => filteredParameters.Contains(_.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var placingOrderParameters = _ruleParameterMapper.Map(dtos);
            var subscriptions = SubscribeToUniverse(execution, opCtx, alertStream, placingOrderParameters, dataRequestSubscriber);

            return subscriptions;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IReadOnlyCollection<IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters> placingOrdersParameters,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (placingOrdersParameters != null
                && placingOrdersParameters.Any())
            {
                foreach (var param in placingOrdersParameters)
                {
                    var paramSubscriptions = SubscribeToParameters(execution, opCtx, alertStream, param, dataRequestSubscriber);
                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                const string errorMessage = "tried to schedule a placing orders with no intent to execute rule execution with no parameters set";
                _logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }

        private IUniverseRule SubscribeToParameters(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters param,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Surveillance.Scheduling.Rules.PlacingOrderWithNoIntentToExecute.GetDescription(),
                    EquityRulePlacingOrdersWithoutIntentToExecuteFactory.Version,
                    param.Id,
                    (int)Domain.Surveillance.Scheduling.Rules.PlacingOrderWithNoIntentToExecute,
                    execution.IsBackTest,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId,
                    execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var placingOrders = _equityRulePlacingOrdersFactory.Build(param, alertStream, ruleCtx, dataRequestSubscriber, runMode);
            var placingOrdersOrgFactors =
                _brokerServiceFactory.Build(
                    placingOrders,
                    param.Factors,
                    param.AggregateNonFactorableIntoOwnCategory);
            var filteredPlacingOrders = DecorateWithFilters(opCtx, param, placingOrdersOrgFactors);

            return filteredPlacingOrders;
        }

        private IUniverseRule DecorateWithFilters(
            ISystemProcessOperationContext opCtx,
            IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters param,
            IUniverseRule placingOrders)
        {
            if (param.HasInternalFilters() || param.HasReferenceDataFilters())
            {
                _logger.LogInformation($"parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");

                var filteredUniverse = _universeFilterFactory.Build(
                    param.Accounts,
                    param.Traders,
                    param.Markets,
                    param.Funds,
                    param.Strategies,
                    param.Sectors,
                    param.Industries,
                    param.Regions,
                    param.Countries);
                filteredUniverse.Subscribe(placingOrders);

                return filteredUniverse;
            }
            else
            {
                return placingOrders;
            }
        }
    }
}
