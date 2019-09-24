namespace Surveillance.Engine.Rules.Universe.Subscribers.Equity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Scheduling;

    using Infrastructure.Network.Extensions;

    using Microsoft.Extensions.Logging;

    using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
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

    public class WashTradeEquitySubscriber : BaseSubscriber, IWashTradeEquitySubscriber
    {
        private readonly IOrganisationalFactorBrokerServiceFactory _brokerServiceFactory;

        private readonly IHighVolumeVenueDecoratorFilterFactory _decoratorFilterFactory;

        private readonly IEquityRuleWashTradeFactory _equityRuleWashTradeFactory;

        private readonly ILogger<WashTradeEquitySubscriber> _logger;

        private readonly IRuleParameterToRulesMapperDecorator _ruleParameterMapper;

        private readonly IUniverseFilterFactory _universeFilterFactory;

        public WashTradeEquitySubscriber(
            IEquityRuleWashTradeFactory equityRuleWashTradeFactory,
            IRuleParameterToRulesMapperDecorator ruleParameterMapper,
            IUniverseFilterFactory universeFilterFactory,
            IOrganisationalFactorBrokerServiceFactory brokerServiceFactory,
            IHighVolumeVenueDecoratorFilterFactory decoratorFilterFactory,
            ILogger<WashTradeEquitySubscriber> logger)
        {
            this._equityRuleWashTradeFactory = equityRuleWashTradeFactory
                                               ?? throw new ArgumentNullException(nameof(equityRuleWashTradeFactory));
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
            if (!execution.Rules?.Select(ru => ru.Rule).Contains(Rules.WashTrade) ?? true)
                return new IObserver<IUniverseEvent>[0];

            var filteredParameters = execution.Rules.SelectMany(ru => ru.Ids).Where(ru => ru != null).ToList();
            var dtos = ruleParameters.WashTrades
                .Where(wt => filteredParameters.Contains(wt.Id, StringComparer.InvariantCultureIgnoreCase)).ToList();

            var washTradeParameters = this._ruleParameterMapper.Map(execution, dtos);
            var subscriptions = this.SubscribeToUniverse(
                execution,
                operationContext,
                alertStream,
                dataRequestSubscriber,
                washTradeParameters);

            return subscriptions;
        }

        private IUniverseRule DecorateWithFilter(
            ISystemProcessOperationContext opCtx,
            IWashTradeRuleEquitiesParameters param,
            IUniverseRule washTrade,
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
                    "Wash Trade Equity",
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
                        this.DataSourceForWindow(param.Windows),
                        ruleRunMode);

                decoratedFilter.Subscribe(washTrade);

                return decoratedFilter;
            }

            return washTrade;
        }

        private IUniverseRule SubscribeToParameters(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            IWashTradeRuleEquitiesParameters param)
        {
            var ctx = opCtx.CreateAndStartRuleRunContext(
                Rules.WashTrade.GetDescription(),
                EquityRuleWashTradeFactory.Version,
                param.Id,
                (int)Rules.WashTrade,
                execution.IsBackTest,
                execution.TimeSeriesInitiation.DateTime,
                execution.TimeSeriesTermination.DateTime,
                execution.CorrelationId,
                execution.IsForceRerun);

            var runMode = execution.IsForceRerun ? RuleRunMode.ForceRun : RuleRunMode.ValidationRun;
            var washTrade = this._equityRuleWashTradeFactory.Build(param, ctx, alertStream, runMode);
            var washTradeOrgFactors = this._brokerServiceFactory.Build(
                washTrade,
                param.Factors,
                param.AggregateNonFactorableIntoOwnCategory);

            var washTradeFiltered = this.DecorateWithFilter(
                opCtx,
                param,
                washTradeOrgFactors,
                universeDataRequestsSubscriber,
                ctx,
                runMode);

            return washTradeFiltered;
        }

        private IReadOnlyCollection<IObserver<IUniverseEvent>> SubscribeToUniverse(
            ScheduledExecution execution,
            ISystemProcessOperationContext opCtx,
            IUniverseAlertStream alertStream,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            IReadOnlyCollection<IWashTradeRuleEquitiesParameters> washTradeParameters)
        {
            var subscriptions = new List<IObserver<IUniverseEvent>>();

            if (washTradeParameters != null && washTradeParameters.Any())
            {
                foreach (var param in washTradeParameters)
                {
                    var paramSubscriptions = this.SubscribeToParameters(
                        execution,
                        opCtx,
                        alertStream,
                        universeDataRequestsSubscriber,
                        param);
                    subscriptions.Add(paramSubscriptions);
                }
            }
            else
            {
                const string errorMessage = "tried to schedule a wash trade rule execution with no parameters set";
                this._logger.LogError(errorMessage);
                opCtx.EventError(errorMessage);
            }

            return subscriptions;
        }
    }
}