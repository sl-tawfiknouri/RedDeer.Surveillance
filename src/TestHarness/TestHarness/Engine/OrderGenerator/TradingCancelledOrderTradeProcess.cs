﻿namespace TestHarness.Engine.OrderGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;

    /// <summary>
    ///     generate a bunch of cancelled orders (v2)
    /// </summary>
    public class TradingCancelledOrderTradeProcess : BaseTradingProcess
    {
        private readonly IReadOnlyCollection<string> _cancelTargetSedols;

        private readonly object _lock = new object();

        private readonly DateTime _triggerCount;

        private bool _hasProcessedCount;

        public TradingCancelledOrderTradeProcess(
            ITradeStrategy<Order> orderStrategy,
            IReadOnlyCollection<string> cancelTargetSedols,
            DateTime triggerCount,
            ILogger logger)
            : base(logger, orderStrategy)
        {
            this._cancelTargetSedols = cancelTargetSedols ?? new string[0];
            this._triggerCount = triggerCount;
        }

        public override void OnNext(EquityIntraDayTimeBarCollection value)
        {
            if (value == null) return;

            if (this._hasProcessedCount) return;

            lock (this._lock)
            {
                if (this._hasProcessedCount) return;

                if (!this._hasProcessedCount && value.Epoch >= this._triggerCount)
                {
                    var i = 0;
                    foreach (var sedol in this._cancelTargetSedols)
                    {
                        if (string.IsNullOrWhiteSpace(sedol)) continue;

                        switch (i)
                        {
                            case 0:
                                this.CancelForSedolByCount(sedol, value, 8);
                                break;
                            case 1:
                                this.CancelForSedolByPosition(sedol, value, 0.8m);
                                break;
                            case 2:
                                this.CancelForSedolByCount(sedol, value, 6);
                                break;
                            case 3:
                                this.CancelForSedolByPosition(sedol, value, 0.6m);
                                break;
                            case 4:
                                this.CancelForSedolByCount(sedol, value, 4);
                                break;
                            case 5:
                                this.CancelForSedolByPosition(sedol, value, 0.4m);
                                break;
                        }

                        i++;
                    }

                    this._hasProcessedCount = true;
                }
            }
        }

        protected override void _InitiateTrading()
        {
        }

        protected override void _TerminateTradingStrategy()
        {
        }

        private void CancelForSedolByCount(string sedol, EquityIntraDayTimeBarCollection value, int amountToCancel)
        {
            if (value == null) return;

            var correctSecurity = value.Securities.Where(
                sec => string.Equals(
                    sec.Security?.Identifiers.Sedol,
                    sedol,
                    StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (!correctSecurity.Any()) return;

            var security = correctSecurity.FirstOrDefault();

            var frames = new List<Order>();
            for (var i = 0; i < 10; i++)
            {
                var frame = new Order(
                    security.Security,
                    security.Market,
                    null,
                    Guid.NewGuid().ToString(),
                    DateTime.UtcNow,
                    "order-v1",
                    "order-v1",
                    "order-group-1",
                    value.Epoch.AddMinutes(1),
                    value.Epoch.AddMinutes(1),
                    null,
                    null,
                    i < amountToCancel ? (DateTime?)value.Epoch.AddMinutes(1) : null,
                    null,
                    OrderTypes.MARKET,
                    OrderDirections.BUY,
                    new Currency("GBP"),
                    new Currency("GBP"),
                    OrderCleanDirty.NONE,
                    null,
                    new Money(security.SpreadTimeBar.Price.Value * 1.05m, security.SpreadTimeBar.Price.Currency),
                    new Money(security.SpreadTimeBar.Price.Value * 1.05m, security.SpreadTimeBar.Price.Currency),
                    i < amountToCancel ? 0 : (int)(security.DailySummaryTimeBar.DailyVolume.Traded * 0.01m),
                    0,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    OptionEuropeanAmerican.NONE,
                    new DealerOrder[0]);

                frames.Add(frame);
            }

            foreach (var trade in frames.OrderBy(i => i.MostRecentDateEvent())) this.TradeStream.Add(trade);
        }

        private void CancelForSedolByPosition(
            string sedol,
            EquityIntraDayTimeBarCollection value,
            decimal positionPercentageToCancel)
        {
            if (value == null) return;

            var correctSecurity = value.Securities.Where(
                sec => string.Equals(
                    sec.Security?.Identifiers.Sedol,
                    sedol,
                    StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (!correctSecurity.Any()) return;

            var security = correctSecurity.FirstOrDefault();
            var frames = new List<Order>();

            var totalPurchase = security.DailySummaryTimeBar.DailyVolume.Traded * 0.1m;
            var initialBuyShare = totalPurchase * positionPercentageToCancel;
            var splitShare = (totalPurchase - initialBuyShare) * (1m / 9m) - 1;

            var cancelledFrame = new Order(
                security.Security,
                security.Market,
                null,
                Guid.NewGuid().ToString(),
                DateTime.UtcNow,
                "order-v1",
                "order-v1",
                "order-group-1",
                value.Epoch,
                value.Epoch,
                null,
                null,
                value.Epoch,
                null,
                OrderTypes.MARKET,
                OrderDirections.BUY,
                new Currency("GBP"),
                new Currency("USD"),
                OrderCleanDirty.CLEAN,
                null,
                new Money(security.SpreadTimeBar.Price.Value * 1.05m, security.SpreadTimeBar.Price.Currency),
                new Money(security.SpreadTimeBar.Price.Value * 1.05m, security.SpreadTimeBar.Price.Currency),
                (int)initialBuyShare,
                (int)initialBuyShare,
                "trader-1",
                "trader one",
                "clearing-agent",
                "dealing-instructions",
                new OrderBroker(string.Empty, string.Empty, "Mr Broker", DateTime.Now, true),
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new DealerOrder[0]);

            frames.Add(cancelledFrame);

            for (var i = 1; i < 10; i++)
            {
                var frame = new Order(
                    security.Security,
                    security.Market,
                    null,
                    Guid.NewGuid().ToString(),
                    DateTime.UtcNow,
                    "order-v1",
                    "order-v1",
                    "order-group-1",
                    value.Epoch.AddMinutes(1),
                    value.Epoch.AddMinutes(1),
                    null,
                    null,
                    value.Epoch.AddMinutes(1),
                    null,
                    OrderTypes.MARKET,
                    OrderDirections.BUY,
                    new Currency("GBP"),
                    new Currency("USD"),
                    OrderCleanDirty.CLEAN,
                    null,
                    new Money(security.SpreadTimeBar.Price.Value * 1.05m, security.SpreadTimeBar.Price.Currency),
                    new Money(security.SpreadTimeBar.Price.Value * 1.05m, security.SpreadTimeBar.Price.Currency),
                    (int)splitShare,
                    (int)splitShare,
                    "trader-1",
                    "trader one",
                    "clearing-agent",
                    "dealing-instructions",
                    new OrderBroker(string.Empty, string.Empty, "Mr Broker", DateTime.Now, true),
                    null,
                    null,
                    OptionEuropeanAmerican.NONE,
                    new DealerOrder[0]);

                frames.Add(frame);
            }

            foreach (var trade in frames.OrderBy(i => i.MostRecentDateEvent())) this.TradeStream.Add(trade);
        }
    }
}