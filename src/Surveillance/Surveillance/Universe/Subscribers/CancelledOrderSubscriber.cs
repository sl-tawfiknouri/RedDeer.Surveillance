﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Scheduling;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.OrganisationalFactors.Interfaces;
using Surveillance.Universe.Subscribers.Interfaces;
using Utilities.Extensions;

namespace Surveillance.Universe.Subscribers
{
    public class CancelledOrderSubscriber : ICancelledOrderSubscriber
    {
        private readonly ICancelledOrderRuleFactory _cancelledOrderRuleFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IOrganisationalFactorBrokerFactory _brokerFactory;
        private readonly ILogger<CancelledOrderSubscriber> _logger;

        public CancelledOrderSubscriber(
            ICancelledOrderRuleFactory cancelledOrderRuleFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerFactory brokerFactory,
            ILogger<CancelledOrderSubscriber> logger)
        {
            _cancelledOrderRuleFactory = cancelledOrderRuleFactory;
            _ruleParameterMapper = ruleParameterMapper;
            _universeFilterFactory = universeFilterFactory;
            _brokerFactory = brokerFactory;
            _logger = logger;
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ab => ab.Rule)?.Contains(Domain.Scheduling.Rules.CancelledOrders) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .CancelledOrders
                    .Where(co => filteredParameters.Contains(co.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var cancelledOrderParameters = _ruleParameterMapper.Map(dtos);

            return SubscribeToUniverse(execution, opCtx, alertStream, cancelledOrderParameters);
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IReadOnlyCollection<ICancelledOrderRuleParameters> cancelledOrderParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (cancelledOrderParameters != null
                && cancelledOrderParameters.Any())
            {
                foreach (var param in cancelledOrderParameters)
                {
                    var baseSubscriber = SubscribeParamToUniverse(execution, opCtx, alertStream, param);
                    var broker = _brokerFactory.Build(baseSubscriber, param.Factors, param.AggregateNonFactorableIntoOwnCategory);

                    subscriptions.Add(broker);
                }
            }
            else
            {
                _logger.LogError("Rule Scheduler - tried to schedule a cancelled order rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a cancelled order rule execution with no parameters set");
            }

            return subscriptions;
        }

        private IUniverseCloneableRule SubscribeParamToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            ICancelledOrderRuleParameters param)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Scheduling.Rules.CancelledOrders.GetDescription(),
                    CancelledOrderRuleFactory.Version,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId);

            var cancelledOrderRule = _cancelledOrderRuleFactory.Build(param, ruleCtx, alertStream);

            if (param.HasFilters())
            {
                var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets);
                filteredUniverse.Subscribe(cancelledOrderRule);

                return filteredUniverse;
            }

            return cancelledOrderRule;
        }
    }
}