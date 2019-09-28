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
    using Surveillance.Data.Universe.Interfaces;
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
    using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;

    /// <summary>
    /// The placing orders with no intent to execute equity subscriber.
    /// </summary>
    public class PlacingOrdersWithNoIntentToExecuteEquitySubscriber : BaseSubscriber, IPlacingOrdersWithNoIntentToExecuteEquitySubscriber
    {
        /// <summary>
        /// The broker service factory.
        /// </summary>
        private readonly IOrganisationalFactorBrokerServiceFactory brokerServiceFactory;

        /// <summary>
        /// The decorator filter factory.
        /// </summary>
        private readonly IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory;

        /// <summary>
        /// The equity rule placing orders factory.
        /// </summary>
        private readonly IEquityRulePlacingOrdersWithoutIntentToExecuteFactory equityRulePlacingOrdersFactory;

        /// <summary>
        /// The rule parameter mapper.
        /// </summary>
        private readonly IRuleParameterToRulesMapperDecorator ruleParameterMapper;

        /// <summary>
        /// The universe filter factory.
        /// </summary>
        private readonly IUniverseFilterFactory universeFilterFactory;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<PlacingOrdersWithNoIntentToExecuteEquitySubscriber> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlacingOrdersWithNoIntentToExecuteEquitySubscriber"/> class.
        /// </summary>
        /// <param name="equityRulePlacingOrdersFactory">
        /// The equity rule placing orders factory.
        /// </param>
        /// <param name="ruleParameterMapper">
        /// The rule parameter mapper.
        /// </param>
        /// <param name="universeFilterFactory">
        /// The universe filter factory.
        /// </param>
        /// <param name="brokerServiceFactory">
        /// The broker service factory.
        /// </param>
        /// <param name="decoratorFilterFactory">
        /// The decorator filter factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public PlacingOrdersWithNoIntentToExecuteEquitySubscriber(
            IEquityRulePlacingOrdersWithoutIntentToExecuteFactory equityRulePlacingOrdersFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            ILogger<PlacingOrdersWithNoIntentToExecuteEquitySubscriber> logger)
        {
            this.equityRulePlacingOrdersFactory = 
                equityRulePlacingOrdersFactory ?? throw new ArgumentNullException(nameof(equityRulePlacingOrdersFactory));
            this.ruleParameterMapper =
                ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            this.universeFilterFactory =
                universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            this.brokerServiceFactory =
                brokerServiceFactory ?? throw new ArgumentNullException(nameof(brokerServiceFactory));
            this.decoratorFilterFactory =
                decoratorFilterFactory ?? throw new ArgumentNullException(nameof(decoratorFilterFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The collate subscriptions.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="ruleParameters">
        /// The rule parameters.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="judgementService">
        /// The judgement service.
        /// </param>
        /// <param name="alertStream">
        /// The alert stream.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(_ => _.Rule)?.Contains(Rules.PlacingOrderWithNoIntentToExecute) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(_ => _.Ids).Where(_ => _ != null).ToList();

            var dtos = ruleParameters.PlacingOrders
                .Where(_ => filteredParameters.Contains(_.Id, StringComparer.InvariantCultureIgnoreCase)).ToList();

            var placingOrderParameters = this.ruleParameterMapper.Map(execution, dtos);
            var subscriptions = this.SubscribeToUniverse(
                execution,
                operationContext,
                alertStream,
                placingOrderParameters,
                dataRequestSubscriber);

            return subscriptions;
        }

        /// <summary>
        /// The decorate with filters.
        /// </summary>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="placingOrders">
        /// The placing orders.
        /// </param>
        /// <param name="universeDataRequestsSubscriber">
        /// The universe data requests subscriber.
        /// </param>
        /// <param name="processOperationRunRuleContext">
        /// The process operation run rule context.
        /// </param>
        /// <param name="ruleRunMode">
        /// The rule run mode.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseRule"/>.
        /// </returns>
        private IUniverseRule DecorateWithFilters(
            ISystemProcessOperationContext operationContext,
            IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters parameter,
            IUniverseRule placingOrders,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext processOperationRunRuleContext,
            RuleRunMode ruleRunMode)
        {
            if (parameter.HasInternalFilters() || parameter.HasReferenceDataFilters() || parameter.HasMarketCapFilters()
                || parameter.HasVenueVolumeFilters())
            {
                this.logger.LogInformation($"parameters had filters. Inserting filtered universe in {operationContext.Id} OpCtx");

                var filteredUniverse = this.universeFilterFactory.Build(
                    parameter.Accounts,
                    parameter.Traders,
                    parameter.Markets,
                    parameter.Funds,
                    parameter.Strategies,
                    parameter.Sectors,
                    parameter.Industries,
                    parameter.Regions,
                    parameter.Countries,
                    parameter.MarketCapFilter,
                    ruleRunMode,
                    "Placing Orders Equity",
                    universeDataRequestsSubscriber,
                    processOperationRunRuleContext);

                var decoratedFilter = filteredUniverse;

                if (parameter.HasVenueVolumeFilters())
                {
                    decoratedFilter = this.decoratorFilterFactory.Build(
                        parameter.Windows,
                        filteredUniverse,
                        parameter.VenueVolumeFilter,
                        processOperationRunRuleContext,
                        universeDataRequestsSubscriber,
                        DataSource.AllIntraday,
                        ruleRunMode);
                }

                decoratedFilter.Subscribe(placingOrders);

                return decoratedFilter;
            }

            return placingOrders;
        }

        /// <summary>
        /// The subscribe to parameters.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="alertStream">
        /// The alert stream.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseRule"/>.
        /// </returns>
        private IUniverseRule SubscribeToParameters(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IUniverseAlertStream alertStream,
            IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters parameter,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            var ruleCtx = operationContext.CreateAndStartRuleRunContext(
                Rules.PlacingOrderWithNoIntentToExecute.GetDescription(),
                EquityRulePlacingOrdersWithoutIntentToExecuteFactory.Version,
                parameter.Id,
                (int)Rules.PlacingOrderWithNoIntentToExecute,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var placingOrders = this.equityRulePlacingOrdersFactory.Build(
                parameter,
                alertStream,
                ruleCtx,
                dataRequestSubscriber,
                runMode);
            var placingOrdersOrgFactors = this.brokerServiceFactory.Build(
                placingOrders,
                parameter.Factors,
                parameter.AggregateNonFactorableIntoOwnCategory);
            var filteredPlacingOrders = this.DecorateWithFilters(
                operationContext,
                parameter,
                placingOrdersOrgFactors,
                dataRequestSubscriber,
                ruleCtx,
                runMode);

            return filteredPlacingOrders;
        }

        /// <summary>
        /// The subscribe to universe.
        /// </summary>
        /// <param name="execution">
        /// The execution.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="alertStream">
        /// The alert stream.
        /// </param>
        /// <param name="placingOrdersParameters">
        /// The placing orders parameters.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
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
                        operationContext,
                        alertStream,
                        param,
                        dataRequestSubscriber);
                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                const string ErrorMessage = "tried to schedule a placing orders with no intent to execute rule execution with no parameters set";
                this.logger.LogError(ErrorMessage);
                operationContext.EventError(ErrorMessage);
            }

            return subscriptions;
        }
    }
}