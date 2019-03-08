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
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Subscribers.FixedIncome
{
    public class HighVolumeFixedIncomeSubscriber : IHighVolumeFixedIncomeSubscriber
    {
        private readonly IFixedIncomeHighVolumeFactory _fixedIncomeRuleHighVolumeFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;
        private readonly ILogger<HighVolumeFixedIncomeSubscriber> _logger;

        public HighVolumeFixedIncomeSubscriber(
            IFixedIncomeHighVolumeFactory fixedIncomeRuleHighVolumeFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            ILogger<HighVolumeFixedIncomeSubscriber> logger)
        {
            _fixedIncomeRuleHighVolumeFactory = fixedIncomeRuleHighVolumeFactory ?? throw new ArgumentNullException(nameof(fixedIncomeRuleHighVolumeFactory));
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
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Domain.Surveillance.Scheduling.Rules.FixedIncomeHighVolumeIssuance) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .FixedIncomeHighVolumeIssuance
                    .Where(hv => filteredParameters.Contains(hv.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var highVolumeParameters = _ruleParameterMapper.Map(dtos);
            var subscriptions = SubscribeToUniverse(execution, opCtx, alertStream, dataRequestSubscriber, highVolumeParameters);

            return subscriptions;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IReadOnlyCollection<IHighVolumeIssuanceRuleFixedIncomeParameters> highVolumeParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (highVolumeParameters != null
                && highVolumeParameters.Any())
            {
                foreach (var param in highVolumeParameters)
                {
                    var paramSubscriptions = SubscribeToParams(execution, opCtx, alertStream, dataRequestSubscriber, param);
                    var broker =
                        _brokerServiceFactory.Build(
                            paramSubscriptions,
                            param.Factors,
                            param.AggregateNonFactorableIntoOwnCategory);

                    subscriptions.Add(broker);
                }
            }
            else
            {
                _logger.LogError($"{nameof(HighVolumeFixedIncomeSubscriber)} - tried to schedule a {nameof(FixedIncomeHighVolumeIssuanceRule)} rule execution with no parameters set");
                opCtx.EventError($"{nameof(HighVolumeFixedIncomeSubscriber)} - tried to schedule a {nameof(FixedIncomeHighVolumeIssuanceRule)} rule execution with no parameters set");
            }

            return subscriptions;
        }

        private IUniverseCloneableRule SubscribeToParams(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IHighVolumeIssuanceRuleFixedIncomeParameters param)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Surveillance.Scheduling.Rules.FixedIncomeHighVolumeIssuance.GetDescription(),
                    FixedIncomeHighVolumeFactory.Version,
                    param.Id,
                    (int)Domain.Surveillance.Scheduling.Rules.FixedIncomeHighVolumeIssuance,
                    execution.IsBackTest,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId,
                    execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var highVolume = _fixedIncomeRuleHighVolumeFactory.BuildRule(param, ruleCtx, alertStream, runMode);

            if (param.HasFilters())
            {
                _logger.LogInformation($"{nameof(HighVolumeFixedIncomeSubscriber)} parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");

                var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets, param.Funds, param.Strategies);
                filteredUniverse.Subscribe(highVolume);

                return filteredUniverse;
            }
            else
            {
                return highVolume;
            }
        }
    }
}
