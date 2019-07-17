using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Trading.Orders;
using Microsoft.Extensions.Logging;
using SharedKernel.Contracts.Markets;
using Surveillance.Auditing.Context.Interfaces;
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
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IMarketTradingHoursService _tradingHoursService;
        private readonly ILogger<HighVolumeVenueFilter> _logger;
        private readonly TimeSpan _eventExpiration;

        public HighVolumeVenueFilter(
            TimeWindows timeWindows,
            DecimalRangeRuleFilter decimalRangeRuleFilter,
            IUniverseOrderFilter universeOrderFilter,
            ISystemProcessOperationRunRuleContext runRuleContext,
            IUniverseMarketCacheFactory universeMarketCacheFactory,
            RuleRunMode ruleRunMode,
            IMarketTradingHoursService marketTradingHoursService,
            ILogger baseLogger,
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
                baseLogger,
                stackLogger)
        {
            _eventExpiration = BackwardWindowSize + BackwardWindowSize + TimeSpan.FromMinutes(1);
            _tradingHoursService = marketTradingHoursService ?? throw new ArgumentNullException(nameof(marketTradingHoursService));
            _decimalRangeRuleFilter = decimalRangeRuleFilter ?? DecimalRangeRuleFilter.None();
            _orderFilter = universeOrderFilter ?? throw new ArgumentNullException(nameof(universeOrderFilter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            var marketDataRequest =
                new MarketDataRequest(
                    mostRecentTrade.Market?.MarketIdentifierCode,
                    mostRecentTrade.Instrument.Cfi,
                    mostRecentTrade.Instrument.Identifiers,
                    tradingHours.OpeningInUtcForDay(UniverseDateTime.Subtract(BackwardWindowSize)),
                    tradingHours.ClosingInUtcForDay(UniverseDateTime),
                    RuleCtx.Id(),
                    DataSource.AllIntraday);

            var securityResult = UniverseEquityIntradayCache.GetMarkets(marketDataRequest);

            if (securityResult.HadMissingData)
            {
                UpdatePassedFilterWithOrders(activeHistory);
                return;
            }

            var marketTradedVolume = securityResult.Response.Sum(_ => _.SpreadTimeBar.Volume.Traded);
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

            if (proportionOfTradedVolume < 0)
            {
                UpdatePassedFilterWithOrders(activeHistory);

                throw new InvalidOperationException($"HighVolumeVenueFilter ended up with a proportion of traded volume that was negative {proportionOfTradedVolume} vol traded {volumeTraded} market traded vol {marketTradedVolume}");
            }

            var passedFilter = false;

            if (_decimalRangeRuleFilter.Type == RuleFilterType.Include)
            {
                passedFilter =
                    proportionOfTradedVolume <= _decimalRangeRuleFilter.Max.GetValueOrDefault(1)
                    && proportionOfTradedVolume >= _decimalRangeRuleFilter.Min.GetValueOrDefault(0);
            }
            else if (_decimalRangeRuleFilter.Type == RuleFilterType.Exclude)
            {
                 passedFilter =
                    proportionOfTradedVolume > _decimalRangeRuleFilter.Max.GetValueOrDefault(1)
                    || proportionOfTradedVolume < _decimalRangeRuleFilter.Min.GetValueOrDefault(0);
            }

            if (passedFilter)
            {
                UpdatePassedFilterWithOrders(activeHistory);
            }
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
                    || _.MostRecentDateEvent() < (UniverseDateTime + _eventExpiration));
        }
    }
}
