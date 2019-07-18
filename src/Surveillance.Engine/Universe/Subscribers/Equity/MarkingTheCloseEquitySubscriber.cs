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
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Engine.Rules.Universe.OrganisationalFactors.Interfaces;
using Surveillance.Engine.Rules.Universe.Subscribers.Equity.Interfaces;

namespace Surveillance.Engine.Rules.Universe.Subscribers.Equity
{
    public class MarkingTheCloseEquitySubscriber : IMarkingTheCloseEquitySubscriber
    {
        private readonly IEquityRuleMarkingTheCloseFactory _equityRuleMarkingTheCloseFactory;
        private readonly IRuleParameterToRulesMapperDecorator _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IHighVolumeVenueDecoratorFilterFactory _decoratorFilterFactory;
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;
        private readonly ILogger<MarkingTheCloseEquitySubscriber> _logger;

        public MarkingTheCloseEquitySubscriber(
            IEquityRuleMarkingTheCloseFactory equityRuleMarkingTheCloseFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            ILogger<MarkingTheCloseEquitySubscriber> logger)
        {
            _equityRuleMarkingTheCloseFactory = equityRuleMarkingTheCloseFactory ?? throw new ArgumentNullException(nameof(equityRuleMarkingTheCloseFactory));
            _ruleParameterMapper = ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            _universeFilterFactory = universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            _brokerServiceFactory = brokerServiceFactory ?? throw new ArgumentNullException(nameof(brokerServiceFactory));
            _decoratorFilterFactory = decoratorFilterFactory ?? throw new ArgumentNullException(nameof(decoratorFilterFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext opCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Domain.Surveillance.Scheduling.Rules.MarkingTheClose) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .MarkingTheCloses
                    .Where(mtc => filteredParameters.Contains(mtc.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var markingTheCloseParameters = _ruleParameterMapper.Map(execution, dtos);
            var subscriptions = SubscribeToUniverse(execution, opCtx, alertStream, dataRequestSubscriber, markingTheCloseParameters);

            return subscriptions;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IReadOnlyCollection<IMarkingTheCloseEquitiesParameters> markingTheCloseParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (markingTheCloseParameters != null
                && markingTheCloseParameters.Any())
            {
                foreach (var param in markingTheCloseParameters)
                {
                    var paramSubscriptions = SubscribeToParams(execution, opCtx, alertStream, param, dataRequestSubscriber);
                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                const string errorMessage = "tried to schedule a marking the close rule execution with no parameters set";
                _logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }

        private IUniverseRule SubscribeToParams(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx, 
            IUniverseAlertStream alertStream,
            IMarkingTheCloseEquitiesParameters param,
            IUniverseDataRequestsSubscriber dataRequestSubscriber)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Surveillance.Scheduling.Rules.MarkingTheClose.GetDescription(),
                    EquityRuleMarkingTheCloseFactory.Version,
                    param.Id,
                    (int)Domain.Surveillance.Scheduling.Rules.MarkingTheClose,
                    execution.IsBackTest,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId,
                    execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var markingTheClose = _equityRuleMarkingTheCloseFactory.Build(param, ruleCtx, alertStream, runMode, dataRequestSubscriber);
            var markingTheCloseOrgFactors =
                _brokerServiceFactory.Build(
                    markingTheClose,
                    param.Factors,
                    param.AggregateNonFactorableIntoOwnCategory);

            var filteredMarkingTheClose = DecorateWithFilters(opCtx, param, markingTheCloseOrgFactors, dataRequestSubscriber, ruleCtx, runMode);

            return filteredMarkingTheClose;
        }

        private IUniverseRule DecorateWithFilters(
            ISystemProcessOperationContext opCtx,
            IMarkingTheCloseEquitiesParameters param,
            IUniverseRule markingTheClose,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext processOperationRunRuleContext,
            RuleRunMode ruleRunMode)
        {
            if (param.HasInternalFilters()
                || param.HasReferenceDataFilters()
                || param.HasMarketCapFilters()
                || param.HasVenueVolumeFilters())
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
                    param.Countries,
                    param.MarketCapFilter,
                    ruleRunMode,
                    "Marking The Close Equity",
                    universeDataRequestsSubscriber,
                    processOperationRunRuleContext);

                var decoratedFilter = filteredUniverse;

                if (param.HasVenueVolumeFilters())
                {
                    decoratedFilter =
                        _decoratorFilterFactory.Build(
                            param.Windows,
                            filteredUniverse,
                            param.VenueVolumeFilter,
                            processOperationRunRuleContext,
                            universeDataRequestsSubscriber,
                            ruleRunMode);
                }

                decoratedFilter.Subscribe(markingTheClose);

                return decoratedFilter;
            }
            else
            {
                return markingTheClose;
            }
        }
    }
}
