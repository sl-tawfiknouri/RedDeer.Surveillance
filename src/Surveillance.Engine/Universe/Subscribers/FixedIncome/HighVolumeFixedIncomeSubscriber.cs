namespace Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome
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
    using Surveillance.Engine.Rules.Factories.FixedIncome;
    using Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome.Interfaces;

    /// <summary>
    /// The high volume fixed income subscriber.
    /// </summary>
    public class HighVolumeFixedIncomeSubscriber : IHighVolumeFixedIncomeSubscriber
    {
        /// <summary>
        /// The broker service factory.
        /// </summary>
        private readonly IOrganisationalFactorBrokerServiceFactory brokerServiceFactory;

        /// <summary>
        /// The fixed income rule high volume factory.
        /// </summary>
        private readonly IFixedIncomeHighVolumeFactory fixedIncomeRuleHighVolumeFactory;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<HighVolumeFixedIncomeSubscriber> logger;

        /// <summary>
        /// The rule parameter mapper.
        /// </summary>
        private readonly IRuleParameterToRulesMapperDecorator ruleParameterMapper;

        /// <summary>
        /// The universe filter factory.
        /// </summary>
        private readonly IUniverseFilterFactory universeFilterFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighVolumeFixedIncomeSubscriber"/> class.
        /// </summary>
        /// <param name="fixedIncomeRuleHighVolumeFactory">
        /// The fixed income rule high volume factory.
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
        /// <param name="logger">
        /// The logger.
        /// </param>
        public HighVolumeFixedIncomeSubscriber(
            IFixedIncomeHighVolumeFactory fixedIncomeRuleHighVolumeFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            ILogger<HighVolumeFixedIncomeSubscriber> logger)
        {
            this.fixedIncomeRuleHighVolumeFactory = 
                fixedIncomeRuleHighVolumeFactory ?? throw new ArgumentNullException(nameof(fixedIncomeRuleHighVolumeFactory));
            this.ruleParameterMapper =
                ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            this.universeFilterFactory =
                universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            this.brokerServiceFactory =
                brokerServiceFactory ?? throw new ArgumentNullException(nameof(brokerServiceFactory));
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
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Rules.FixedIncomeHighVolumeIssuance) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos = ruleParameters.FixedIncomeHighVolumeIssuance.Where(
                hv => filteredParameters.Contains(hv.Id, StringComparer.InvariantCultureIgnoreCase)).ToList();

            var highVolumeParameters = this.ruleParameterMapper.Map(execution, dtos);

            var subscriptions = this.SubscribeToUniverse(
                execution,
                operationContext,
                dataRequestSubscriber,
                judgementService,
                highVolumeParameters);

            return subscriptions;
        }

        /// <summary>
        /// The decorate with filters.
        /// </summary>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="highVolume">
        /// The high volume.
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
            IHighVolumeIssuanceRuleFixedIncomeParameters parameters,
            IUniverseRule highVolume,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext processOperationRunRuleContext,
            RuleRunMode ruleRunMode)
        {
            if (parameters.HasInternalFilters())
            {
                this.logger.LogInformation($"parameters had filters. Inserting filtered universe in {operationContext.Id} operation context");

                var filteredUniverse = this.universeFilterFactory.Build(
                    parameters.Accounts,
                    parameters.Traders,
                    parameters.Markets,
                    parameters.Funds,
                    parameters.Strategies,
                    null,
                    null,
                    null,
                    null,
                    null,
                    ruleRunMode,
                    "High Volume Fixed Income",
                    universeDataRequestsSubscriber,
                    processOperationRunRuleContext);

                filteredUniverse.Subscribe(highVolume);

                return filteredUniverse;
            }

            return highVolume;
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
        /// <param name="judgementService">
        /// The judgement Service.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
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
            IJudgementService judgementService,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IHighVolumeIssuanceRuleFixedIncomeParameters parameters)
        {
            var ruleContext = operationContext.CreateAndStartRuleRunContext(
                Rules.FixedIncomeHighVolumeIssuance.GetDescription(),
                FixedIncomeHighVolumeFactory.Version,
                parameters.Id,
                (int)Rules.FixedIncomeHighVolumeIssuance,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var highVolume = this.fixedIncomeRuleHighVolumeFactory.BuildRule(
                parameters, 
                ruleContext,
                judgementService,
                dataRequestSubscriber,
                runMode);
            var highVolumeOrgFactors = this.brokerServiceFactory.Build(
                highVolume,
                parameters.Factors,
                parameters.AggregateNonFactorableIntoOwnCategory);
            var highVolumeFilters = this.DecorateWithFilters(
                operationContext,
                parameters,
                highVolumeOrgFactors,
                dataRequestSubscriber,
                ruleContext,
                runMode);

            return highVolumeFilters;
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
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
        /// </param>
        /// <param name="judgementService">
        /// The judgement service.
        /// </param>
        /// <param name="highVolumeParameters">
        /// The high volume parameters.
        /// </param>
        /// <returns>
        /// The <see cref="IHighVolumeIssuanceRuleFixedIncomeParameters"/>.
        /// </returns>
        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IReadOnlyCollection<IHighVolumeIssuanceRuleFixedIncomeParameters> highVolumeParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (highVolumeParameters != null && highVolumeParameters.Any())
            {
                foreach (var param in highVolumeParameters)
                {
                    var paramSubscriptions = this.SubscribeToParameters(
                        execution,
                        operationContext,
                        judgementService,
                        dataRequestSubscriber,
                        param);

                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                var errorMessage =
                    $"tried to schedule a {nameof(FixedIncomeHighVolumeRule)} rule execution with no parameters set";
                this.logger.LogError(errorMessage);
                operationContext.EventError(errorMessage);
            }

            return subscriptions;
        }
    }
}