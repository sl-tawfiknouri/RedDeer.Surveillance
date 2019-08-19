namespace TestHarness.Engine.OrderGenerator
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
    using TestHarness.Engine.Plans;

    public class TradingWashTradeProcess : BaseTradingProcess
    {
        private readonly object _lock = new object();

        private readonly DataGenerationPlan _plan;

        private readonly DateTime _thirdGroupActivation;

        private bool _initialCluster;

        private bool _secondaryCluster;

        private bool _thirdCluster;

        public TradingWashTradeProcess(ITradeStrategy<Order> orderStrategy, DataGenerationPlan plan, ILogger logger)
            : base(logger, orderStrategy)
        {
            this._plan = plan;
            this._thirdGroupActivation = this._plan.EquityInstructions.TerminationInUtc.AddHours(4);
        }

        public override void OnNext(EquityIntraDayTimeBarCollection value)
        {
            if (value == null) return;

            if (this._plan == null) return;

            lock (this._lock)
            {
                if (!this._initialCluster && this._plan.EquityInstructions.CommencementInUtc <= value.Epoch)
                {
                    this.WashTradeInSecurityWithClustering(this._plan.Sedol, value, 20);

                    this._initialCluster = true;
                    return;
                }

                if (!this._secondaryCluster && this._plan.EquityInstructions.TerminationInUtc <= value.Epoch)
                {
                    this.WashTradeInSecurityWithClustering(this._plan.Sedol, value, 30);

                    this._secondaryCluster = true;
                    return;
                }

                if (!this._thirdCluster && this._thirdGroupActivation <= value.Epoch)
                {
                    this.WashTradeInSecurityWithClustering(this._plan.Sedol, value, 20);

                    this._thirdCluster = true;
                }
            }
        }

        protected override void _InitiateTrading()
        {
        }

        protected override void _TerminateTradingStrategy()
        {
        }

        private void WashTradeInSecurityWithClustering(
            string sedol,
            EquityIntraDayTimeBarCollection value,
            int clusterSize)
        {
            if (value == null) return;

            var correctSecurity = value.Securities.Where(
                sec => string.Equals(
                    sec.Security?.Identifiers.Sedol,
                    sedol,
                    StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (!correctSecurity.Any()) return;

            var security = correctSecurity.FirstOrDefault();

            var splitSize = clusterSize / 2;
            var frames = new List<Order>();
            for (var i = 0; i < clusterSize; i++)
            {
                var frame2 = new Order(
                    security.Security,
                    security.Market,
                    null,
                    Guid.NewGuid().ToString(),
                    DateTime.UtcNow,
                    "version-1",
                    "version-1",
                    "version-1",
                    value.Epoch,
                    value.Epoch,
                    null,
                    null,
                    null,
                    value.Epoch.AddSeconds(i),
                    OrderTypes.LIMIT,
                    i < splitSize ? OrderDirections.SELL : OrderDirections.BUY,
                    new Currency("GBP"),
                    new Currency("GBP"),
                    OrderCleanDirty.NONE,
                    null,
                    new Money(security.SpreadTimeBar.Price.Value * 1.05m, security.SpreadTimeBar.Price.Currency),
                    new Money(security.SpreadTimeBar.Price.Value * 1.05m, security.SpreadTimeBar.Price.Currency),
                    (int)(security.DailySummaryTimeBar.DailyVolume.Traded * 0.001m),
                    (int)(security.DailySummaryTimeBar.DailyVolume.Traded * 0.001m),
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    OptionEuropeanAmerican.NONE,
                    new DealerOrder[0]);

                frames.Add(frame2);
            }

            foreach (var trade in frames.OrderBy(i => i.MostRecentDateEvent())) this.TradeStream.Add(trade);
        }
    }
}