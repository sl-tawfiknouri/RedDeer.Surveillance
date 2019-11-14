namespace Surveillance.Engine.Rules.Universe.Subscribers.Equity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Scheduling;

    using Infrastructure.Network.Extensions;

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
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;

    /// <summary>
    /// The marking the close equity subscriber.
    /// </summary>
    public class MarkingTheCloseEquitySubscriber : BaseSubscriber, IMarkingTheCloseEquitySubscriber
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
        /// The equity rule marking the close factory.
        /// </summary>
        private readonly IEquityRuleMarkingTheCloseFactory equityRuleMarkingTheCloseFactory;

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
        private readonly ILogger<MarkingTheCloseEquitySubscriber> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkingTheCloseEquitySubscriber"/> class.
        /// </summary>
        /// <param name="equityRuleMarkingTheCloseFactory">
        /// The equity rule marking the close factory.
        /// </param>
        /// <param name="ruleParameterMapper">
        /// The rule parameter mapper.
        /// </param>
        /// <param name="universeFilterFactory">
        /// The universe filter factory.
        /// </param>
        /// <param name="decoratorFilterFactory">
        /// The decorator filter factory.
        /// </param>
        /// <param name="brokerServiceFactory">
        /// The broker service factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public MarkingTheCloseEquitySubscriber(
            IEquityRuleMarkingTheCloseFactory equityRuleMarkingTheCloseFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            ILogger<MarkingTheCloseEquitySubscriber> logger)
        {
            this.equityRuleMarkingTheCloseFactory = 
                equityRuleMarkingTheCloseFactory ?? throw new ArgumentNullException(nameof(equityRuleMarkingTheCloseFactory));
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
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Rules.MarkingTheClose) ?? true)
            {
                return new IUniverseRule[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos = ruleParameters.MarkingTheCloses.Where(
                mtc => filteredParameters.Contains(mtc.Id, StringComparer.InvariantCultureIgnoreCase)).ToList();

            var markingTheCloseParameters = this.ruleParameterMapper.Map(execution, dtos);
            var subscriptions = this.SubscribeToUniverse(
                execution,
                operationContext,
                alertStream,
                dataRequestSubscriber,
                markingTheCloseParameters);

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
        /// <param name="markingTheClose">
        /// The marking the close.
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
            IMarkingTheCloseEquitiesParameters parameter,
            IUniverseRule markingTheClose,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext processOperationRunRuleContext,
            RuleRunMode ruleRunMode)
        {
            if (parameter.HasInternalFilters() || parameter.HasReferenceDataFilters() || parameter.HasMarketCapFilters()
                || parameter.HasVenueVolumeFilters())
            {
                this.logger.LogInformation($"parameters had filters. Inserting filtered universe in {operationContext.Id} OpCtx");

                var filteredUniverse = this.universeFilterFactory.Build(
                    markingTheClose,
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
                    "Marking The Close Equity",
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
                        this.MarkingTheCloseDataSource(parameter),
                        ruleRunMode);
                }

                decoratedFilter.Subscribe(markingTheClose);

                return decoratedFilter;
            }

            return markingTheClose;
        }

        /// <summary>
        /// The marking the close data source.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="DataSource"/>.
        /// </returns>
        private DataSource MarkingTheCloseDataSource(IMarkingTheCloseEquitiesParameters parameters)
        {
            if (parameters == null)
            {
                return DataSource.AnyInterday;
            }

            if (parameters.PercentageThresholdWindowVolume != null)
            {
                return DataSource.AnyIntraday;
            }

            if (parameters.PercentageThresholdDailyVolume != null)
            {
                return DataSource.AnyInterday;
            }

            return this.DataSourceForWindow(parameters.Windows);
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
            IMarkingTheCloseEquitiesParameters parameter,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            var ruleCtx = operationContext.CreateAndStartRuleRunContext(
                Rules.MarkingTheClose.GetDescription(),
                EquityRuleMarkingTheCloseFactory.Version,
                parameter.Id,
                (int)Rules.MarkingTheClose,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var markingTheClose = this.equityRuleMarkingTheCloseFactory.Build(
                parameter,
                ruleCtx,
                alertStream,
                runMode,
                dataRequestSubscriber);
            var markingTheCloseOrgFactors = this.brokerServiceFactory.Build(
                markingTheClose,
                parameter.Factors,
                parameter.AggregateNonFactorableIntoOwnCategory);

            var filteredMarkingTheClose = this.DecorateWithFilters(
                operationContext,
                parameter,
                markingTheCloseOrgFactors,
                dataRequestSubscriber,
                ruleCtx,
                runMode);

            return filteredMarkingTheClose;
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
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="markingTheCloseParameters">
        /// The marking the close parameters.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseRule"/>.
        /// </returns>
        private IReadOnlyCollection<IUniverseRule> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IReadOnlyCollection<IMarkingTheCloseEquitiesParameters> markingTheCloseParameters)
        {
            var subscriptions = new List<IUniverseRule>();

            if (markingTheCloseParameters != null && markingTheCloseParameters.Any())
            {
                foreach (var param in markingTheCloseParameters)
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
                const string ErrorMessage = "tried to schedule a marking the close rule execution with no parameters set";
                this.logger.LogError(ErrorMessage);
                operationContext.EventError(ErrorMessage);
            }

            return subscriptions;
        }
    }
}