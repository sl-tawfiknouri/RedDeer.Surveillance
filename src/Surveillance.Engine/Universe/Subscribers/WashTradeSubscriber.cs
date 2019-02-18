﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Scheduling;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Interfaces;
using Utilities.Extensions;

namespace Surveillance.Engine.Rules.Universe.Subscribers
{
    public class WashTradeSubscriber : IWashTradeSubscriber
    {
        private readonly IWashTradeRuleFactory _washTradeRuleFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IOrganisationalFactorBrokerFactory _brokerFactory;
        private readonly ILogger<MarkingTheCloseSubscriber> _logger;

        public WashTradeSubscriber(
            IWashTradeRuleFactory washTradeRuleFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerFactory brokerFactory,
            ILogger<MarkingTheCloseSubscriber> logger)
        {
            _washTradeRuleFactory = washTradeRuleFactory ?? throw new ArgumentNullException(nameof(washTradeRuleFactory));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _brokerFactory = brokerFactory ?? throw new ArgumentNullException(nameof(brokerFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule).Contains(Domain.Scheduling.Rules.WashTrade) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .WashTrades
                    .Where(wt => filteredParameters.Contains(wt.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var washTradeParameters = _ruleParameterMapper.Map(dtos);
            var subscriptions = SubscribeToUniverse(execution, opCtx, alertStream, washTradeParameters);

            return subscriptions;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IReadOnlyCollection<IWashTradeRuleParameters> washTradeParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (washTradeParameters != null
                && washTradeParameters.Any())
            {
                foreach (var param in washTradeParameters)
                {
                    var paramSubscriptions = SubscribeToParameters(execution, opCtx, alertStream, param);
                    var broker = 
                        _brokerFactory.Build(
                            paramSubscriptions,
                            param.Factors,
                            param.AggregateNonFactorableIntoOwnCategory);

                    subscriptions.Add(broker);
                }
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a wash trade rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a wash trade rule execution with no parameters set");
            }

            return subscriptions;
        }

        private IUniverseCloneableRule SubscribeToParameters(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IWashTradeRuleParameters param)
        {
            var ctx = opCtx.CreateAndStartRuleRunContext(
                Domain.Scheduling.Rules.WashTrade.GetDescription(),
                WashTradeRuleFactory.Version,
                param.Id,
                (int)Domain.Scheduling.Rules.WashTrade,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var washTrade = _washTradeRuleFactory.Build(param, ctx, alertStream, runMode);

            if (param.HasFilters())
            {
                _logger.LogInformation($"WashTradeSubscriber parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");

                var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets);
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
