using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Financial;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Money;
using Domain.Core.Markets.Collections;
using Domain.Core.Markets.Interfaces;
using Domain.Core.Markets.Timebars;
using Domain.Core.Trading.Orders;
using Microsoft.Extensions.Logging;
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
        private readonly IIntraDayHistoryStack _intraDayHistoryStack;
        private readonly TimeSpan _executePoint = TimeSpan.FromMinutes(65);

        private DateTime? _executeOn;

        public TradingHighVolumeTradeProcess(
            IReadOnlyCollection<string> cancelTargetSedols,
            ITradeStrategy<Order> orderStrategy,
            ILogger logger)
            : base(logger, orderStrategy)
        {
            _highVolumeTargetSedols =
                cancelTargetSedols
                    ?.Where(cts => !string.IsNullOrWhiteSpace(cts))
                    ?.ToList()
                ?? new List<string>();
            _intraDayHistoryStack = new IntraDayHistoryStack(TimeSpan.FromHours(2));
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
                _executeOn = value.Epoch.Add(_executePoint);
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

                if (value.Epoch < _executeOn.Value)
                {
                    return;
                }

                _intraDayHistoryStack.ArchiveExpiredActiveItems(value.Epoch);
                var activeItems = _intraDayHistoryStack.ActiveMarketHistory();

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

        private void CreateHighVolumeTradesForWindowBreachInSedol(
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

            var tradedVolume = securities.Sum(sec => sec.SpreadTimeBar.Volume.Traded);
            var headSecurity = securities.FirstOrDefault();
            var volumeForBreachesToTrade = (((decimal)tradedVolume * percentageOfTraded) +1) * 0.2m;

            for (var i = 0; i < 5; i++)
            {
                var volume = new Order(
                    headSecurity.Security,
                    headSecurity.Market,
                    null,
                    Guid.NewGuid().ToString(),
                    DateTime.UtcNow,    
                    "order-v1",
                    "order-v1",
                    "order-v1",
                    headSecurity.TimeStamp.AddSeconds(30 * i),
                    headSecurity.TimeStamp.AddSeconds(30 * i),
                    null,
                    null,
                    null,
                    headSecurity.TimeStamp.AddSeconds(30 * i),
                    OrderTypes.MARKET,
                    OrderDirections.BUY,
                    headSecurity.SpreadTimeBar.Price.Currency,
                    headSecurity.SpreadTimeBar.Price.Currency,
                    OrderCleanDirty.NONE,
                    null,
                    new Money(headSecurity.SpreadTimeBar.Price.Value * 1.05m, headSecurity.SpreadTimeBar.Price.Currency),
                    new Money(headSecurity.SpreadTimeBar.Price.Value * 1.05m, headSecurity.SpreadTimeBar.Price.Currency),
                    (int)volumeForBreachesToTrade,
                    (int)volumeForBreachesToTrade,
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

        private void CreateHighVolumeTradesForDailyBreachInSedol(
            string sedol,
            EquityIntraDayTimeBarCollection frame,
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

            var tradedVolume = securities.DailySummaryTimeBar.DailyVolume.Traded;
            var volumeForBreachesToTrade = (((decimal)tradedVolume * percentageOfTraded) + 1) * 0.2m;

            for (var i = 0; i < 5; i++)
            {
                var volume = new Order(
                    securities.Security,
                    securities.Market,
                    null,
                    Guid.NewGuid().ToString(),
                    DateTime.UtcNow,
                    "order-v1",
                    "order-v1",
                    "order-v1",
                    securities.TimeStamp.AddSeconds(30 * i),
                    securities.TimeStamp.AddSeconds(30 * i),
                    null,
                    null,
                    null,
                    securities.TimeStamp.AddSeconds(30 * i),
                    OrderTypes.MARKET,
                    OrderDirections.BUY,
                    securities.SpreadTimeBar.Price.Currency,
                    securities.SpreadTimeBar.Price.Currency,
                    OrderCleanDirty.NONE,
                    null,
                    new Money(securities.SpreadTimeBar.Price.Value * 1.05m, securities.SpreadTimeBar.Price.Currency),
                    new Money(securities.SpreadTimeBar.Price.Value * 1.05m, securities.SpreadTimeBar.Price.Currency),
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

        protected override void _TerminateTradingStrategy()
        { }
    }
}
