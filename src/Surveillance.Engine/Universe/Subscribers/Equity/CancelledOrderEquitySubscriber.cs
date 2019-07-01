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
using Surveillance.Engine.Rules.Factories.Equities;
using Surveillance.Engine.Rules.Factories.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Subscribers.Equity
{
    public class CancelledOrderEquitySubscriber : ICancelledOrderEquitySubscriber
    {
        private readonly IEquityRuleCancelledOrderFactory _equityRuleCancelledOrderFactory;
        private readonly IRuleParameterToRulesMapperDecorator _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;
        private readonly ILogger<CancelledOrderEquitySubscriber> _logger;

        public CancelledOrderEquitySubscriber(
            IEquityRuleCancelledOrderFactory equityRuleCancelledOrderFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            ILogger<CancelledOrderEquitySubscriber> logger)
        {
            _equityRuleCancelledOrderFactory = equityRuleCancelledOrderFactory;
            _ruleParameterMapper = ruleParameterMapper;
            _universeFilterFactory = universeFilterFactory;
            _brokerServiceFactory = brokerServiceFactory;
            _logger = logger;
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ab => ab.Rule)?.Contains(Domain.Surveillance.Scheduling.Rules.CancelledOrders) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .CancelledOrders
                    .Where(co => filteredParameters.Contains(co.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var cancelledOrderParameters = _ruleParameterMapper.Map(execution, dtos);

            return SubscribeToUniverse(execution, opCtx, alertStream, cancelledOrderParameters);
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IReadOnlyCollection<ICancelledOrderRuleEquitiesParameters> cancelledOrderParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (cancelledOrderParameters != null
                && cancelledOrderParameters.Any())
            {
                foreach (var param in cancelledOrderParameters)
                {
                    var baseSubscriber = SubscribeParamToUniverse(execution, opCtx, alertStream, param);
                    subscriptions.Add(baseSubscriber);
                }
            }
            else
            {
                const string errorMessage = "tried to schedule a cancelled order rule execution with no parameters set";
                _logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }

        private IUniverseRule SubscribeParamToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            ICancelledOrderRuleEquitiesParameters param)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Surveillance.Scheduling.Rules.CancelledOrders.GetDescription(),
                    EquityRuleCancelledOrderFactory.Version,
                    param.Id,
                    (int)Domain.Surveillance.Scheduling.Rules.CancelledOrders,
                    execution.IsBackTest,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId,
                    execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var cancelledOrderRule = _equityRuleCancelledOrderFactory.Build(param, ruleCtx, alertStream, runMode);
            var cancelledOrderOrgFactors = _brokerServiceFactory.Build(cancelledOrderRule, param.Factors, param.AggregateNonFactorableIntoOwnCategory);
            var cancelledOrderFiltered = DecorateWithFilter(opCtx, param, cancelledOrderOrgFactors);

            return cancelledOrderFiltered;
        }

        private IUniverseRule DecorateWithFilter(
            ISystemProcessOperationContext opCtx,
            ICancelledOrderRuleEquitiesParameters param,
            IUniverseRule cancelledOrderRule)
        {
            if (param.HasInternalFilters() || param.HasReferenceDataFilters())
            {
                _logger.LogInformation($"parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");
                var filteredUniverse = _universeFilterFactory.Build(
                    param.Accounts,
                    param.Traders,
                    param.Markets,
                    param.Funds,
                    param.Strategies,
                    param.Sectors,
                    param.Industries,
                    param.Regions,
                    param.Countries);
                filteredUniverse.Subscribe(cancelledOrderRule);

                return filteredUniverse;
            }

            return cancelledOrderRule;
        }
    }
}
