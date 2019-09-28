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
    /// The high volume equity subscriber.
    /// </summary>
    public class HighVolumeEquitySubscriber : BaseSubscriber, IHighVolumeEquitySubscriber
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
        /// The equity rule high volume factory.
        /// </summary>
        private readonly IEquityRuleHighVolumeFactory equityRuleHighVolumeFactory;

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
        private readonly ILogger<HighVolumeEquitySubscriber> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighVolumeEquitySubscriber"/> class.
        /// </summary>
        /// <param name="equityRuleHighVolumeFactory">
        /// The equity rule high volume factory.
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
        public HighVolumeEquitySubscriber(
            IEquityRuleHighVolumeFactory equityRuleHighVolumeFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            ILogger<HighVolumeEquitySubscriber> logger)
        {
            this.equityRuleHighVolumeFactory = equityRuleHighVolumeFactory
                                                ?? throw new ArgumentNullException(nameof(equityRuleHighVolumeFactory));
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
        /// The <see cref="IUniverseRule"/>.
        /// </returns>
        public IReadOnlyCollection<IUniverseRule> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Rules.HighVolume) ?? true)
            {
                return new IUniverseRule[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos = ruleParameters.HighVolumes
                .Where(hv => filteredParameters.Contains(hv.Id, StringComparer.InvariantCultureIgnoreCase)).ToList();

            var highVolumeParameters = this.ruleParameterMapper.Map(execution, dtos);

            var subscriptions = this.SubscribeToUniverse(
                execution,
                operationContext,
                alertStream,
                dataRequestSubscriber,
                highVolumeParameters);

            return subscriptions;
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
        /// <param name="highVolume">
        /// The high volume.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
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
            IHighVolumeRuleEquitiesParameters parameter,
            IUniverseRule highVolume,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
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
                    "High Volume Equity",
                    dataRequestSubscriber,
                    processOperationRunRuleContext);

                var decoratedFilter = filteredUniverse;

                if (parameter.HasVenueVolumeFilters())
                {
                    decoratedFilter = this.decoratorFilterFactory.Build(
                        parameter.Windows,
                        filteredUniverse,
                        parameter.VenueVolumeFilter,
                        processOperationRunRuleContext,
                        dataRequestSubscriber,
                        this.HighVolumeDataSource(parameter),
                        ruleRunMode);
                }

                decoratedFilter.Subscribe(highVolume);

                return decoratedFilter;
            }

            return highVolume;
        }

        /// <summary>
        /// The high volume data source.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="DataSource"/>.
        /// </returns>
        private DataSource HighVolumeDataSource(IHighVolumeRuleEquitiesParameters parameters)
        {
            if (parameters == null)
            {
                return DataSource.AllInterday;
            }

            if (parameters.HighVolumePercentageWindow != null)
            {
                return DataSource.AllIntraday;
            }

            if (parameters.HighVolumePercentageDaily != null)
            {
                return DataSource.AllInterday;
            }

            if (parameters.HighVolumePercentageMarketCap != null)
            {
                return DataSource.AllInterday;
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
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseRule"/>.
        /// </returns>
        private IUniverseRule SubscribeToParameters(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IHighVolumeRuleEquitiesParameters parameter)
        {
            var ruleCtx = operationContext.CreateAndStartRuleRunContext(
                Rules.HighVolume.GetDescription(),
                EquityRuleHighVolumeFactory.Version,
                parameter.Id,
                (int)Rules.HighVolume,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var highVolume = this.equityRuleHighVolumeFactory.Build(
                parameter,
                ruleCtx,
                alertStream,
                dataRequestSubscriber,
                runMode);

            var highVolumeOrgFactors = this.brokerServiceFactory.Build(
                highVolume,
                parameter.Factors,
                parameter.AggregateNonFactorableIntoOwnCategory);

            var decoratedHighVolumeRule = this.DecorateWithFilter(
                operationContext,
                parameter,
                highVolumeOrgFactors,
                dataRequestSubscriber,
                ruleCtx,
                runMode);

            return decoratedHighVolumeRule;
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
        /// <param name="highVolumeParameters">
        /// The high volume parameters.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseRule"/>.
        /// </returns>
        private IReadOnlyCollection<IUniverseRule> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IReadOnlyCollection<IHighVolumeRuleEquitiesParameters> highVolumeParameters)
        {
            var subscriptions = new List<IUniverseRule>();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (highVolumeParameters != null && highVolumeParameters.Any())
            {
                foreach (var param in highVolumeParameters)
                {
                    var paramSubscriptions = this.SubscribeToParameters(
                        execution,
                        operationContext,
                        alertStream,
                        dataRequestSubscriber,
                        param);
                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                const string ErrorMessage = "tried to schedule a high volume rule execution with no parameters set";
                this.logger.LogError(ErrorMessage);
                operationContext.EventError(ErrorMessage);
            }

            return subscriptions;
        }
    }
}