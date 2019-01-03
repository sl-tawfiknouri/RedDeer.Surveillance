using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Scheduling;
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
    public class MarkingTheCloseSubscriber : IMarkingTheCloseSubscriber
    {
        private readonly IMarkingTheCloseRuleFactory _markingTheCloseFactory;
        private readonly IRuleParameterToRulesMapper _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IOrganisationalFactorBrokerFactory _brokerFactory;
        private readonly ILogger<MarkingTheCloseSubscriber> _logger;

        public MarkingTheCloseSubscriber(
            IMarkingTheCloseRuleFactory markingTheCloseFactory,
            IRuleParameterToRulesMapper ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerFactory brokerFactory,
            ILogger<MarkingTheCloseSubscriber> logger)
        {
            _markingTheCloseFactory = markingTheCloseFactory ?? throw new ArgumentNullException(nameof(markingTheCloseFactory));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _brokerFactory = brokerFactory ?? throw new ArgumentNullException(nameof(brokerFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(DomainV2.Scheduling.Rules.MarkingTheClose) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .MarkingTheCloses
                    .Where(mtc => filteredParameters.Contains(mtc.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var markingTheCloseParameters = _ruleParameterMapper.Map(dtos);
            var subscriptions = SubscribeToUniverse(execution, opCtx, alertStream, markingTheCloseParameters);

            return subscriptions;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IReadOnlyCollection<Rules.MarkingTheClose.Interfaces.IMarkingTheCloseParameters> markingTheCloseParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (markingTheCloseParameters != null
                && markingTheCloseParameters.Any())
            {
                foreach (var param in markingTheCloseParameters)
                {
                    var paramSubscriptions = SubscribeToParams(execution, opCtx, alertStream, param);
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
                _logger.LogError("Rule Scheduler - tried to schedule a marking the close rule execution with no parameters set");
                opCtx.EventError("Rule Scheduler - tried to schedule a marking the close rule execution with no parameters set");
            }

            return subscriptions;
        }

        private IUniverseCloneableRule SubscribeToParams(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx, 
            IUniverseAlertStream alertStream,
            Rules.MarkingTheClose.Interfaces.IMarkingTheCloseParameters param)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    DomainV2.Scheduling.Rules.MarkingTheClose.GetDescription(),
                    MarkingTheCloseRuleFactory.Version,
                    param.Id,
                    (int)DomainV2.Scheduling.Rules.MarkingTheClose,
                    execution.IsBackTest,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId);

            var markingTheClose = _markingTheCloseFactory.Build(param, ruleCtx, alertStream);

            if (param.HasFilters())
            {
                _logger.LogInformation($"MarkingTheCloseSubscriber parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");

                var filteredUniverse = _universeFilterFactory.Build(param.Accounts, param.Traders, param.Markets);
                filteredUniverse.Subscribe(markingTheClose);

                return filteredUniverse;
            }
            else
            {
                return markingTheClose;
            }
        }
    }
}
