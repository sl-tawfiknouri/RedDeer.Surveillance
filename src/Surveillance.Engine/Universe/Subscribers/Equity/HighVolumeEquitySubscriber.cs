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
using Surveillance.Engine.Rules.Judgements.Interfaces;
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
    public class HighVolumeEquitySubscriber : BaseSubscriber, IHighVolumeEquitySubscriber
    {
        private readonly IEquityRuleHighVolumeFactory _equityRuleHighVolumeFactory;
        private readonly IRuleParameterToRulesMapperDecorator _ruleParameterMapper;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;
        private readonly IHighVolumeVenueDecoratorFilterFactory _decoratorFilterFactory;
        private readonly ILogger<HighVolumeEquitySubscriber> _logger;

        public HighVolumeEquitySubscriber(
            IEquityRuleHighVolumeFactory equityRuleHighVolumeFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            ILogger<HighVolumeEquitySubscriber> logger)
        {
            _equityRuleHighVolumeFactory = equityRuleHighVolumeFactory ?? throw new ArgumentNullException(nameof(equityRuleHighVolumeFactory));
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
            IJudgementService judgementService,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Domain.Surveillance.Scheduling.Rules.HighVolume) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .HighVolumes
                    .Where(hv => filteredParameters.Contains(hv.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var highVolumeParameters = _ruleParameterMapper.Map(execution, dtos);

            var subscriptions = SubscribeToUniverse(execution, opCtx, alertStream, dataRequestSubscriber, highVolumeParameters);

            return subscriptions;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IReadOnlyCollection<IHighVolumeRuleEquitiesParameters> highVolumeParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (highVolumeParameters != null
                && highVolumeParameters.Any())
            {
                foreach (var param in highVolumeParameters)
                {
                    var paramSubscriptions = SubscribeToParams(execution, opCtx, alertStream, dataRequestSubscriber, param);
                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                const string errorMessage = "tried to schedule a high volume rule execution with no parameters set";
                _logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }

        private IUniverseRule SubscribeToParams(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IHighVolumeRuleEquitiesParameters param)
        {
            var ruleCtx = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Surveillance.Scheduling.Rules.HighVolume.GetDescription(),
                    EquityRuleHighVolumeFactory.Version,
                    param.Id,
                    (int)Domain.Surveillance.Scheduling.Rules.HighVolume,
                    execution.IsBackTest,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId,
                    execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var highVolume = _equityRuleHighVolumeFactory.Build(param, ruleCtx, alertStream, dataRequestSubscriber, runMode);

            var highVolumeOrgFactors =
                _brokerServiceFactory.Build(
                    highVolume,
                    param.Factors,
                    param.AggregateNonFactorableIntoOwnCategory);

            var decoratedHighVolumeRule = DecorateWithFilter(opCtx, param, highVolumeOrgFactors, dataRequestSubscriber, ruleCtx, runMode);

            return decoratedHighVolumeRule;
        }

        private IUniverseRule DecorateWithFilter(
            ISystemProcessOperationContext opCtx,
            IHighVolumeRuleEquitiesParameters param,
            IUniverseRule highVolume,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
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
                    "High Volume Equity",
                    dataRequestSubscriber,
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
                            dataRequestSubscriber,
                            DataSourceForWindow(param.Windows),
                            ruleRunMode);
                }

                decoratedFilter.Subscribe(highVolume);

                return decoratedFilter;
            }
            else
            {
                return highVolume;
            }
        }
    }
}
