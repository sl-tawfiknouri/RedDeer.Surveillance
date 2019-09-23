namespace TestHarness.Engine.OrderGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Interfaces;
    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading.Orders;

    using Microsoft.Extensions.Logging;

    using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;
    using TestHarness.Engine.Plans;

    public class TradingHighProfitProcess : BaseTradingProcess
    {
        private readonly IIntraDayHistoryStack _intraDayHistoryStack;

        private readonly object _lock = new object();

        private readonly IReadOnlyCollection<DataGenerationPlan> _plan;

        public TradingHighProfitProcess(
            IReadOnlyCollection<DataGenerationPlan> plan,
            ITradeStrategy<Order> orderStrategy,
            ILogger logger)
            : base(logger, orderStrategy)
        {
            this._intraDayHistoryStack = new IntraDayHistoryStack(TimeSpan.FromHours(1));
            this._plan = plan ?? new DataGenerationPlan[0];
        }

        public override void OnNext(EquityIntraDayTimeBarCollection value)
        {
            if (value == null) return;

            if (!this._plan?.Any() ?? true) return;

            this._intraDayHistoryStack.Add(value, value.Epoch);

            var plan = this.PlanInDateRange(value);
            if (plan == null) return;

            lock (this._lock)
            {
                this._intraDayHistoryStack.ArchiveExpiredActiveItems(value.Epoch);
                var activeItems = this._intraDayHistoryStack.ActiveMarketHistory();

                if (plan.EquityInstructions.TerminationInUtc == value.Epoch)
                {
                    this.CreateHighProfitTradesForWindowBreachInSedol(plan.Sedol, activeItems, value, false);
                    this.CreateHighProfitTradesForWindowBreachInSedol(plan.Sedol, activeItems, value, true);
                }
                else
                {
                    this.CreateHighProfitTradesForWindowBreachInSedol(plan.Sedol, activeItems, value, false);
                }
            }
        }

        protected override void _InitiateTrading()
        {
        }

        protected override void _TerminateTradingStrategy()
        {
        }

        private void CreateHighProfitTradesForWindowBreachInSedol(
            string sedol,
            Stack<EquityIntraDayTimeBarCollection> frames,
            EquityIntraDayTimeBarCollection latestFrame,
            bool sellTrade)
        {
            if (string.IsNullOrWhiteSpace(sedol)) return;

            var securities = frames.SelectMany(
                frame => frame.Securities.Where(
                    sec => string.Equals(
                        sec?.Security.Identifiers.Sedol,
                        sedol,
                        StringComparison.InvariantCultureIgnoreCase))).ToList();

            var headSecurity = latestFrame.Securities.FirstOrDefault(
                fram => string.Equals(
                    fram.Security.Identifiers.Sedol,
                    sedol,
                    StringComparison.InvariantCultureIgnoreCase));

            if (!securities.Any()) return;

            var tradedVolume = securities.Sum(sec => sec.SpreadTimeBar.Volume.Traded);

            // select a suitably low % of the traded volume so we don't fire a huge amount of other rules =)
            tradedVolume = (int)(tradedVolume * 0.05m);
            var tradeTime = latestFrame.Epoch;

            var volume = new Order(
                headSecurity.Security,
                headSecurity.Market,
                null,
                Guid.NewGuid().ToString(),
                DateTime.UtcNow,
                "order-v1",
                "order-v1",
                "order-v1",
                sellTrade ? tradeTime.AddMinutes(1) : tradeTime,
                sellTrade ? tradeTime.AddMinutes(1) : tradeTime,
                null,
                null,
                null,
                sellTrade ? tradeTime.AddMinutes(1) : tradeTime,
                OrderTypes.MARKET,
                sellTrade ? OrderDirections.SELL : OrderDirections.BUY,
                headSecurity.SpreadTimeBar.Price.Currency,
                headSecurity.SpreadTimeBar.Price.Currency,
                OrderCleanDirty.NONE,
                null,
                headSecurity.SpreadTimeBar.Price,
                headSecurity.SpreadTimeBar.Price,
                (int)tradedVolume,
                (int)tradedVolume,
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

        private DataGenerationPlan PlanInDateRange(EquityIntraDayTimeBarCollection value)
        {
            if (!this._plan?.Any() ?? true) return null;

            if (value == null) return null;

            foreach (var plan in this._plan)
                if (plan.EquityInstructions.CommencementInUtc <= value.Epoch
                    && plan.EquityInstructions.TerminationInUtc >= value.Epoch)
                    return plan;

            return null;
        }
    }
}