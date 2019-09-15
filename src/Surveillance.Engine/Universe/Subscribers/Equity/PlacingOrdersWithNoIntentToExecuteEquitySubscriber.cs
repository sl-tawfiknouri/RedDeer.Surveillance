namespace Surveillance.Engine.Rules.Universe.Subscribers.Equity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Extensions;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Equities;
    using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;

    public class PlacingOrdersWithNoIntentToExecuteEquitySubscriber : BaseSubscriber,
                                                                      IPlacingOrdersWithNoIntentToExecuteEquitySubscriber
    {
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;

        private readonly IHighVolumeVenueDecoratorFilterFactory _decoratorFilterFactory;

        private readonly IEquityRulePlacingOrdersWithoutIntentToExecuteFactory _equityRulePlacingOrdersFactory;

        private readonly ILogger<PlacingOrdersWithNoIntentToExecuteEquitySubscriber> _logger;

        private readonly IRuleParameterToRulesMapperDecorator _ruleParameterMapper;

        private readonly IUniverseFilterFactory _universeFilterFactory;

        public PlacingOrdersWithNoIntentToExecuteEquitySubscriber(
            IEquityRulePlacingOrdersWithoutIntentToExecuteFactory equityRulePlacingOrdersFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            ILogger<PlacingOrdersWithNoIntentToExecuteEquitySubscriber> logger)
        {
            this._equityRulePlacingOrdersFactory = equityRulePlacingOrdersFactory
                                                   ?? throw new ArgumentNullException(
                                                       nameof(equityRulePlacingOrdersFactory));
            this._ruleParameterMapper =
                ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            this._universeFilterFactory =
                universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            this._brokerServiceFactory =
                brokerServiceFactory ?? throw new ArgumentNullException(nameof(brokerServiceFactory));
            this._decoratorFilterFactory =
                decoratorFilterFactory ?? throw new ArgumentNullException(nameof(decoratorFilterFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(_ => _.Rule)?.Contains(Rules.PlacingOrderWithNoIntentToExecute) ?? true)
                return new IObserver<IUniverseEvent>[0];

            var filteredParameters = execution.Rules.SelectMany(_ => _.Ids).Where(_ => _ != null).ToList();

            var dtos = ruleParameters.PlacingOrders
                .Where(_ => filteredParameters.Contains(_.Id, StringComparer.InvariantCultureIgnoreCase)).ToList();

            var placingOrderParameters = this._ruleParameterMapper.Map(execution, dtos);
            var subscriptions = this.SubscribeToUniverse(
                execution,
                operationContext,
                alertStream,
                placingOrderParameters,
                dataRequestSubscriber);

            return subscriptions;
        }

        private IUniverseRule DecorateWithFilters(
            ISystemProcessOperationContext opCtx,
            IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters param,
            IUniverseRule placingOrders,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext processOperationRunRuleContext,
            RuleRunMode ruleRunMode)
        {
            if (param.HasInternalFilters() || param.HasReferenceDataFilters() || param.HasMarketCapFilters()
                || param.HasVenueVolumeFilters())
            {
                this._logger.LogInformation($"parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");

                var filteredUniverse = this._universeFilterFactory.Build(
                    param.Accounts,
                    param.Traders,
                    param.Markets,
                    param.Funds,
                    param.Strategies,
                    param.Sectors,
                    param.Industries,
                    param.Regions,
                    param.Countries,
                    param.MarketCapFilter,
                    ruleRunMode,
                    "Placing Orders Equity",
                    universeDataRequestsSubscriber,
                    processOperationRunRuleContext);

                var decoratedFilter = filteredUniverse;

                if (param.HasVenueVolumeFilters())
                    decoratedFilter = this._decoratorFilterFactory.Build(
                        param.Windows,
                        filteredUniverse,
                        param.VenueVolumeFilter,
                        processOperationRunRuleContext,
                        universeDataRequestsSubscriber,
                        DataSource.AllIntraday,
                        ruleRunMode);

                decoratedFilter.Subscribe(placingOrders);

                return decoratedFilter;
            }

            return placingOrders;
        }

        private IUniverseRule SubscribeToParameters(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters param,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            var ruleCtx = opCtx.CreateAndStartRuleRunContext(
                Rules.PlacingOrderWithNoIntentToExecute.GetDescription(),
                EquityRulePlacingOrdersWithoutIntentToExecuteFactory.Version,
                param.Id,
                (int)Rules.PlacingOrderWithNoIntentToExecute,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var placingOrders = this._equityRulePlacingOrdersFactory.Build(
                param,
                alertStream,
                ruleCtx,
                dataRequestSubscriber,
                runMode);
            var placingOrdersOrgFactors = this._brokerServiceFactory.Build(
                placingOrders,
                param.Factors,
                param.AggregateNonFactorableIntoOwnCategory);
            var filteredPlacingOrders = this.DecorateWithFilters(
                opCtx,
                param,
                placingOrdersOrgFactors,
                dataRequestSubscriber,
                ruleCtx,
                runMode);

            return filteredPlacingOrders;
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
            if (placingOrdersParameters != null && placingOrdersParameters.Any())
            {
                foreach (var param in placingOrdersParameters)
                {
                    var paramSubscriptions = this.SubscribeToParameters(
                        execution,
                        opCtx,
                        alertStream,
                        param,
                        dataRequestSubscriber);
                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                const string errorMessage =
                    "tried to schedule a placing orders with no intent to execute rule execution with no parameters set";
                this._logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }
    }
}