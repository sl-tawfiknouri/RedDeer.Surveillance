namespace Surveillance.Engine.Rules.Universe.Subscribers.Equity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Scheduling;

    using Infrastructure.Network.Extensions;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using SharedKernel.Contracts.Markets;

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

    public class HighProfitsEquitySubscriber : BaseSubscriber, IHighProfitsEquitySubscriber
    {
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;

        private readonly IHighVolumeVenueDecoratorFilterFactory _decoratorFilterFactory;

        private readonly IEquityRuleHighProfitFactory _equityRuleHighProfitFactory;

        private readonly ILogger _logger;

        private readonly IRuleParameterToRulesMapperDecorator _ruleParameterMapper;

        private readonly IUniverseFilterFactory _universeFilterFactory;

        public HighProfitsEquitySubscriber(
            IEquityRuleHighProfitFactory equityRuleHighProfitFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            ILogger<UniverseRuleSubscriber> logger)
        {
            this._equityRuleHighProfitFactory = equityRuleHighProfitFactory
                                                ?? throw new ArgumentNullException(nameof(equityRuleHighProfitFactory));
            this._ruleParameterMapper =
                ruleParameterMapper ?? throw new ArgumentNullException(nameof(ruleParameterMapper));
            this._universeFilterFactory =
                universeFilterFactory ?? throw new ArgumentNullException(nameof(universeFilterFactory));
            this._brokerServiceFactory =
                brokerServiceFactory ?? throw new ArgumentNullException(nameof(brokerServiceFactory));
            this._decoratorFilterFactory =
                decoratorFilterFactory ?? throw new ArgumentNullException(nameof(decoratorFilterFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IReadOnlyCollection<IObserver<IUniverseEvent>> CollateSubscriptions(
            ScheduledExecution execution,
            RuleParameterDto ruleParameters,
            ISystemProcessOperationContext operationContext,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IUniverseAlertStream alertStream)
        {
            if (!execution.Rules?.Select(ru => ru.Rule)?.Contains(Rules.HighProfits) ?? true)
                return new IObserver<IUniverseEvent>[0];

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos = ruleParameters.HighProfits
                .Where(hp => filteredParameters.Contains(hp.Id, StringComparer.InvariantCultureIgnoreCase)).ToList();

            var highProfitParameters = this._ruleParameterMapper.Map(execution, dtos);

            return this.SubscribeToUniverse(
                execution,
                operationContext,
                dataRequestSubscriber,
                judgementService,
                highProfitParameters);
        }

        private IUniverseRule DecorateWithFilter(
            ISystemProcessOperationContext opCtx,
            IHighProfitsRuleEquitiesParameters param,
            IUniverseRule highProfitsRule,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            ISystemProcessOperationRunRuleContext processOperationRunRuleContext,
            RuleRunMode ruleRunMode)
        {
            if (param.HasInternalFilters() || param.HasReferenceDataFilters() || param.HasMarketCapFilters()
                || param.HasVenueVolumeFilters())
            {
                this._logger.LogInformation($"parameters had filters. Inserting filtered universe in {opCtx.Id} OpCtx");

                var filteredUniverse = this._universeFilterFactory.Build(
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
                    decoratedFilter = this._decoratorFilterFactory.Build(
                        param.Windows,
                        filteredUniverse,
                        param.VenueVolumeFilter,
                        processOperationRunRuleContext,
                        universeDataRequestsSubscriber,
                        this.HighProfitDataSourceForWindow(param),
                        ruleRunMode);

                decoratedFilter.Subscribe(highProfitsRule);

                return decoratedFilter;
            }

            return highProfitsRule;
        }

        private DataSource HighProfitDataSourceForWindow(IHighProfitsRuleEquitiesParameters parameters)
        {
            if (parameters == null) return DataSource.AllInterday;

            if (parameters.PerformHighProfitWindowAnalysis) return DataSource.AllIntraday;

            if (parameters.PerformHighProfitDailyAnalysis) return DataSource.AllInterday;

            return this.DataSourceForWindow(parameters.Windows);
        }

        private IUniverseRule SubscribeParameters(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IHighProfitsRuleEquitiesParameters param)
        {
            var ruleCtxStream = opCtx.CreateAndStartRuleRunContext(
                Rules.HighProfits.GetDescription(),
                EquityRuleHighProfitFactory.Version,
                param.Id,
                (int)Rules.HighProfits,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var ruleCtxMarketClosure = opCtx.CreateAndStartRuleRunContext(
                Rules.HighProfits.GetDescription(),
                EquityRuleHighProfitFactory.Version,
                param.Id,
                (int)Rules.HighProfits,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var highProfitsRule = this._equityRuleHighProfitFactory.Build(
                param,
                ruleCtxStream,
                ruleCtxMarketClosure,
                dataRequestSubscriber,
                judgementService,
                execution);

            var highProfitsRuleOrgFactor = this._brokerServiceFactory.Build(
                highProfitsRule,
                param.Factors,
                param.AggregateNonFactorableIntoOwnCategory);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var decoratedHighProfits = this.DecorateWithFilter(
                opCtx,
                param,
                highProfitsRuleOrgFactor,
                dataRequestSubscriber,
                ruleCtxMarketClosure,
                runMode);

            return decoratedHighProfits;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            IJudgementService judgementService,
            IReadOnlyCollection<IHighProfitsRuleEquitiesParameters> highProfitParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (highProfitParameters != null && highProfitParameters.Any())
            {
                foreach (var param in highProfitParameters)
                {
                    var cloneableRule = this.SubscribeParameters(
                        execution,
                        opCtx,
                        dataRequestSubscriber,
                        judgementService,
                        param);
                    subscriptions.Add(cloneableRule);
                }
            }
            else
            {
                const string errorMessage = "tried to schedule a high profit rule execution with no parameters set";
                this._logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }
    }
}