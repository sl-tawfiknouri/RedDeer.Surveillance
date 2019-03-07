using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Money;
using Domain.Core.Markets.Collections;
using Domain.Core.Markets.Interfaces;
using Domain.Core.Markets.Timebars;
using Domain.Core.Trading.Orders;
using Microsoft.Extensions.Logging;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    public class TradingMarkingTheCloseProcess : BaseTradingProcess
    {
        private readonly object _lock = new object();
        private readonly IReadOnlyCollection<string> _markingTheCloseTargetSedols;
        private readonly IIntraDayHistoryStack _intraDayHistoryStack;
        private readonly ExchangeDto _marketDto;

        private bool _hasProcessedMarkingTheCloseRuleBreaches;
        private DateTime? _executeOn;

        public TradingMarkingTheCloseProcess(
            IReadOnlyCollection<string> markingTheCloseTargetSedols,
            ITradeStrategy<Order> orderStrategy,
            ExchangeDto marketDto,
            ILogger logger)
            : base(logger, orderStrategy)
        {
            _markingTheCloseTargetSedols =
                markingTheCloseTargetSedols
                    ?.Where(cts => !string.IsNullOrWhiteSpace(cts))
                    ?.ToList()
                ?? new List<string>();
            _intraDayHistoryStack = new IntraDayHistoryStack(TimeSpan.FromMinutes(29));
            _marketDto = marketDto ?? throw new ArgumentNullException(nameof(marketDto));
        }

        protected override void _InitiateTrading()
        { }

        public override void OnNext(EquityIntraDayTimeBarCollection value)
        {
            if (value == null)
            {
                return;
            }

            _intraDayHistoryStack.Add(value, value.Epoch);

            if (_executeOn == null)
            {
                _executeOn = value.Epoch.Date.Add(_marketDto.MarketCloseTime).Add(-TimeSpan.FromMinutes(15));
            }

            if (_hasProcessedMarkingTheCloseRuleBreaches)
            {
                return;
            }

            lock (_lock)
            {
                if (_hasProcessedMarkingTheCloseRuleBreaches)
                {
                    return;
                }

                if (value.Epoch < _executeOn.Value)
                {
                    return;
                }
                
                _intraDayHistoryStack.ArchiveExpiredActiveItems(value.Epoch);
                var activeItems = _intraDayHistoryStack.ActiveMarketHistory();

                var i = 0;
                foreach (var sedol in _markingTheCloseTargetSedols)
                {
                    switch (i)
                    {
                        case 0:
                            CreateMarkingTheCloseTradesForWindowBreachInSedol(sedol, activeItems, 0.2m);
                            break;
                        case 1:
                            CreateMarkingTheCloseTradesForDailyBreachInSedol(sedol, activeItems, 0.4m);
                            break;
                        case 2:
                            CreateMarkingTheCloseTradesForWindowBreachInSedol(sedol, activeItems, 0.15m);
                            break;
                        case 3:
                            CreateMarkingTheCloseTradesForDailyBreachInSedol(sedol, activeItems, 0.3m);
                            break;
                        case 4:
                            CreateMarkingTheCloseTradesForWindowBreachInSedol(sedol, activeItems, 0.1m);
                            break;
                        case 5:
                            CreateMarkingTheCloseTradesForDailyBreachInSedol(sedol, activeItems, 0.05m);
                            break;
                    }
                    i++;
                }
                _hasProcessedMarkingTheCloseRuleBreaches = true;
            }
        }

        private void CreateMarkingTheCloseTradesForWindowBreachInSedol(
            string sedol,
            Stack<EquityIntraDayTimeBarCollection> frames,
            decimal percentageOfTraded)
        {
            if (string.IsNullOrWhiteSpace(sedol))
            {
                return;
            }

            var securities =
                frames
                .SelectMany(frame =>
                    frame.Securities.Where(sec =>
                        string.Equals(
                            sec?.Security.Identifiers.Sedol,
                            sedol,
                            StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            if (!securities.Any())
            {
                return;
            }
            // _executeOn
            var tradedVolume = securities.Sum(sec => sec.SpreadTimeBar.Volume.Traded);
            var headSecurity = securities.FirstOrDefault();
            var volumeForBreachesToTrade = (((decimal)tradedVolume * percentageOfTraded) + 1) * 0.18m;

            for (var i = 0; i < 6; i++)
            {
                var timeOffset = _marketDto.MarketCloseTime.Add(-TimeSpan.FromMinutes((2 * i) + 1));
                var tradeTime = headSecurity.TimeStamp.Date.Add(timeOffset);

                var volume = new Order(
                    headSecurity.Security,
                    headSecurity.Market,
                    null,
                    Guid.NewGuid().ToString(),
                    DateTime.UtcNow,
                    "version-1",
                    "version-1",
                    "version-1",
                    tradeTime,
                    tradeTime,
                    null,
                    null,
                    null,
                    tradeTime,
                    OrderTypes.MARKET,
                    OrderDirections.BUY,
                    headSecurity.SpreadTimeBar.Price.Currency,
                    headSecurity.SpreadTimeBar.Price.Currency,
                    OrderCleanDirty.NONE,
                    null,
                    new Money(headSecurity.SpreadTimeBar.Price.Value, headSecurity.SpreadTimeBar.Price.Currency),
                    new Money(headSecurity.SpreadTimeBar.Price.Value, headSecurity.SpreadTimeBar.Price.Currency),
                    (int) volumeForBreachesToTrade,
                    (int) volumeForBreachesToTrade,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    OptionEuropeanAmerican.NONE,
                    new DealerOrder[0]);

                TradeStream.Add(volume);
            }
        }

        private void CreateMarkingTheCloseTradesForDailyBreachInSedol(
            string sedol,
            Stack<EquityIntraDayTimeBarCollection> frames,
            decimal percentageOfTraded)
        {
            if (string.IsNullOrWhiteSpace(sedol))
            {
                return;
            }

            var securities =
                frames
                    .OrderByDescending(i => i.Epoch)
                    .FirstOrDefault()
                    .Securities.FirstOrDefault(sec =>
                        string.Equals(
                            sec?.Security.Identifiers.Sedol,
                            sedol,
                            StringComparison.InvariantCultureIgnoreCase));

            var totalVolumeInWindow =
                frames.Sum(io => io.Securities.FirstOrDefault(sec =>
                     string.Equals(
                         sec?.Security.Identifiers.Sedol,
                         sedol,
                         StringComparison.InvariantCultureIgnoreCase))?.SpreadTimeBar.Volume.Traded ?? 0);

            if (securities == null)
            {
                return;
            }

            var tradedVolume = securities.DailySummaryTimeBar.DailyVolume.Traded;
            var volumeForBreachesToTrade = (((decimal) tradedVolume * percentageOfTraded) + 1);

            var adjustedVolumeForBreachesToTrade =
                volumeForBreachesToTrade > totalVolumeInWindow
                    ? totalVolumeInWindow
                    : volumeForBreachesToTrade;

            var finalVolumeForBreachestoTrade = adjustedVolumeForBreachesToTrade * 0.18m;

            for (var i = 0; i < 6; i++)
            {
                var timeOffset = _marketDto.MarketCloseTime.Add(-TimeSpan.FromMinutes((2 * i) + 1));
                var tradeTime = securities.TimeStamp.Date.Add(timeOffset);

                var volume = new Order(
                    securities.Security,
                    securities.Market,
                    null,
                    Guid.NewGuid().ToString(),
                    DateTime.UtcNow,
                    "version-1",
                    "version-1",
                    "version-1",
                    tradeTime,
                    tradeTime,
                    null,
                    null,
                    null,
                    tradeTime,
                    OrderTypes.MARKET,
                    OrderDirections.BUY,
                    securities.SpreadTimeBar.Price.Currency,
                    securities.SpreadTimeBar.Price.Currency,
                    OrderCleanDirty.NONE,
                    null,
                    new Money(securities.SpreadTimeBar.Price.Value, securities.SpreadTimeBar.Price.Currency),
                    new Money(securities.SpreadTimeBar.Price.Value, securities.SpreadTimeBar.Price.Currency),
                    (int)finalVolumeForBreachestoTrade,
                    (int)finalVolumeForBreachestoTrade,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    OptionEuropeanAmerican.NONE,
                    new DealerOrder[0]);

                TradeStream.Add(volume);
            }
        }

        protected override void _TerminateTradingStrategy()
        { }

    }
}
