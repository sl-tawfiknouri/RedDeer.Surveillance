using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Markets.Timebars;
using Domain.Core.Trading.Orders;
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

namespace Surveillance.Engine.Rules.Universe.Filter
{
    public class HighVolumeVenueFilter : BaseUniverseRule, IHighVolumeVenueFilter
    {
        public HashSet<Order> UniverseEventsPassedFilter { get; set; }

        private readonly DecimalRangeRuleFilter _decimalRangeRuleFilter;
        private readonly DataSource _source;
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private readonly ILogger<HighVolumeVenueFilter> _logger;
        private readonly TimeSpan _eventExpiration;

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
                Domain.Surveillance.Scheduling.Rules.UniverseFilter,
                Versioner.Version(1,0),
                nameof(HighVolumeVenueFilter),
                runRuleContext,
                universeMarketCacheFactory,
                ruleRunMode,
                logger,
                stackLogger)
        {
            _eventExpiration = BackwardWindowSize + BackwardWindowSize + TimeSpan.FromDays(3);
            _tradingHoursService = marketTradingHoursService ?? throw new ArgumentNullException(nameof(marketTradingHoursService));
            _decimalRangeRuleFilter = decimalRangeRuleFilter ?? DecimalRangeRuleFilter.None();
            _orderFilter = universeOrderFilter ?? throw new ArgumentNullException(nameof(universeOrderFilter));
            _dataRequestSubscriber = dataRequestsSubscriber ?? throw new ArgumentNullException(nameof(dataRequestsSubscriber));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            UniverseEventsPassedFilter = new HashSet<Order>();
            _source = source;
        }

        protected override IUniverseEvent Filter(IUniverseEvent value)
        {
            return _orderFilter.Filter(value);
        }

        protected override void RunPostOrderEvent(ITradingHistoryStack history)
        {
            FlushFilterEvents();

            if (history == null)
            {
                _logger.LogInformation($"null history received by run post order event");
                return;
            }

            var activeHistory = history.ActiveTradeHistory();

            if (activeHistory == null
                || !activeHistory.Any())
            {
                return;
            }

            if (_decimalRangeRuleFilter.Type == RuleFilterType.None)
            {
                UpdatePassedFilterWithOrders(activeHistory);
                return;
            }

            var mostRecentTrade = activeHistory.Peek();

            var tradingHours = _tradingHoursService.GetTradingHoursForMic(mostRecentTrade.Market?.MarketIdentifierCode);
            if (!tradingHours.IsValid)
            {
                _logger.LogError($"Request for trading hours was invalid. MIC - {mostRecentTrade.Market?.MarketIdentifierCode}");

                UpdatePassedFilterWithOrders(activeHistory);
                return;
            }

            var marketTradedVolume = RetrieveMarketTradedVolume(mostRecentTrade, tradingHours, activeHistory);

            if (marketTradedVolume == null)
            {
                return;
            }

            var volumeTraded = activeHistory.Sum(_ => _.OrderFilledVolume ?? 0);

            if (marketTradedVolume <= 0)
            {
                _logger.LogInformation($"market traded volume was {marketTradedVolume} for {mostRecentTrade.Instrument.Identifiers}");

                UpdatePassedFilterWithOrders(activeHistory);
                return;
            }

            if (volumeTraded <= 0)
            {
                _logger.LogInformation($"market traded volume was {volumeTraded} for {mostRecentTrade.Instrument.Identifiers}");

                UpdatePassedFilterWithOrders(activeHistory);
                return;
            }

            var proportionOfTradedVolume = (volumeTraded / (decimal)marketTradedVolume);

            var passedFilter = false;

            if (_decimalRangeRuleFilter.Type == RuleFilterType.Include)
            {
                passedFilter =
                   (_decimalRangeRuleFilter.Max == null || _decimalRangeRuleFilter.Max == 1 || proportionOfTradedVolume <= _decimalRangeRuleFilter.Max)
                    && (_decimalRangeRuleFilter.Min == null || proportionOfTradedVolume >= _decimalRangeRuleFilter.Min);
            }
            else if (_decimalRangeRuleFilter.Type == RuleFilterType.Exclude)
            {
                passedFilter =
                    (_decimalRangeRuleFilter.Max == null || _decimalRangeRuleFilter.Max == 1 || proportionOfTradedVolume > _decimalRangeRuleFilter.Max)
                    || (_decimalRangeRuleFilter.Min == null || proportionOfTradedVolume < _decimalRangeRuleFilter.Min);
            }

            if (passedFilter)
            {
                UpdatePassedFilterWithOrders(activeHistory);
            }
        }

        private long? RetrieveMarketTradedVolume(Order mostRecentTrade, ITradingHours tradingHours, Stack<Order> activeHistory)
        {
            var marketDataRequest =
                new MarketDataRequest(
                    mostRecentTrade.Market?.MarketIdentifierCode,
                    mostRecentTrade.Instrument.Cfi,
                    mostRecentTrade.Instrument.Identifiers,
                    tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(BackwardWindowSize)),
                    tradingHours.ClosingInUtcForDay(UniverseDateTime),
                    RuleCtx.Id(),
                    _source);
            
            var hadMissingData = false;
            long? marketTradedVolume = null;

            switch (_source)
            {
                case DataSource.AllInterday:
                    var securityResultInterday = UniverseEquityInterdayCache.GetMarkets(marketDataRequest);
                    hadMissingData = securityResultInterday.HadMissingData;

                    if (!hadMissingData)
                        marketTradedVolume = InterdayMarketTradedVolume(securityResultInterday);

                    break;
                case DataSource.AllIntraday:
                    var securityResultIntraday = UniverseEquityIntradayCache.GetMarkets(marketDataRequest);
                    hadMissingData = securityResultIntraday.HadMissingData;

                    if (!hadMissingData)
                        marketTradedVolume = IntradayMarketTradedVolume(securityResultIntraday);

                    break;
            }

            if (hadMissingData && RunMode == RuleRunMode.ForceRun)
            {
                UpdatePassedFilterWithOrders(activeHistory);
                return null;
            }

            if (hadMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                _logger.LogInformation($"market traded volume was not calculable for {mostRecentTrade.Instrument.Identifiers} due to missing data");
                _hadMissingData = true;
                return null;
            }

            return marketTradedVolume;
        }

        private long IntradayMarketTradedVolume(MarketDataResponse<List<EquityInstrumentIntraDayTimeBar>> securityResult)
        {
            var marketTradedVolume = securityResult.Response.Sum(_ => _.SpreadTimeBar.Volume.Traded);

            return marketTradedVolume;
        }

        private long InterdayMarketTradedVolume(MarketDataResponse<List<EquityInstrumentInterDayTimeBar>> securityResult)
        {
            var marketTradedVolume = securityResult.Response.Sum(_ => _.DailySummaryTimeBar.DailyVolume.Traded);

            return marketTradedVolume;
        }

        public override void RunOrderFilledEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunPostOrderEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunInitialSubmissionEvent(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void RunInitialSubmissionEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        public override void RunOrderFilledEventDelayed(ITradingHistoryStack history)
        {
            // do nothing
        }

        protected override void MarketOpen(MarketOpenClose exchange)
        {
            _logger.LogInformation($"Market Open ({exchange?.MarketId}) occurred {exchange?.MarketOpen}");
        }

        protected override void MarketClose(MarketOpenClose exchange)
        {
            _logger.LogInformation("Market Close occurred");

            RunRuleForAllTradingHistoriesInMarket(exchange);
        }

        protected override void Genesis()
        {
            _logger.LogInformation("Genesis occurred");
        }

        protected override void EndOfUniverse()
        {
            if (_hadMissingData && RunMode == RuleRunMode.ValidationRun)
            {
                _dataRequestSubscriber.SubmitRequest();
            }

            _logger.LogInformation("Eschaton occurred");
        }

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
                if (UniverseEventsPassedFilter.Contains(order))
                {
                    continue;
                }

                UniverseEventsPassedFilter.Add(order);
            }
        }

        private void FlushFilterEvents()
        {
            UniverseEventsPassedFilter
                .RemoveWhere(_ =>
                    _ == null
                    || _.MostRecentDateEvent() < (UniverseDateTime - _eventExpiration));
        }
    }
}
