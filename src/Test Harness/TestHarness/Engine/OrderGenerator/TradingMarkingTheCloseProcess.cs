using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Market;
using Domain.Market.Interfaces;
using Domain.Trades.Orders;
using NLog;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    public class TradingMarkingTheCloseProcess : BaseTradingProcess
    {
        private readonly object _lock = new object();
        private readonly IReadOnlyCollection<string> _markingTheCloseTargetSedols;
        private readonly IMarketHistoryStack _marketHistoryStack;
        private readonly ExchangeDto _marketDto;

        private bool _hasProcessedMarkingTheCloseRuleBreaches;
        private DateTime? _executeOn;

        public TradingMarkingTheCloseProcess(
            IReadOnlyCollection<string> _markingtheCloseTargetSedols,
            ITradeStrategy<TradeOrderFrame> orderStrategy,
            ExchangeDto marketDto,
            ILogger logger)
            : base(logger, orderStrategy)
        {
            _markingTheCloseTargetSedols =
                _markingtheCloseTargetSedols
                    ?.Where(cts => !string.IsNullOrWhiteSpace(cts))
                    ?.ToList()
                ?? new List<string>();
            _marketHistoryStack = new MarketHistoryStack(TimeSpan.FromMinutes(29));
            _marketDto = marketDto ?? throw new ArgumentNullException(nameof(marketDto));
        }

        protected override void _InitiateTrading()
        { }

        public override void OnNext(ExchangeFrame value)
        {
            if (value == null)
            {
                return;
            }

            _marketHistoryStack.Add(value, value.TimeStamp);

            if (_executeOn == null)
            {
                _executeOn = value.TimeStamp.Date.Add(_marketDto.MarketCloseTime).Add(-TimeSpan.FromMinutes(15));
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

                if (value.TimeStamp < _executeOn.Value)
                {
                    return;
                }
                
                _marketHistoryStack.ArchiveExpiredActiveItems(value.TimeStamp);
                var activeItems = _marketHistoryStack.ActiveMarketHistory();

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
            Stack<ExchangeFrame> frames,
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
            var tradedVolume = securities.Sum(sec => sec.Volume.Traded);
            var headSecurity = securities.FirstOrDefault();
            var volumeForBreachesToTrade = (((decimal)tradedVolume * percentageOfTraded) + 1) * 0.18m;

            for (var i = 0; i < 6; i++)
            {
                var timeOffset = _marketDto.MarketCloseTime.Add(-TimeSpan.FromMinutes((2 * i) + 1));
                var tradeTime = headSecurity.TimeStamp.Date.Add(timeOffset);

                var volumeFrame = new TradeOrderFrame(
                    OrderType.Market,
                    headSecurity.Market,
                    headSecurity.Security,
                    new Price(headSecurity.Spread.Price.Value, headSecurity.Spread.Price.Currency),
                    new Price(headSecurity.Spread.Price.Value, headSecurity.Spread.Price.Currency),
                    (int)volumeForBreachesToTrade,
                    (int)volumeForBreachesToTrade,
                    OrderPosition.Buy,
                    OrderStatus.Fulfilled,
                    tradeTime,
                    tradeTime,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    headSecurity.Spread.Price.Currency);

                TradeStream.Add(volumeFrame);
            }
        }

        private void CreateMarkingTheCloseTradesForDailyBreachInSedol(
            string sedol,
            Stack<ExchangeFrame> frames,
            decimal percentageOfTraded)
        {
            if (string.IsNullOrWhiteSpace(sedol))
            {
                return;
            }

            var securities =
                frames
                    .OrderByDescending(i => i.TimeStamp)
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
                         StringComparison.InvariantCultureIgnoreCase))?.Volume.Traded ?? 0);

            if (securities == null)
            {
                return;
            }

            var tradedVolume = securities.DailyVolume.Traded;
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

                var volumeFrame = new TradeOrderFrame(
                    OrderType.Market,
                    securities.Market,
                    securities.Security,
                    new Price(securities.Spread.Price.Value, securities.Spread.Price.Currency),
                    new Price(securities.Spread.Price.Value, securities.Spread.Price.Currency),
                    (int)finalVolumeForBreachestoTrade,
                    (int)finalVolumeForBreachestoTrade,
                    OrderPosition.Buy,
                    OrderStatus.Fulfilled,
                    tradeTime,
                    tradeTime,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    securities.Spread.Price.Currency);

                TradeStream.Add(volumeFrame);
            }
        }

        protected override void _TerminateTradingStrategy()
        { }

    }
}
