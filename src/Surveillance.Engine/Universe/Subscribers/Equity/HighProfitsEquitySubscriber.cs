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
    public class HighProfitsEquitySubscriber : BaseSubscriber, IHighProfitsEquitySubscriber
    {
        private readonly IEquityRuleHighProfitFactory _equityRuleHighProfitFactory;
        private readonly IRuleParameterToRulesMapperDecorator _ruleParameterMapper;
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;
        private readonly IUniverseFilterFactory _universeFilterFactory;
        private readonly IHighVolumeVenueDecoratorFilterFactory _decoratorFilterFactory;
        private readonly ILogger _logger;

        public HighProfitsEquitySubscriber(
            IEquityRuleHighProfitFactory equityRuleHighProfitFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            ILogger<UniverseRuleSubscriber> logger)
        {
            _equityRuleHighProfitFactory = equityRuleHighProfitFactory ?? throw new ArgumentNullException(nameof(equityRuleHighProfitFactory));
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
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Domain.Surveillance.Scheduling.Rules.HighProfits) ?? true)
            {
                return new IObserver<IUniverseEvent>[0];
            }

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos =
                ruleParameters
                    .HighProfits
                    .Where(hp => filteredParameters.Contains(hp.Id, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

            var highProfitParameters = _ruleParameterMapper.Map(execution, dtos);

            return SubscribeToUniverse(execution, opCtx, dataRequestSubscriber, judgementService, highProfitParameters);
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IReadOnlyCollection<IHighProfitsRuleEquitiesParameters> highProfitParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (highProfitParameters != null
                && highProfitParameters.Any())
            {
                foreach (var param in highProfitParameters)
                {
                    var cloneableRule = SubscribeParameters(execution, opCtx, dataRequestSubscriber, judgementService, param);
                    subscriptions.Add(cloneableRule);
                }
            }
            else
            {
                const string errorMessage = "tried to schedule a high profit rule execution with no parameters set";
                _logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }

        private IUniverseRule SubscribeParameters(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IHighProfitsRuleEquitiesParameters param)
        {
            var ruleCtxStream = opCtx
                    .CreateAndStartRuleRunContext(
                        Domain.Surveillance.Scheduling.Rules.HighProfits.GetDescription(),
                        EquityRuleHighProfitFactory.Version,
                        param.Id,
                        (int)Domain.Surveillance.Scheduling.Rules.HighProfits,
                        execution.IsBackTest,
                        execution.TimeSeriesInitiation.DateTime,
                        execution.TimeSeriesTermination.DateTime,
                        execution.CorrelationId,
                        execution.IsForceRerun);

            var ruleCtxMarketClosure = opCtx
                .CreateAndStartRuleRunContext(
                    Domain.Surveillance.Scheduling.Rules.HighProfits.GetDescription(),
                    EquityRuleHighProfitFactory.Version,
                    param.Id,
                    (int)Domain.Surveillance.Scheduling.Rules.HighProfits,
                    execution.IsBackTest,
                    execution.TimeSeriesInitiation.DateTime,
                    execution.TimeSeriesTermination.DateTime,
                    execution.CorrelationId,
                    execution.IsForceRerun);

            var highProfitsRule = 
                _equityRuleHighProfitFactory.Build(
                    param,
                    ruleCtxStream,
                    ruleCtxMarketClosure,
                    dataRequestSubscriber,
                    judgementService,
                    execution);

            var highProfitsRuleOrgFactor =
                _brokerServiceFactory.Build(
                    highProfitsRule,
                    param.Factors,
                    param.AggregateNonFactorableIntoOwnCategory);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var decoratedHighProfits = DecorateWithFilter(opCtx, param, highProfitsRuleOrgFactor, dataRequestSubscriber, ruleCtxMarketClosure, runMode);

            return decoratedHighProfits;
        }

        private IUniverseRule DecorateWithFilter(
            ISystemProcessOperationContext opCtx,
            IHighProfitsRuleEquitiesParameters param,
            IUniverseRule highProfitsRule,
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
                    "High Profits Equity",
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
                            DataSourceForWindow(param.Windows),
                            ruleRunMode);
                }

                decoratedFilter.Subscribe(highProfitsRule);

                return decoratedFilter;
            }
            else
            {
                return highProfitsRule;
            }
        }
    }
}
