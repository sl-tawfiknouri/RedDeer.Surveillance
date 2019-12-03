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
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome.Interfaces;

    /// <summary>
    /// The high profits fixed income subscriber.
    /// </summary>
    public class HighProfitsFixedIncomeSubscriber : IHighProfitsFixedIncomeSubscriber
    {
        /// <summary>
        /// The broker service factory.
        /// </summary>
        private readonly IOrganisationalFactorBrokerServiceFactory brokerServiceFactory;

        /// <summary>
        /// The fixed income rule high profits factory.
        /// </summary>
        private readonly IFixedIncomeHighProfitFactory fixedIncomeRuleHighProfitsFactory;

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
        /// Initializes a new instance of the <see cref="HighProfitsFixedIncomeSubscriber"/> class.
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
        public HighProfitsFixedIncomeSubscriber(
            IFixedIncomeHighProfitFactory fixedIncomeRuleHighVolumeFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            ILogger<HighVolumeFixedIncomeSubscriber> logger)
        {
            this.fixedIncomeRuleHighProfitsFactory = 
                fixedIncomeRuleHighVolumeFactory ?? throw new ArgumentNullException(nameof(fixedIncomeRuleHighVolumeFactory));
            this.ruleParameterMapper =
                ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            this.universeFilterFactory =
                universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            this.brokerServiceFactory =
                brokerServiceFactory ?? throw new ArgumentNullException(nameof(brokerServiceFactory));
            this.logger =
                logger ?? throw new ArgumentNullException(nameof(logger));
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
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Rules.FixedIncomeHighProfits) ?? true)
            {
                return new IUniverseRule[0];
            }

            var filteredParameters =
                execution
                    .Rules
                    .SelectMany(ru => ru.Ids)
                    .Where(ru => ru != null)
                    .ToList();

            var dtos =
                ruleParameters
                    .FixedIncomeHighProfits
                    .Where(hv => 
                        filteredParameters.Contains(hv.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var highProfitParameters = this.ruleParameterMapper.Map(execution, dtos);

            var subscriptions = this.SubscribeToUniverse(
                execution,
                operationContext,
                dataRequestSubscriber,
                highProfitParameters,
                judgementService);

            return subscriptions;
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
        /// <param name="highProfitParameters">
        /// The high profit parameters.
        /// </param>
        /// <param name="judgementService">
        /// The judgement service.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseRule"/>.
        /// </returns>
        private IReadOnlyCollection<IUniverseRule> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IReadOnlyCollection<IHighProfitsRuleFixedIncomeParameters> highProfitParameters,
            IJudgementService judgementService)
        {
            var subscriptions = new List<IUniverseRule>();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (highProfitParameters != null && highProfitParameters.Any())
            {
                foreach (var param in highProfitParameters)
                {
                    var paramSubscriptions = this.SubscribeToParameters(
                        execution,
                        operationContext,
                        dataRequestSubscriber,
                        param,
                        judgementService);
                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                var errorMessage =
                    $"tried to schedule a {nameof(FixedIncomeHighProfitsRule)} rule execution with no parameters set";
                this.logger.LogError(errorMessage);
                operationContext.EventError(errorMessage);
            }

            return subscriptions;
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
        /// <param name="universeDataRequestsSubscriber">
        /// The universe data requests subscriber.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="judgementService">
        /// The judgement service.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseRule"/>.
        /// </returns>
        private IUniverseRule SubscribeToParameters(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            IHighProfitsRuleFixedIncomeParameters parameters,
            IJudgementService judgementService)
        {
            var ruleCtx = operationContext.CreateAndStartRuleRunContext(
                Rules.FixedIncomeHighProfits.GetDescription(),
                FixedIncomeHighProfitFactory.Version,
                parameters.Id,
                (int)Rules.FixedIncomeHighProfits,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;

            var highProfits = 
                this.fixedIncomeRuleHighProfitsFactory.BuildRule(
                    parameters,
                    ruleCtx,
                    judgementService,
                    universeDataRequestsSubscriber,
                    runMode,
                    execution);

            var highProfitsOrgFactors = this.brokerServiceFactory.Build(
                highProfits,
                parameters.Factors,
                parameters.AggregateNonFactorableIntoOwnCategory);

            var highProfitsFiltered = this.DecorateWithFilters(
                operationContext,
                parameters,
                highProfitsOrgFactors,
                universeDataRequestsSubscriber,
                ruleCtx,
                runMode);

            return highProfitsFiltered;
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
        /// <param name="highProfits">
        /// The high profits.
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
            IHighProfitsRuleFixedIncomeParameters parameters,
            IUniverseRule highProfits,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext processOperationRunRuleContext,
            RuleRunMode ruleRunMode)
        {
            if (parameters.HasInternalFilters())
            {
                this.logger.LogInformation($"parameters had filters. Inserting filtered universe in {operationContext.Id} OpCtx");

                var filteredUniverse = this.universeFilterFactory.Build(
                    highProfits,
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
                    "High Profits Fixed Income",
                    universeDataRequestsSubscriber,
                    processOperationRunRuleContext);
                filteredUniverse.Subscribe(highProfits);

                return filteredUniverse;
            }

            return highProfits;
        }
    }
}