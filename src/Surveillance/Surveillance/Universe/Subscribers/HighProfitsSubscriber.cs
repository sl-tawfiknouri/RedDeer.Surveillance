﻿using System;
using System.Linq;
using Domain.Scheduling;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.Subscribers.Interfaces;
using Utilities.Extensions;

namespace Surveillance.Universe.Subscribers
{
    public class HighProfitsSubscriber : IHighProfitsSubscriber
    {
        private readonly IHighProfitRuleFactory _highProfitRuleFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly ILogger _logger;

        public HighProfitsSubscriber(
            IHighProfitRuleFactory highProfitRuleFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            ILogger<UniverseRuleSubscriber> logger)
        {
            _highProfitRuleFactory = highProfitRuleFactory ?? throw new ArgumentNullException(nameof(highProfitRuleFactory));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void HighProfitsRule(
            ScheduledExecution execution,
            IUniversePlayer player,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Domain.Scheduling.Rules.HighProfits) ?? true)
            {
                return;
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .HighProfits
                    .Where(hp => filteredParameters.Contains(hp.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var highProfitParameters = _ruleParameterMapper.Map(dtos);

            SubscribeToUniverse(execution, player, opCtx, alertStream, highProfitParameters);
        }

        private void SubscribeToUniverse(
            ScheduledExecution execution,
            IUniversePlayer player,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            global::System.Collections.Generic.IReadOnlyCollection<IHighProfitsRuleParameters> highProfitParameters)
        {
            if (highProfitParameters != null
                && highProfitParameters.Any())
            {
                foreach (var param in highProfitParameters)
                {
                    SubscribeParameters(execution, player, opCtx, alertStream, param);
                }
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a high profit rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a high profit rule execution with no parameters set");
            }
        }

        private void SubscribeParameters(
            ScheduledExecution execution,
            IUniversePlayer player, 
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IHighProfitsRuleParameters param)
        {
            var ruleCtxStream = opCtx
                    .CreateAndStartRuleRunContext(
                        Domain.Scheduling.Rules.HighProfits.GetDescription(),
                        HighProfitRuleFactory.Version,
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime,
                        execution.CorrelationId);

            var ruleCtxMarketClosure = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Scheduling.Rules.HighProfits.GetDescription(),
                    HighProfitRuleFactory.Version,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId);

            var highProfitsRule = _highProfitRuleFactory.Build(param, ruleCtxStream, ruleCtxMarketClosure, alertStream);

            if (param.HasFilters())
            {
                var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets);
                filteredUniverse.Subscribe(highProfitsRule);
                player.Subscribe(filteredUniverse);
            }
            else
            {
                player.Subscribe(highProfitsRule);
            }
        }
    }
}
