using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Surveillance.Scheduling;
using Infrastructure.Network.Extensions;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories.FixedIncome;
using Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome
{
    public class WashTradeFixedIncomeSubscriber : IWashTradeFixedIncomeSubscriber
    {
        private readonly IFixedIncomeWashTradeFactory _fixedIncomeRuleWashTradeFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;
        private readonly ILogger<WashTradeFixedIncomeSubscriber> _logger;

        public WashTradeFixedIncomeSubscriber(
            IFixedIncomeWashTradeFactory fixedIncomeRuleWashTradeFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            ILogger<WashTradeFixedIncomeSubscriber> logger)
        {
            _fixedIncomeRuleWashTradeFactory = fixedIncomeRuleWashTradeFactory ?? throw new ArgumentNullException(nameof(fixedIncomeRuleWashTradeFactory));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _brokerServiceFactory = brokerServiceFactory ?? throw new ArgumentNullException(nameof(brokerServiceFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule).Contains(Domain.Surveillance.Scheduling.Rules.FixedIncomeWashTrades) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .FixedIncomeWashTrades
                    .Where(wt => filteredParameters.Contains(wt.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var fixedIncomeWashTradeParameters = _ruleParameterMapper.Map(execution, dtos);
            var subscriptions = SubscribeToUniverse(execution, opCtx, alertStream, fixedIncomeWashTradeParameters);

            return subscriptions;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IReadOnlyCollection<IWashTradeRuleFixedIncomeParameters> washTradeParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (washTradeParameters != null
                && washTradeParameters.Any())
            {
                foreach (var param in washTradeParameters)
                {
                    var paramSubscriptions = SubscribeToParameters(execution, opCtx, alertStream, param);
                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                const string errorMessage = "tried to schedule a wash trade rule execution with no parameters set";
                _logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }

        private IUniverseRule SubscribeToParameters(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IWashTradeRuleFixedIncomeParameters param)
        {
            var ctx = opCtx.CreateAndStartRuleRunContext(
                Domain.Surveillance.Scheduling.Rules.FixedIncomeWashTrades.GetDescription(),
                FixedIncomeWashTradeFactory.Version,
                param.Id,
                (int)Domain.Surveillance.Scheduling.Rules.FixedIncomeWashTrades,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var washTrade = _fixedIncomeRuleWashTradeFactory.BuildRule(param, ctx, alertStream, runMode);
            var washTradeOrgFactors =
                _brokerServiceFactory.Build(
                    washTrade,
                    param.Factors,
                    param.AggregateNonFactorableIntoOwnCategory);
            var washTradeFilters = DecorateWithFilters(opCtx, param, washTradeOrgFactors);

            return washTradeFilters;
        }

        private IUniverseRule DecorateWithFilters(
            ISystemProcessOperationContext opCtx,
            IWashTradeRuleFixedIncomeParameters param,
            IUniverseRule washTrade)
        {
            if (param.HasFilters())
            {
                _logger.LogInformation($"parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");

                var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets, param.Funds, param.Strategies);
                filteredUniverse.Subscribe(washTrade);

                return filteredUniverse;
            }
            else
            {
                return washTrade;
            }
        }
    }
}
