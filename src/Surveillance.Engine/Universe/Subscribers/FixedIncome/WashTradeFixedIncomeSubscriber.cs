namespace Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome
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
    using Surveillance.Engine.Rules.Factories.FixedIncome;
    using Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
    using Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome.Interfaces;

    public class WashTradeFixedIncomeSubscriber : IWashTradeFixedIncomeSubscriber
    {
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;

        private readonly IFixedIncomeWashTradeFactory _fixedIncomeRuleWashTradeFactory;

        private readonly ILogger<WashTradeFixedIncomeSubscriber> _logger;

        private readonly IRuleParameterToRulesMapperDecorator _ruleParameterMapper;

        private readonly IUniverseFilterFactory _universeFilterFactory;

        public WashTradeFixedIncomeSubscriber(
            IFixedIncomeWashTradeFactory fixedIncomeRuleWashTradeFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            ILogger<WashTradeFixedIncomeSubscriber> logger)
        {
            this._fixedIncomeRuleWashTradeFactory = fixedIncomeRuleWashTradeFactory
                                                    ?? throw new ArgumentNullException(
                                                        nameof(fixedIncomeRuleWashTradeFactory));
            this._ruleParameterMapper =
                ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            this._universeFilterFactory =
                universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            this._brokerServiceFactory =
                brokerServiceFactory ?? throw new ArgumentNullException(nameof(brokerServiceFactory));
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
            if (!execution.Rules?.Select(ru => ru.Rule).Contains(Rules.FixedIncomeWashTrades) ?? true)
                return new IObserver<IUniverseEvent>[0];

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos = ruleParameters.FixedIncomeWashTrades.Where(
                wt => filteredParameters.Contains(wt.Id, StringComparer.InvariantCultureIgnoreCase)).ToList();

            var fixedIncomeWashTradeParameters = this._ruleParameterMapper.Map(execution, dtos);
            var subscriptions = this.SubscribeToUniverse(
                execution,
                operationContext,
                alertStream,
                dataRequestSubscriber,
                fixedIncomeWashTradeParameters);

            return subscriptions;
        }

        private IUniverseRule DecorateWithFilters(
            ISystemProcessOperationContext opCtx,
            IWashTradeRuleFixedIncomeParameters param,
            IUniverseRule washTrade,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext processOperationRunRuleContext,
            RuleRunMode ruleRunMode)
        {
            if (param.HasInternalFilters())
            {
                this._logger.LogInformation($"parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");

                var filteredUniverse = this._universeFilterFactory.Build(
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
                    "Wash Trade Fixed Income",
                    universeDataRequestsSubscriber,
                    processOperationRunRuleContext);
                filteredUniverse.Subscribe(washTrade);

                return filteredUniverse;
            }

            return washTrade;
        }

        private IUniverseRule SubscribeToParameters(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            IWashTradeRuleFixedIncomeParameters param)
        {
            var ctx = opCtx.CreateAndStartRuleRunContext(
                Rules.FixedIncomeWashTrades.GetDescription(),
                FixedIncomeWashTradeFactory.Version,
                param.Id,
                (int)Rules.FixedIncomeWashTrades,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var washTrade = this._fixedIncomeRuleWashTradeFactory.BuildRule(param, ctx, alertStream, runMode);
            var washTradeOrgFactors = this._brokerServiceFactory.Build(
                washTrade,
                param.Factors,
                param.AggregateNonFactorableIntoOwnCategory);
            var washTradeFilters = this.DecorateWithFilters(
                opCtx,
                param,
                washTradeOrgFactors,
                universeDataRequestsSubscriber,
                ctx,
                runMode);

            return washTradeFilters;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            IReadOnlyCollection<IWashTradeRuleFixedIncomeParameters> washTradeParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (washTradeParameters != null && washTradeParameters.Any())
            {
                foreach (var param in washTradeParameters)
                {
                    var paramSubscriptions = this.SubscribeToParameters(
                        execution,
                        opCtx,
                        alertStream,
                        universeDataRequestsSubscriber,
                        param);
                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                const string errorMessage = "tried to schedule a wash trade rule execution with no parameters set";
                this._logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }
    }
}