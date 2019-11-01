namespace Surveillance.Engine.Rules.Universe.Filter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Rules;
    using Domain.Surveillance.Rules.Interfaces;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Data.Universe.MarketEvents;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Trades.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    /// <summary>
    /// The high volume venue filter.
    /// </summary>
    public class HighVolumeVenueFilter : BaseUniverseRule, IHighVolumeVenueFilter
    {
        /// <summary>
        /// The decimal range rule filter.
        /// </summary>
        private readonly DecimalRangeRuleFilter decimalRangeRuleFilter;

        /// <summary>
        /// The source.
        /// </summary>
        private readonly DataSource source;

        /// <summary>
        /// The event expiration.
        /// </summary>
        private readonly TimeSpan eventExpiration;

        /// <summary>
        /// The order filter.
        /// </summary>
        private readonly IUniverseOrderFilter orderFilter;

        /// <summary>
        /// The trading hours service.
        /// </summary>
        private readonly IMarketTradingHoursService tradingHoursService;

        /// <summary>
        /// The data request subscriber.
        /// </summary>
        private readonly IUniverseDataRequestsSubscriber dataRequestSubscriber;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger<HighVolumeVenueFilter> logger;

        /// <summary>
        /// The had missing data.
        /// </summary>
        private bool hadMissingData;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighVolumeVenueFilter"/> class.
        /// </summary>
        /// <param name="timeWindows">
        /// The time windows.
        /// </param>
        /// <param name="decimalRangeRuleFilter">
        /// The decimal range rule filter.
        /// </param>
        /// <param name="universeOrderFilter">
        /// The universe order filter.
        /// </param>
        /// <param name="runRuleContext">
        /// The run rule context.
        /// </param>
        /// <param name="equityMarketCacheFactory">
        /// The universe market cache factory.
        /// </param>
        /// <param name="fixedIncomeMarketCacheFactory">
        /// The universe market cache factory.
        /// </param>
        /// <param name="ruleRunMode">
        /// The rule run mode.
        /// </param>
        /// <param name="marketTradingHoursService">
        /// The market trading hours service.
        /// </param>
        /// <param name="dataRequestsSubscriber">
        /// The data requests subscriber.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="stackLogger">
        /// The stack logger.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public HighVolumeVenueFilter(
            TimeWindows timeWindows,
            DecimalRangeRuleFilter decimalRangeRuleFilter,
            IUniverseOrderFilter universeOrderFilter,
            ISystemProcessOperationRunRuleContext runRuleContext,
            IUniverseEquityMarketCacheFactory equityMarketCacheFactory,
            IUniverseFixedIncomeMarketCacheFactory fixedIncomeMarketCacheFactory,
            RuleRunMode ruleRunMode,
            IMarketTradingHoursService marketTradingHoursService,
            IUniverseDataRequestsSubscriber dataRequestsSubscriber,
            DataSource source,
            ILogger<TradingHistoryStack> stackLogger,
            ILogger<HighVolumeVenueFilter> logger) 
            : base(
                timeWindows.BackwardWindowSize,
                timeWindows.BackwardWindowSize,
                timeWindows.FutureWindowSize,
                Domain.Surveillance.Scheduling.Rules.UniverseFilter,
                Versioner.Version(1,0),
                nameof(HighVolumeVenueFilter),
                runRuleContext,
                equityMarketCacheFactory,
                fixedIncomeMarketCacheFactory,
                ruleRunMode,
                logger,
                stackLogger)
        {
            this.eventExpiration = this.TradeBackwardWindowSize + this.TradeBackwardWindowSize + TimeSpan.FromDays(3);
            this.tradingHoursService = marketTradingHoursService ?? throw new ArgumentNullException(nameof(marketTradingHoursService));
            this.decimalRangeRuleFilter = decimalRangeRuleFilter ?? DecimalRangeRuleFilter.None();
            this.orderFilter = universeOrderFilter ?? throw new ArgumentNullException(nameof(universeOrderFilter));
            this.dataRequestSubscriber = dataRequestsSubscriber ?? throw new ArgumentNullException(nameof(dataRequestsSubscriber));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.UniverseEventsPassedFilter = new HashSet<Order>();
            this.source = source;
        }

        /// <summary>
        /// Gets or sets the universe events passed filter.
        /// </summary>
        public HashSet<Order> UniverseEventsPassedFilter { get; set; }

        /// <summary>
        /// The data constraints.
        /// </summary>
        /// <returns>
        /// The <see cref="IRuleDataConstraint"/>.
        /// </returns>
        public override IRuleDataConstraint DataConstraints()
        {
            var constraints = new List<RuleDataSubConstraint>();

            if (this.source == DataSource.AnyInterday)
            {
                var constraint = new RuleDataSubConstraint(
                    this.ForwardWindowSize,
                    this.TradeBackwardWindowSize,
                    DataSource.AnyInterday,
                    _ => true);

                constraints.Add(constraint);
            }

            if (this.source == DataSource.AnyIntraday)
            {
                var constraint = new RuleDataSubConstraint(
                    this.ForwardWindowSize,
                    this.TradeBackwardWindowSize,
                    DataSource.AnyIntraday,
                    _ => true);

                constraints.Add(constraint);
            }

            return new RuleDataConstraint(
                this.Rule,
                string.Empty,
                constraints);
        }

        /// <summary>
        /// The filter.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="IUniverseEvent"/>.
        /// </returns>
        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return this.orderFilter.Filter(value);
        }

        /// <summary>
        /// The run post order event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            this.FlushFilterEvents();

            if (history == null)
            {
                this.logger.LogInformation($"null history received by run post order event");
                return;
            }

            var activeHistory = history.ActiveTradeHistory();

            if (activeHistory == null
                || !activeHistory.Any())
            {
                return;
            }

            if (this.decimalRangeRuleFilter.Type == RuleFilterType.None)
            {
                this.UpdatePassedFilterWithOrders(activeHistory);
                return;
            }

            var mostRecentTrade = activeHistory.Peek();

            var tradingHours = this.tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                this.logger.LogError($"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");

                this.UpdatePassedFilterWithOrders(activeHistory);
                return;
            }

            var marketTradedVolume = this.RetrieveMarketTradedVolume(mostRecentTrade, tradingHours, activeHistory);

            if (marketTradedVolume == null)
            {
                return;
            }

            var volumeTraded = activeHistory.Sum(_ => _.OrderFilledVolume ?? 0);

            if (marketTradedVolume <= 0)
            {
                this.logger.LogInformation($"market traded volume was {marketTradedVolume} for {mostRecentTrade.Instrument.Identifiers}");

                this.UpdatePassedFilterWithOrders(activeHistory);
                return;
            }

            if (volumeTraded <= 0)
            {
                this.logger.LogInformation($"market traded volume was {volumeTraded} for {mostRecentTrade.Instrument.Identifiers}");

                this.UpdatePassedFilterWithOrders(activeHistory);
                return;
            }

            var proportionOfTradedVolume = (volumeTraded / (decimal)marketTradedVolume);

            var passedFilter = false;

            if (this.decimalRangeRuleFilter.Type == RuleFilterType.Include)
            {
                passedFilter =
                   (this.decimalRangeRuleFilter.Max == null || this.decimalRangeRuleFilter.Max == 1 || proportionOfTradedVolume <= this.decimalRangeRuleFilter.Max)
                    && (this.decimalRangeRuleFilter.Min == null || proportionOfTradedVolume >= this.decimalRangeRuleFilter.Min);
            }
            else if (this.decimalRangeRuleFilter.Type == RuleFilterType.Exclude)
            {
                passedFilter =
                    (this.decimalRangeRuleFilter.Max == null || this.decimalRangeRuleFilter.Max == 1 || proportionOfTradedVolume > this.decimalRangeRuleFilter.Max)
                    || (this.decimalRangeRuleFilter.Min == null || proportionOfTradedVolume < this.decimalRangeRuleFilter.Min);
            }

            if (passedFilter)
            {
                this.UpdatePassedFilterWithOrders(activeHistory);
            }
        }

        /// <summary>
        /// The retrieve market traded volume.
        /// </summary>
        /// <param name="mostRecentTrade">
        /// The most recent trade.
        /// </param>
        /// <param name="tradingHours">
        /// The trading hours.
        /// </param>
        /// <param name="activeHistory">
        /// The active history.
        /// </param>
        /// <returns>
        /// The <see cref="long?"/>.
        /// </returns>
        private long? RetrieveMarketTradedVolume(Order mostRecentTrade, ITradingHours tradingHours, Stack<Order> activeHistory)
        {
            var closeTime =
                this.source == DataSource.AnyIntraday
                    ? UniverseDateTime
                    : tradingHours.ClosingInUtcForDay(UniverseDateTime);

             var marketDataRequest =
                new MarketDataRequest(
                    mostRecentTrade.Market?.MarketIdentifierCode,
                    mostRecentTrade.Instrument.Cfi,
                    mostRecentTrade.Instrument.Identifiers,
                    tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(this.TradeBackwardWindowSize)),
                    closeTime,
                    RuleCtx.Id(),
                    this.source);
            
            var hadMissingData = false;
            long? marketTradedVolume = null;

            switch (this.source)
            {
                case DataSource.AnyInterday:
                    var securityResultInterday = UniverseEquityInterdayCache.GetMarkets(marketDataRequest);
                    hadMissingData = securityResultInterday.HadMissingData;

                    if (!hadMissingData)
                        marketTradedVolume = this.InterdayMarketTradedVolume(securityResultInterday);

                    break;
                case DataSource.AnyIntraday:
                    var securityResultIntraday = UniverseEquityIntradayCache.GetMarkets(marketDataRequest);
                    hadMissingData = securityResultIntraday.HadMissingData;

                    if (!hadMissingData)
                        marketTradedVolume = this.IntradayMarketTradedVolume(securityResultIntraday);

                    break;
            }

            if (hadMissingData && RunMode == RuleRunMode.ForceRun)
            {
                this.UpdatePassedFilterWithOrders(activeHistory);
                return null;
            }

            if (hadMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                this.logger.LogInformation($"market traded volume was not calculable for {mostRecentTrade.Instrument.Identifiers} due to missing data");
                this.hadMissingData = true;
                return null;
            }

            return marketTradedVolume;
        }

        /// <summary>
        /// The intraday market traded volume.
        /// </summary>
        /// <param name="securityResult">
        /// The security result.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private long IntradayMarketTradedVolume(MarketDataResponse<List<EquityInstrumentIntraDayTimeBar>> securityResult)
        {
            var marketTradedVolume = securityResult.Response.Sum(_ => _.SpreadTimeBar.Volume.Traded);

            return marketTradedVolume;
        }

        /// <summary>
        /// The interday market traded volume.
        /// </summary>
        /// <param name="securityResult">
        /// The security result.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private long InterdayMarketTradedVolume(MarketDataResponse<List<EquityInstrumentInterDayTimeBar>> securityResult)
        {
            var marketTradedVolume = securityResult.Response.Sum(_ => _.DailySummaryTimeBar.DailyVolume.Traded);

            return marketTradedVolume;
        }

        /// <summary>
        /// The run order filled event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run post order event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run initial submission event.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run initial submission event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The run order filled event delayed.
        /// </summary>
        /// <param name="history">
        /// The history.
        /// </param>
        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        /// <summary>
        /// The market open.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketOpen(MarketOpenClose exchange)
        {
            this.logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred {exchange?.MarketOpen}");
        }

        /// <summary>
        /// The market close.
        /// </summary>
        /// <param name="exchange">
        /// The exchange.
        /// </param>
        protected override void MarketClose(MarketOpenClose exchange)
        {
            this.logger.LogInformation("Market Close occurred");

            this.RunRuleForAllTradingHistoriesInMarket(exchange);
        }

        /// <summary>
        /// The genesis.
        /// </summary>
        protected override void Genesis()
        {
            this.logger.LogInformation("Genesis occurred");
        }

        /// <summary>
        /// The end of universe.
        /// </summary>
        protected override void EndOfUniverse()
        {
            if (this.hadMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                this.dataRequestSubscriber.SubmitRequest();
            }

            this.logger.LogInformation("Eschaton occurred");
        }

        /// <summary>
        /// The update passed filter with orders.
        /// </summary>
        /// <param name="orders">
        /// The orders.
        /// </param>
        private void UpdatePassedFilterWithOrders(Stack<Order> orders)
        {
            if (orders == null
                || !orders.Any())
            {
                return;
            }

            var tempStack = new Stack<Order>(orders);

            while (tempStack.Any())
            {
                var order = tempStack.Pop();
                if (this.UniverseEventsPassedFilter.Contains(order))
                {
                    continue;
                }

                this.UniverseEventsPassedFilter.Add(order);
            }
        }

        /// <summary>
        /// The flush filter events.
        /// </summary>
        private void FlushFilterEvents()
        {
            this.UniverseEventsPassedFilter
                .RemoveWhere(_ =>
                    _ == null
                    || _.MostRecentDateEvent() < (this.UniverseDateTime - this.eventExpiration));
        }
    }
}
