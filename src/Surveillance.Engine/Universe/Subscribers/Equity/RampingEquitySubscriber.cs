namespace Surveillance.Engine.Rules.Universe.Subscribers.Equity
{
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
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;

    /// <summary>
    /// The ramping equity subscriber.
    /// </summary>
    public class RampingEquitySubscriber : BaseSubscriber, IRampingEquitySubscriber
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
        /// The equity rule ramping factory.
        /// </summary>
        private readonly IEquityRuleRampingFactory equityRuleRampingFactory;

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
        private readonly ILogger<RampingEquitySubscriber> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RampingEquitySubscriber"/> class.
        /// </summary>
        /// <param name="ruleParameterMapper">
        /// The rule parameter mapper.
        /// </param>
        /// <param name="universeFilterFactory">
        /// The universe filter factory.
        /// </param>
        /// <param name="brokerServiceFactory">
        /// The broker service factory.
        /// </param>
        /// <param name="equityRuleRampingFactory">
        /// The equity rule ramping factory.
        /// </param>
        /// <param name="decoratorFilterFactory">
        /// The decorator filter factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public RampingEquitySubscriber(
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IEquityRuleRampingFactory equityRuleRampingFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            ILogger<RampingEquitySubscriber> logger)
        {
            this.ruleParameterMapper =
                ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            this.universeFilterFactory =
                universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            this.brokerServiceFactory =
                brokerServiceFactory ?? throw new ArgumentNullException(nameof(brokerServiceFactory));
            this.equityRuleRampingFactory = equityRuleRampingFactory
                                             ?? throw new ArgumentNullException(nameof(equityRuleRampingFactory));
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
            if (!execution.Rules?.Select(_ => _.Rule)?.Contains(Rules.Ramping) ?? true)
            {
                return new IUniverseRule[0];
            }

            var filteredParameters = execution.Rules.SelectMany(_ => _.Ids).Where(_ => _ != null).ToList();
            var dtos = ruleParameters.Rampings
                .Where(_ => filteredParameters.Contains(_.Id, StringComparer.OrdinalIgnoreCase)).ToList();

            var rampingParameters = this.ruleParameterMapper.Map(execution, dtos);

            return this.SubscribeToUniverse(execution, operationContext, alertStream, rampingParameters, dataRequestSubscriber);
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
        /// <param name="rampingRule">
        /// The ramping rule.
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
            IRampingRuleEquitiesParameters parameter,
            IUniverseRule rampingRule,
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
                    "Ramping Equity",
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
                        this.DataSourceForWindow(parameter.Windows),
                        ruleRunMode);
                }

                decoratedFilter.Subscribe(rampingRule);

                return decoratedFilter;
            }

            return rampingRule;
        }

        /// <summary>
        /// The subscribe parameter to universe.
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
        private IUniverseRule SubscribeParameterToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IUniverseAlertStream alertStream,
            IRampingRuleEquitiesParameters parameter,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            var ruleCtx = operationContext.CreateAndStartRuleRunContext(
                Rules.Ramping.GetDescription(),
                EquityRuleRampingFactory.Version,
                parameter.Id,
                (int)Rules.Ramping,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var rampingRule = this.equityRuleRampingFactory.Build(
                parameter,
                ruleCtx,
                alertStream,
                runMode,
                dataRequestSubscriber);
            var rampingRuleOrgFactors = this.brokerServiceFactory.Build(
                rampingRule,
                parameter.Factors,
                parameter.AggregateNonFactorableIntoOwnCategory);
            var rampingRuleFiltered = this.DecorateWithFilters(
                operationContext,
                parameter,
                rampingRuleOrgFactors,
                dataRequestSubscriber,
                ruleCtx,
                runMode);

            return rampingRuleFiltered;
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
        /// <param name="rampingParameters">
        /// The ramping parameters.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseRule"/>.
        /// </returns>
        private IReadOnlyCollection<IUniverseRule> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IUniverseAlertStream alertStream,
            IReadOnlyCollection<IRampingRuleEquitiesParameters> rampingParameters,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            var subscriptions = new List<IUniverseRule>();

            if (rampingParameters != null && rampingParameters.Any())
            {
                foreach (var param in rampingParameters)
                {
                    var baseSubscriber = this.SubscribeParameterToUniverse(
                        execution,
                        operationContext,
                        alertStream,
                        param,
                        dataRequestSubscriber);
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