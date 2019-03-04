﻿using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Enums;
using Domain.Scheduling;
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
using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome
{
    public class HighProfitsFixedIncomeSubscriber : IHighProfitsFixedIncomeSubscriber
    {
        private readonly IFixedIncomeHighProfitFactory _fixedIncomeRuleHighProfitsFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IOrganisationalFactorBrokerFactory _brokerFactory;
        private readonly ILogger<HighVolumeFixedIncomeSubscriber> _logger;

        public HighProfitsFixedIncomeSubscriber(
            IFixedIncomeHighProfitFactory fixedIncomeRuleHighVolumeFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerFactory brokerFactory,
            ILogger<HighVolumeFixedIncomeSubscriber> logger)
        {
            _fixedIncomeRuleHighProfitsFactory = fixedIncomeRuleHighVolumeFactory ?? throw new ArgumentNullException(nameof(fixedIncomeRuleHighVolumeFactory));
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
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Domain.Scheduling.Rules.FixedIncomeHighProfits) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();

            var dtos =
                ruleParameters
                    .FixedIncomeHighProfits
                    .Where(hv => filteredParameters.Contains(hv.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var highProfitParameters = _ruleParameterMapper.Map(dtos);
            var subscriptions = SubscribeToUniverse(execution, opCtx, alertStream, dataRequestSubscriber, highProfitParameters);

            return subscriptions;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IReadOnlyCollection<IHighProfitsRuleFixedIncomeParameters> highProfitParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (highProfitParameters != null
                && highProfitParameters.Any())
            {
                foreach (var param in highProfitParameters)
                {
                    var paramSubscriptions = SubscribeToParams(execution, opCtx, alertStream, dataRequestSubscriber, param);
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
                _logger.LogError($"{nameof(HighProfitsFixedIncomeSubscriber)} - tried to schedule a {nameof(FixedIncomeHighProfitsRule)} rule execution with no parameters set");
                opCtx.EventError($"{nameof(HighProfitsFixedIncomeSubscriber)} - tried to schedule a {nameof(FixedIncomeHighProfitsRule)} rule execution with no parameters set");
            }

            return subscriptions;
        }

        private IUniverseCloneableRule SubscribeToParams(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IHighProfitsRuleFixedIncomeParameters param)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Scheduling.Rules.FixedIncomeHighProfits.GetDescription(),
                    FixedIncomeHighProfitFactory.Version,
                    param.Id,
                    (int)Domain.Scheduling.Rules.FixedIncomeHighProfits,
                    execution.IsBackTest,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId,
                    execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var highProfits = _fixedIncomeRuleHighProfitsFactory.BuildRule(param, ruleCtx, alertStream, runMode);

            if (param.HasFilters())
            {
                _logger.LogInformation($"{nameof(HighProfitsFixedIncomeSubscriber)} parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");

                var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets, param.Funds, param.Strategies);
                filteredUniverse.Subscribe(highProfits);

                return filteredUniverse;
            }
            else
            {
                return highProfits;
            }
        }
    }
}