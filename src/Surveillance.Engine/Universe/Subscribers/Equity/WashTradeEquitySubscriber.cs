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
    /// The wash trade equity subscriber.
    /// </summary>
    public class WashTradeEquitySubscriber : BaseSubscriber, IWashTradeEquitySubscriber
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
        /// The equity rule wash trade factory.
        /// </summary>
        private readonly IEquityRuleWashTradeFactory equityRuleWashTradeFactory;

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
        private readonly ILogger<WashTradeEquitySubscriber> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WashTradeEquitySubscriber"/> class.
        /// </summary>
        /// <param name="equityRuleWashTradeFactory">
        /// The equity rule wash trade factory.
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
        public WashTradeEquitySubscriber(
            IEquityRuleWashTradeFactory equityRuleWashTradeFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            ILogger<WashTradeEquitySubscriber> logger)
        {
            this.equityRuleWashTradeFactory = equityRuleWashTradeFactory
                                               ?? throw new ArgumentNullException(nameof(equityRuleWashTradeFactory));
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
        public IReadOnlyCollection<IUniverseRule> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule).Contains(Rules.WashTrade) ?? true)
            {
                return new IUniverseRule[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos = ruleParameters.WashTrades
                .Where(wt => filteredParameters.Contains(wt.Id, StringComparer.InvariantCultureIgnoreCase)).ToList();

            var washTradeParameters = this.ruleParameterMapper.Map(execution, dtos);
            var subscriptions = this.SubscribeToUniverse(
                execution,
                operationContext,
                alertStream,
                dataRequestSubscriber,
                washTradeParameters);

            return subscriptions;
        }

        /// <summary>
        /// The decorate with filter.
        /// </summary>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="washTrade">
        /// The wash trade.
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
            IWashTradeRuleEquitiesParameters parameters,
            IUniverseRule washTrade,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext processOperationRunRuleContext,
            RuleRunMode ruleRunMode)
        {
            if (parameters.HasInternalFilters() || parameters.HasReferenceDataFilters() || parameters.HasMarketCapFilters()
                || parameters.HasVenueVolumeFilters())
            {
                this.logger.LogInformation($"parameters had filters. Inserting filtered universe in {operationContext.Id} OpCtx");

                var filteredUniverse = this.universeFilterFactory.Build(
                    washTrade,
                    parameters.Accounts,
                    parameters.Traders,
                    parameters.Markets,
                    parameters.Funds,
                    parameters.Strategies,
                    parameters.Sectors,
                    parameters.Industries,
                    parameters.Regions,
                    parameters.Countries,
                    parameters.MarketCapFilter,
                    ruleRunMode,
                    "Wash Trade Equity",
                    universeDataRequestsSubscriber,
                    processOperationRunRuleContext);

                var decoratedFilter = filteredUniverse;

                if (parameters.HasVenueVolumeFilters())
                {
                    decoratedFilter = this.decoratorFilterFactory.Build(
                        parameters.Windows,
                        filteredUniverse,
                        parameters.VenueVolumeFilter,
                        processOperationRunRuleContext,
                        universeDataRequestsSubscriber,
                        this.DataSourceForWindow(parameters.Windows),
                        ruleRunMode);
                }

                decoratedFilter.Subscribe(washTrade);

                return decoratedFilter;
            }

            return washTrade;
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
        /// <param name="universeDataRequestsSubscriber">
        /// The universe data requests subscriber.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseRule"/>.
        /// </returns>
        private IUniverseRule SubscribeToParameters(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            IWashTradeRuleEquitiesParameters parameters)
        {
            var ctx = operationContext.CreateAndStartRuleRunContext(
                Rules.WashTrade.GetDescription(),
                EquityRuleWashTradeFactory.Version,
                parameters.Id,
                (int)Rules.WashTrade,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var washTrade = this.equityRuleWashTradeFactory.Build(parameters, ctx, alertStream, runMode);
            var washTradeOrgFactors = this.brokerServiceFactory.Build(
                washTrade,
                parameters.Factors,
                parameters.AggregateNonFactorableIntoOwnCategory);

            var washTradeFiltered = this.DecorateWithFilter(
                operationContext,
                parameters,
                washTradeOrgFactors,
                universeDataRequestsSubscriber,
                ctx,
                runMode);

            return washTradeFiltered;
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
        /// <param name="washTradeParameters">
        /// The wash trade parameters.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseRule"/>.
        /// </returns>
        private IReadOnlyCollection<IUniverseRule> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            IReadOnlyCollection<IWashTradeRuleEquitiesParameters> washTradeParameters)
        {
            var subscriptions = new List<IUniverseRule>();

            if (washTradeParameters != null && washTradeParameters.Any())
            {
                foreach (var param in washTradeParameters)
                {
                    var paramSubscriptions = this.SubscribeToParameters(
                        execution,
                        operationContext,
                        alertStream,
                        universeDataRequestsSubscriber,
                        param);
                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                const string ErrorMessage = "tried to schedule a wash trade rule execution with no parameters set";
                this.logger.LogError(ErrorMessage);
                operationContext.EventError(ErrorMessage);
            }

            return subscriptions;
        }
    }
}