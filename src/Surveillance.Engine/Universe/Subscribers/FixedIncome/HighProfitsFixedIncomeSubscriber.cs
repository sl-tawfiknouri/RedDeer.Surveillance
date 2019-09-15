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
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome.Interfaces;

    public class HighProfitsFixedIncomeSubscriber : IHighProfitsFixedIncomeSubscriber
    {
        private readonly IOrganisationalFactorBrokerServiceFactory brokerServiceFactory;

        private readonly IFixedIncomeHighProfitFactory fixedIncomeRuleHighProfitsFactory;

        private readonly ILogger<HighVolumeFixedIncomeSubscriber> logger;

        private readonly IRuleParameterToRulesMapperDecorator ruleParameterMapper;

        private readonly IUniverseFilterFactory universeFilterFactory;

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

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Rules.FixedIncomeHighProfits) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
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

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IReadOnlyCollection<IHighProfitsRuleFixedIncomeParameters> highProfitParameters,
            IJudgementService judgementService)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (highProfitParameters != null && highProfitParameters.Any())
            {
                foreach (var param in highProfitParameters)
                {
                    var paramSubscriptions = this.SubscribeToParams(
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

        private IUniverseRule SubscribeToParams(
            ScheduledExecution execution,
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            IHighProfitsRuleFixedIncomeParameters param,
            IJudgementService judgementService)
        {
            var ruleCtx = operationContext.CreateAndStartRuleRunContext(
                Rules.FixedIncomeHighProfits.GetDescription(),
                FixedIncomeHighProfitFactory.Version,
                param.Id,
                (int)Rules.FixedIncomeHighProfits,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;

            var highProfits = 
                this.fixedIncomeRuleHighProfitsFactory.BuildRule(
                    param,
                    ruleCtx,
                    judgementService,
                    universeDataRequestsSubscriber,
                    runMode,
                    execution);

            var highProfitsOrgFactors = this.brokerServiceFactory.Build(
                highProfits,
                param.Factors,
                param.AggregateNonFactorableIntoOwnCategory);

            var highProfitsFiltered = this.DecorateWithFilters(
                operationContext,
                param,
                highProfitsOrgFactors,
                universeDataRequestsSubscriber,
                ruleCtx,
                runMode);

            return highProfitsFiltered;
        }

        private IUniverseRule DecorateWithFilters(
            ISystemProcessOperationContext opCtx,
            IHighProfitsRuleFixedIncomeParameters param,
            IUniverseRule highProfits,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext processOperationRunRuleContext,
            RuleRunMode ruleRunMode)
        {
            if (param.HasInternalFilters())
            {
                this.logger.LogInformation($"parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");

                var filteredUniverse = this.universeFilterFactory.Build(
                    param.Accounts,
                    param.Traders,
                    param.Markets,
                    param.Funds,
                    param.Strategies,
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