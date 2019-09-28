namespace Surveillance.Engine.Rules.Universe.Subscribers.Equity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Scheduling;

    using Infrastructure.Network.Extensions;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

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
    /// The cancelled order equity subscriber.
    /// </summary>
    public class CancelledOrderEquitySubscriber : BaseSubscriber, ICancelledOrderEquitySubscriber
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
        /// The equity rule cancelled order factory.
        /// </summary>
        private readonly IEquityRuleCancelledOrderFactory equityRuleCancelledOrderFactory;

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
        private readonly ILogger<CancelledOrderEquitySubscriber> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelledOrderEquitySubscriber"/> class.
        /// </summary>
        /// <param name="equityRuleCancelledOrderFactory">
        /// The equity rule cancelled order factory.
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
        public CancelledOrderEquitySubscriber(
            IEquityRuleCancelledOrderFactory equityRuleCancelledOrderFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            ILogger<CancelledOrderEquitySubscriber> logger)
        {
            this.equityRuleCancelledOrderFactory = equityRuleCancelledOrderFactory;
            this.ruleParameterMapper = ruleParameterMapper;
            this.universeFilterFactory = universeFilterFactory;
            this.brokerServiceFactory = brokerServiceFactory;
            this.decoratorFilterFactory = decoratorFilterFactory;
            this.logger = logger;
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
        public IReadOnlyCollection<IUniverseRule> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ab => ab.Rule)?.Contains(Rules.CancelledOrders) ?? true)
            {
                return new IUniverseRule[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos = ruleParameters.CancelledOrders.Where(
                co => filteredParameters.Contains(co.Id, StringComparer.InvariantCultureIgnoreCase)).ToList();

            var cancelledOrderParameters = this.ruleParameterMapper.Map(execution, dtos);

            return this.SubscribeToUniverse(
                execution,
                operationContext,
                alertStream,
                dataRequestSubscriber,
                cancelledOrderParameters);
        }

        /// <summary>
        /// The decorate with filter.
        /// </summary>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="cancelledOrderRule">
        /// The cancelled order rule.
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
        private IUniverseRule DecorateWithFilter(
            ISystemProcessOperationContext operationContext,
            ICancelledOrderRuleEquitiesParameters parameter,
            IUniverseRule cancelledOrderRule,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext processOperationRunRuleContext,
            RuleRunMode ruleRunMode)
        {
            if (parameter.HasInternalFilters() 
                || parameter.HasReferenceDataFilters() 
                || parameter.HasMarketCapFilters()
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
                    "Cancelled Order Equity",
                    universeDataRequestsSubscriber,
                    processOperationRunRuleContext);

                var decoratedFilteredUniverse = filteredUniverse;

                if (parameter.HasVenueVolumeFilters())
                {
                    decoratedFilteredUniverse = this.decoratorFilterFactory.Build(
                        parameter.Windows,
                        filteredUniverse,
                        parameter.VenueVolumeFilter,
                        processOperationRunRuleContext,
                        universeDataRequestsSubscriber,
                        this.DataSourceForWindow(parameter.Windows),
                        ruleRunMode);
                }

                decoratedFilteredUniverse.Subscribe(cancelledOrderRule);

                return decoratedFilteredUniverse;
            }

            return cancelledOrderRule;
        }

        /// <summary>
        /// The subscribe parameters to universe.
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
        /// <param name="universeDataRequestsSubscriber">
        /// The universe data requests subscriber.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseRule"/>.
        /// </returns>
        private IUniverseRule SubscribeParameterToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ICancelledOrderRuleEquitiesParameters parameter)
        {
            var ruleCtx = operationContext.CreateAndStartRuleRunContext(
                Rules.CancelledOrders.GetDescription(),
                EquityRuleCancelledOrderFactory.Version,
                parameter.Id,
                (int)Rules.CancelledOrders,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var cancelledOrderRule = this.equityRuleCancelledOrderFactory.Build(parameter, ruleCtx, alertStream, runMode);

            var cancelledOrderOrgFactors = this.brokerServiceFactory.Build(
                cancelledOrderRule,
                parameter.Factors,
                parameter.AggregateNonFactorableIntoOwnCategory);

            var cancelledOrderFiltered = this.DecorateWithFilter(
                operationContext,
                parameter,
                cancelledOrderOrgFactors,
                universeDataRequestsSubscriber,
                ruleCtx,
                runMode);

            return cancelledOrderFiltered;
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
        /// <param name="universeDataRequestsSubscriber">
        /// The universe data requests subscriber.
        /// </param>
        /// <param name="cancelledOrderParameters">
        /// The cancelled order parameters.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        private IReadOnlyCollection<IUniverseRule> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            IReadOnlyCollection<ICancelledOrderRuleEquitiesParameters> cancelledOrderParameters)
        {
            var subscriptions = new List<IUniverseRule>();

            if (cancelledOrderParameters != null && cancelledOrderParameters.Any())
            {
                foreach (var param in cancelledOrderParameters)
                {
                    var baseSubscriber = this.SubscribeParameterToUniverse(
                        execution,
                        operationContext,
                        alertStream,
                        universeDataRequestsSubscriber,
                        param);
                    subscriptions.Add(baseSubscriber);
                }
            }
            else
            {
                const string ErrorMessage = "tried to schedule a cancelled order rule execution with no parameters set";
                this.logger.LogError(ErrorMessage);
                operationContext.EventError(ErrorMessage);
            }

            return subscriptions;
        }
    }
}