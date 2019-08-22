namespace Surveillance.Engine.Rules.Universe.Filter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Engine.Rules.Universe.MarketEvents;

    public class HighVolumeVenueFilter : BaseUniverseRule, IHighVolumeVenueFilter
    {
        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private readonly DecimalRangeRuleFilter _decimalRangeRuleFilter;

        private readonly TimeSpan _eventExpiration;

        private readonly ILogger<HighVolumeVenueFilter> _logger;

        private readonly IUniverseOrderFilter _orderFilter;

        private readonly DataSource _source;

        private readonly IMarketTradingHoursService _tradingHoursService;

        private bool _hadMissingData;

        public HighVolumeVenueFilter(
            TimeWindows timeWindows,
            DecimalRangeRuleFilter decimalRangeRuleFilter,
            IUniverseOrderFilter universeOrderFilter,
            ISystemProcessOperationRunRuleContext runRuleContext,
            IUniverseMarketCacheFactory universeMarketCacheFactory,
            RuleRunMode ruleRunMode,
            IMarketTradingHoursService marketTradingHoursService,
            IUniverseDataRequestsSubscriber dataRequestsSubscriber,
            DataSource source,
            ILogger<TradingHistoryStack> stackLogger,
            ILogger<HighVolumeVenueFilter> logger)
            : base(
                timeWindows.BackwardWindowSize,
                timeWindows.FutureWindowSize,
                Rules.UniverseFilter,
                Versioner.Version(1, 0),
                nameof(HighVolumeVenueFilter),
                runRuleContext,
                universeMarketCacheFactory,
                ruleRunMode,
                logger,
                stackLogger)
        {
            this._eventExpiration = this.BackwardWindowSize + this.BackwardWindowSize + TimeSpan.FromDays(3);
            this._tradingHoursService = marketTradingHoursService
                                        ?? throw new ArgumentNullException(nameof(marketTradingHoursService));
            this._decimalRangeRuleFilter = decimalRangeRuleFilter ?? DecimalRangeRuleFilter.None();
            this._orderFilter = universeOrderFilter ?? throw new ArgumentNullException(nameof(universeOrderFilter));
            this._dataRequestSubscriber =
                dataRequestsSubscriber ?? throw new ArgumentNullException(nameof(dataRequestsSubscriber));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.UniverseEventsPassedFilter = new HashSet<Order>();
            this._source = source;
        }

        public HashSet<Order> UniverseEventsPassedFilter { get; set; }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void EndOfUniverse()
        {
            if (this._hadMissingData && this.RunMode == RuleRunMode.ValidationRun)
                this._dataRequestSubscriber.SubmitRequest();

            this._logger.LogInformation("Eschaton occurred");
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this._orderFilter.Filter(value);
        }

        protected override void Genesis()
        {
            this._logger.LogInformation("Genesis occurred");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            this._logger.LogInformation("Market Close occurred");

            this.RunRuleForAllTradingHistoriesInMarket(exchange);
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this._logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred {exchange?.MarketOpen}");
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            this.FlushFilterEvents();

            if (history == null)
            {
                this._logger.LogInformation("null history received by run post order event");
                return;
            }

            var activeHistory = history.ActiveTradeHistory();

            if (activeHistory == null || !activeHistory.Any()) return;

            if (this._decimalRangeRuleFilter.Type == RuleFilterType.None)
            {
                this._logger.LogInformation($"No filter applied (RuleFilterType.None)");
                this.UpdatePassedFilterWithOrders(activeHistory);
                return;
            }

            var mostRecentTrade = activeHistory.Peek();

            var tradingHours =
                this._tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);

            if (!tradingHours.IsValid)
            {
                this._logger.LogError(
                    $"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");

                this.UpdatePassedFilterWithOrders(activeHistory);
                return;
            }

            var marketTradedVolume = this.RetrieveMarketTradedVolume(mostRecentTrade, tradingHours, activeHistory);

            if (marketTradedVolume == null)
            {
                this._logger.LogInformation($"{mostRecentTrade?.Instrument} had no market traded volume for {this.UniverseDateTime} and {mostRecentTrade.Market?.MarketIdentifierCode}");

                return;
            }

            var volumeTraded = activeHistory.Sum(_ => _.OrderFilledVolume ?? 0);

            if (marketTradedVolume <= 0)
            {
                this._logger.LogInformation(
                    $"market traded volume was {marketTradedVolume} for {mostRecentTrade.Instrument.Identifiers}");

                this.UpdatePassedFilterWithOrders(activeHistory);
                return;
            }

            if (volumeTraded <= 0)
            {
                this._logger.LogInformation(
                    $"market traded volume was {volumeTraded} for {mostRecentTrade.Instrument.Identifiers}");

                this.UpdatePassedFilterWithOrders(activeHistory);
                return;
            }

            var proportionOfTradedVolume = volumeTraded / (decimal)marketTradedVolume;

            this._logger.LogInformation($"proportion of traded volume was {proportionOfTradedVolume} calculated from volume traded {volumeTraded} and market traded volume of {(decimal)marketTradedVolume} for {mostRecentTrade.Instrument.Identifiers}");

            var passedFilter = false;

            if (this._decimalRangeRuleFilter.Type == RuleFilterType.Include)
            {
                passedFilter =
                    (this._decimalRangeRuleFilter.Max == null
                     || this._decimalRangeRuleFilter.Max == 1
                     || proportionOfTradedVolume <= this._decimalRangeRuleFilter.Max)
                    && 
                    (this._decimalRangeRuleFilter.Min == null
                     || proportionOfTradedVolume >= this._decimalRangeRuleFilter.Min);

                this._logger.LogInformation($"{mostRecentTrade.Instrument.Identifiers} was evaluated on an include filter and it was found to be PASSED FILTER? {passedFilter}");
            }
            else if (this._decimalRangeRuleFilter.Type == RuleFilterType.Exclude)
            {
                passedFilter =
                    this._decimalRangeRuleFilter.Max == null 
                    || this._decimalRangeRuleFilter.Max == 1
                    || proportionOfTradedVolume > this._decimalRangeRuleFilter.Max
                    || this._decimalRangeRuleFilter.Min == null
                    || proportionOfTradedVolume < this._decimalRangeRuleFilter.Min;

                this._logger.LogInformation($"{mostRecentTrade.Instrument.Identifiers} was evaluated on an exclude filter and it was found to be PASSED FILTER? {passedFilter}");
            }

            if (passedFilter) this.UpdatePassedFilterWithOrders(activeHistory);
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        private void FlushFilterEvents()
        {
            this.UniverseEventsPassedFilter.RemoveWhere(
                _ => _ == null || _.MostRecentDateEvent() < this.UniverseDateTime - this._eventExpiration);
        }

        private long InterdayMarketTradedVolume(
            MarketDataResponse<List<EquityInstrumentInterDayTimeBar>> securityResult)
        {
            var marketTradedVolume = securityResult.Response.Sum(_ => _.DailySummaryTimeBar.DailyVolume.Traded);

            return marketTradedVolume;
        }

        private long IntradayMarketTradedVolume(
            MarketDataResponse<List<EquityInstrumentIntraDayTimeBar>> securityResult)
        {
            var marketTradedVolume = securityResult.Response.Sum(_ => _.SpreadTimeBar.Volume.Traded);

            return marketTradedVolume;
        }

        private long? RetrieveMarketTradedVolume(
            Order mostRecentTrade,
            ITradingHours tradingHours,
            Stack<Order> activeHistory)
        {
            var closeTime = this._source == DataSource.AllIntraday
                                ? this.UniverseDateTime
                                : tradingHours.ClosingInUtcForDay(this.UniverseDateTime);

            var marketDataRequest = new MarketDataRequest(
                mostRecentTrade.Market?.MarketIdentifierCode,
                mostRecentTrade.Instrument.Cfi,
                mostRecentTrade.Instrument.Identifiers,
                tradingHours.OpeningInUtcForDay(this.UniverseDateTime.Subtract(this.BackwardWindowSize)),
                closeTime,
                this.RuleCtx.Id(),
                this._source);

            var hadMissingData = false;
            long? marketTradedVolume = null;

            switch (this._source)
            {
                case DataSource.AllInterday:
                    var securityResultInterday = this.UniverseEquityInterdayCache.GetMarkets(marketDataRequest);
                    hadMissingData = securityResultInterday.HadMissingData;

                    if (!hadMissingData)
                        marketTradedVolume = this.InterdayMarketTradedVolume(securityResultInterday);

                    break;
                case DataSource.AllIntraday:
                    var securityResultIntraday = this.UniverseEquityIntradayCache.GetMarkets(marketDataRequest);
                    hadMissingData = securityResultIntraday.HadMissingData;

                    if (!hadMissingData)
                        marketTradedVolume = this.IntradayMarketTradedVolume(securityResultIntraday);

                    break;
            }

            if (hadMissingData && this.RunMode == RuleRunMode.ForceRun)
            {
                this._logger.LogInformation($"{mostRecentTrade.Instrument.Identifiers} had missing data on a force run, allowing the trade through");

                this.UpdatePassedFilterWithOrders(activeHistory);
                return null;
            }

            if (hadMissingData && this.RunMode == RuleRunMode.ValidationRun)
            {
                this._logger.LogInformation(
                    $"market traded volume was not calculable for {mostRecentTrade.Instrument.Identifiers} due to missing data");
                this._hadMissingData = true;
                return null;
            }

            return marketTradedVolume;
        }

        private void UpdatePassedFilterWithOrders(Stack<Order> orders)
        {
            if (orders == null || !orders.Any()) return;

            var tempStack = new Stack<Order>(orders);

            while (tempStack.Any())
            {
                var order = tempStack.Pop();
                if (this.UniverseEventsPassedFilter.Contains(order)) continue;

                this.UniverseEventsPassedFilter.Add(order);
            }
        }
    }
}