namespace TestHarness.Engine.OrderGenerator
{
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

    using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

    /// <summary>
    ///     Generate a bunch of high volume orders
    /// </summary>
    public class TradingHighVolumeTradeProcess : BaseTradingProcess
    {
        private readonly TimeSpan _executePoint = TimeSpan.FromMinutes(65);

        private readonly IReadOnlyCollection<string> _highVolumeTargetSedols;

        private readonly IEquityIntraDayHistoryStack _intraDayHistoryStack;

        private readonly object _lock = new object();

        private DateTime? _executeOn;

        private bool _hasProcessedHighVolumeBreaches;

        public TradingHighVolumeTradeProcess(
            IReadOnlyCollection<string> cancelTargetSedols,
            ITradeStrategy<Order> orderStrategy,
            ILogger logger)
            : base(logger, orderStrategy)
        {
            this._highVolumeTargetSedols = cancelTargetSedols?.Where(cts => !string.IsNullOrWhiteSpace(cts))?.ToList()
                                           ?? new List<string>();
            this._intraDayHistoryStack = new EquityIntraDayHistoryStack(TimeSpan.FromHours(2));
        }

        public override void OnNext(EquityIntraDayTimeBarCollection value)
        {
            if (value == null) return;

            this._intraDayHistoryStack.Add(value, value.Epoch);

            if (this._executeOn == null)
            {
                this._executeOn = value.Epoch.Add(this._executePoint);
                return;
            }

            if (this._hasProcessedHighVolumeBreaches) return;

            lock (this._lock)
            {
                if (this._hasProcessedHighVolumeBreaches) return;

                if (value.Epoch < this._executeOn.Value) return;

                this._intraDayHistoryStack.ArchiveExpiredActiveItems(value.Epoch);
                var activeItems = this._intraDayHistoryStack.ActiveMarketHistory();

                var i = 0;
                foreach (var sedol in this._highVolumeTargetSedols)
                {
                    switch (i)
                    {
                        case 0:
                            this.CreateHighVolumeTradesForWindowBreachInSedol(sedol, activeItems, 0.8m);
                            break;
                        case 1:
                            this.CreateHighVolumeTradesForDailyBreachInSedol(sedol, value, 0.8m);
                            break;
                        case 2:
                            this.CreateHighVolumeTradesForWindowBreachInSedol(sedol, activeItems, 0.6m);
                            break;
                        case 3:
                            this.CreateHighVolumeTradesForDailyBreachInSedol(sedol, value, 0.6m);
                            break;
                        case 4:
                            this.CreateHighVolumeTradesForWindowBreachInSedol(sedol, activeItems, 0.4m);
                            break;
                        case 5:
                            this.CreateHighVolumeTradesForDailyBreachInSedol(sedol, value, 0.4m);
                            break;
                    }

                    i++;
                }

                this._hasProcessedHighVolumeBreaches = true;
            }
        }

        protected override void _InitiateTrading()
        {
        }

        protected override void _TerminateTradingStrategy()
        {
        }

        private void CreateHighVolumeTradesForDailyBreachInSedol(
            string sedol,
            EquityIntraDayTimeBarCollection frame,
            decimal percentageOfTraded)
        {
            if (string.IsNullOrWhiteSpace(sedol)) return;

            var securities = frame.Securities.FirstOrDefault(
                sec => string.Equals(
                    sec?.Security.Identifiers.Sedol,
                    sedol,
                    StringComparison.InvariantCultureIgnoreCase));

            if (securities == null) return;

            var tradedVolume = securities.DailySummaryTimeBar.DailyVolume.Traded;
            var volumeForBreachesToTrade = (tradedVolume * percentageOfTraded + 1) * 0.2m;

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
                    (int)volumeForBreachesToTrade,
                    (int)volumeForBreachesToTrade,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    OptionEuropeanAmerican.NONE,
                    new DealerOrder[0]);

                this.TradeStream.Add(volume);
            }
        }

        private void CreateHighVolumeTradesForWindowBreachInSedol(
            string sedol,
            Stack<EquityIntraDayTimeBarCollection> frames,
            decimal percentageOfTraded)
        {
            if (string.IsNullOrWhiteSpace(sedol)) return;

            var securities = frames.SelectMany(
                frame => frame.Securities.Where(
                    sec => string.Equals(
                        sec?.Security.Identifiers.Sedol,
                        sedol,
                        StringComparison.InvariantCultureIgnoreCase))).ToList();

            if (!securities.Any()) return;

            var tradedVolume = securities.Sum(sec => sec.SpreadTimeBar.Volume.Traded);
            var headSecurity = securities.FirstOrDefault();
            var volumeForBreachesToTrade = (tradedVolume * percentageOfTraded + 1) * 0.2m;

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
                    new Money(
                        headSecurity.SpreadTimeBar.Price.Value * 1.05m,
                        headSecurity.SpreadTimeBar.Price.Currency),
                    new Money(
                        headSecurity.SpreadTimeBar.Price.Value * 1.05m,
                        headSecurity.SpreadTimeBar.Price.Currency),
                    (int)volumeForBreachesToTrade,
                    (int)volumeForBreachesToTrade,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    OptionEuropeanAmerican.NONE,
                    new DealerOrder[0]);

                this.TradeStream.Add(volume);
            }
        }
    }
}