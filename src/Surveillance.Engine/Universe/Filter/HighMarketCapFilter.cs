namespace Surveillance.Engine.Rules.Universe.Filter
{
    using System;

    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;

    public class HighMarketCapFilter : IHighMarketCapFilter
    {
        private readonly ILogger _logger;

        private readonly DecimalRangeRuleFilter _marketCapFilter;

        private readonly string _name;

        private readonly ISystemProcessOperationRunRuleContext _operationRunRuleContext;

        private readonly RuleRunMode _ruleRunMode;

        private readonly IMarketTradingHoursService _tradingHoursService;

        private readonly IUniverseDataRequestsSubscriber _universeDataRequestsSubscriber;

        private readonly IUniverseEquityInterDayCache _universeEquityInterdayCache;

        private bool _requestData;

        public HighMarketCapFilter(
            IUniverseMarketCacheFactory factory,
            RuleRunMode ruleRunMode,
            DecimalRangeRuleFilter marketCap,
            IMarketTradingHoursService tradingHoursService,
            ISystemProcessOperationRunRuleContext operationRunRuleContext,
            IUniverseDataRequestsSubscriber universeDataRequestsSubscriber,
            string ruleName,
            ILogger<HighMarketCapFilter> logger)
        {
            this._universeEquityInterdayCache = factory?.BuildInterday(ruleRunMode)
                                                ?? throw new ArgumentNullException(nameof(factory));

            this._ruleRunMode = ruleRunMode;
            this._marketCapFilter = marketCap ?? DecimalRangeRuleFilter.None();

            this._tradingHoursService =
                tradingHoursService ?? throw new ArgumentNullException(nameof(tradingHoursService));
            this._operationRunRuleContext = operationRunRuleContext
                                            ?? throw new ArgumentNullException(nameof(operationRunRuleContext));
            this._universeDataRequestsSubscriber = universeDataRequestsSubscriber;
            this._name = ruleName;
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool Filter(IUniverseEvent universeEvent)
        {
            if (universeEvent == null) return false;

            if (universeEvent.StateChange == UniverseStateEvent.EquityInterDayTick) this.EquityInterDay(universeEvent);

            if (universeEvent.StateChange == UniverseStateEvent.Eschaton && this._requestData
                                                                         && this._ruleRunMode
                                                                         == RuleRunMode.ValidationRun)
                this._universeDataRequestsSubscriber.SubmitRequest();

            if (universeEvent.StateChange != UniverseStateEvent.Order
                && universeEvent.StateChange != UniverseStateEvent.OrderPlaced) return false;

            if (!(universeEvent.UnderlyingEvent is Order mostRecentTrade)) return false;

            if (this._marketCapFilter?.Type != RuleFilterType.Include) return false;

            var tradingHours =
                this._tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                this._logger.LogError(
                    $"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");
                return true;
            }

            var universeDateTime = universeEvent.EventTime;
            var marketDataRequest = new MarketDataRequest(
                mostRecentTrade.Market?.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(universeDateTime),
                tradingHours.MinimumOfCloseInUtcForDayOrUniverse(universeDateTime),
                this._operationRunRuleContext?.Id(),
                DataSource.AllInterday);

            var securityResult = this._universeEquityInterdayCache.Get(marketDataRequest);

            if (securityResult.HadMissingData && this._ruleRunMode == RuleRunMode.ValidationRun)
                this._requestData = true;

            if (securityResult.HadMissingData)
            {
                this._logger.LogInformation($"Missing data for {marketDataRequest}.");
                return true;
            }

            var security = securityResult.Response;
            var marketCap = security.DailySummaryTimeBar.MarketCap.GetValueOrDefault(0);

            var min = this._marketCapFilter.Min ?? marketCap;
            var max = this._marketCapFilter.Max ?? marketCap;

            return !(marketCap >= min && marketCap <= max);
        }

        private void EquityInterDay(IUniverseEvent universeEvent)
        {
            if (!(universeEvent.UnderlyingEvent is EquityInterDayTimeBarCollection value)) return;

            this._logger?.LogInformation(
                $"Equity inter day event in HighMarketCapFilter occuring for {this._name} | event/universe time {universeEvent.EventTime} | MIC {value.Exchange?.MarketIdentifierCode} | timestamp  {value.Epoch} | security count {value.Securities?.Count ?? 0}");

            this._universeEquityInterdayCache.Add(value);
        }
    }
}