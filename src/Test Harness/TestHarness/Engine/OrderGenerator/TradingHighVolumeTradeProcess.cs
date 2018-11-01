using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Market;
using Domain.Market.Interfaces;
using Domain.Trades.Orders;
using NLog;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

namespace TestHarness.Engine.OrderGenerator
{
    /// <summary>
    /// Generate a bunch of high volume orders
    /// </summary>
    public class TradingHighVolumeTradeProcess : BaseTradingProcess
    {
        private readonly object _lock = new object();
        private readonly IReadOnlyCollection<string> _highVolumeTargetSedols;
        private bool _hasProcessedHighVolumeBreaches;
        private readonly IMarketHistoryStack _marketHistoryStack;
        private readonly TimeSpan _executePoint = TimeSpan.FromMinutes(65);

        private DateTime? _executeOn;

        public TradingHighVolumeTradeProcess(
            IReadOnlyCollection<string> cancelTargetSedols,
            ITradeStrategy<TradeOrderFrame> orderStrategy,
            ILogger logger)
            : base(logger, orderStrategy)
        {
            _highVolumeTargetSedols =
                cancelTargetSedols
                    ?.Where(cts => !string.IsNullOrWhiteSpace(cts))
                    ?.ToList()
                ?? new List<string>();
            _marketHistoryStack = new MarketHistoryStack(TimeSpan.FromHours(2));
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
                _executeOn = value.TimeStamp.Add(_executePoint);
                return;
            }

            if (_hasProcessedHighVolumeBreaches)
            {
                return;
            }

            lock (_lock)
            {
                if (_hasProcessedHighVolumeBreaches)
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
                foreach (var sedol in _highVolumeTargetSedols)
                {
                    switch (i)
                    {
                        case 0:
                            CreateHighVolumeTradesForWindowBreachInSedol(sedol, activeItems, 0.8m);
                            break;
                        case 1:
                            CreateHighVolumeTradesForDailyBreachInSedol(sedol, value, 0.8m);
                            break;
                        case 2:
                            CreateHighVolumeTradesForWindowBreachInSedol(sedol, activeItems, 0.6m);
                            break;
                        case 3:
                            CreateHighVolumeTradesForDailyBreachInSedol(sedol, value, 0.6m);
                            break;
                        case 4:
                            CreateHighVolumeTradesForWindowBreachInSedol(sedol, activeItems, 0.4m);
                            break;
                        case 5:
                            CreateHighVolumeTradesForDailyBreachInSedol(sedol, value, 0.4m);
                            break;
                    }
                    i++;
                }
                _hasProcessedHighVolumeBreaches = true;
            }
        }

        private void CreateHighVolumeTradesForDailyBreachInSedol(
            string sedol,
            ExchangeFrame frame,
            decimal percentageOfTraded)
        {
            if (string.IsNullOrWhiteSpace(sedol))
            {
                return;
            }

            var securities =
                frame
                    .Securities.FirstOrDefault(sec =>
                        string.Equals(
                            sec?.Security.Identifiers.Sedol,
                            sedol,
                            StringComparison.InvariantCultureIgnoreCase));
            
            if (securities == null)
            {
                return;
            }

            var tradedVolume = securities.DailyVolume.Traded;
            var volumeForBreachesToTrade = (((decimal)tradedVolume * percentageOfTraded) + 1) * 0.2m;

            for (var i = 0; i < 5; i++)
            {
                var volumeFrame = new TradeOrderFrame(
                    OrderType.Market,
                    securities.Market,
                    securities.Security,
                    new Price(securities.Spread.Price.Value * 1.05m, securities.Spread.Price.Currency),
                    new Price(securities.Spread.Price.Value * 1.05m, securities.Spread.Price.Currency),
                    (int)volumeForBreachesToTrade,
                    (int)volumeForBreachesToTrade,
                    OrderPosition.Buy,
                    OrderStatus.Fulfilled,
                    securities.TimeStamp.AddSeconds(30 * i),
                    securities.TimeStamp.AddSeconds(30 * i),
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

        private void CreateHighVolumeTradesForWindowBreachInSedol(
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

            var tradedVolume = securities.Sum(sec => sec.Volume.Traded);
            var headSecurity = securities.FirstOrDefault();
            var volumeForBreachesToTrade = (((decimal)tradedVolume * percentageOfTraded) +1) * 0.2m;

            for (var i = 0; i < 5; i++)
            {
                var volumeFrame = new TradeOrderFrame(
                    OrderType.Market,
                    headSecurity.Market,
                    headSecurity.Security,
                    new Price(headSecurity.Spread.Price.Value * 1.05m, headSecurity.Spread.Price.Currency),
                    new Price(headSecurity.Spread.Price.Value * 1.05m, headSecurity.Spread.Price.Currency),
                    (int)volumeForBreachesToTrade,
                    (int)volumeForBreachesToTrade,
                    OrderPosition.Buy,
                    OrderStatus.Fulfilled,
                    headSecurity.TimeStamp.AddSeconds(30 * i),
                    headSecurity.TimeStamp.AddSeconds(30 * i),
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

        protected override void _TerminateTradingStrategy()
        { }
    }
}
