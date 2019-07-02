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

namespace TestHarness.Engine.OrderGenerator
{
    public class TradingHighProfitProcess : BaseTradingProcess
    {
        private readonly object _lock = new object();
        private readonly IIntraDayHistoryStack _intraDayHistoryStack;
        private readonly IReadOnlyCollection<DataGenerationPlan> _plan;

        public TradingHighProfitProcess(
            IReadOnlyCollection<DataGenerationPlan> plan,
            ITradeStrategy<Order> orderStrategy,
            ILogger logger)
            : base(logger, orderStrategy)
        {

            _intraDayHistoryStack = new IntraDayHistoryStack(TimeSpan.FromHours(1));
            _plan = plan ?? new DataGenerationPlan[0];
        }

        protected override void _InitiateTrading()
        { }

        public override void OnNext(EquityIntraDayTimeBarCollection value)
        {
            if (value == null)
            {
                return;
            }

            if (!_plan?.Any() ?? true)
            {
                return;
            }

            _intraDayHistoryStack.Add(value, value.Epoch);

            var plan = PlanInDateRange(value);
            if (plan == null)
            {
                return;
            }

            lock (_lock)
            {

                _intraDayHistoryStack.ArchiveExpiredActiveItems(value.Epoch);
                var activeItems = _intraDayHistoryStack.ActiveMarketHistory();

                if (plan.EquityInstructions.TerminationInUtc == value.Epoch)
                {
                    CreateHighProfitTradesForWindowBreachInSedol(plan.Sedol, activeItems, value, false);
                    CreateHighProfitTradesForWindowBreachInSedol(plan.Sedol, activeItems, value, true);
                }
                else
                {
                    CreateHighProfitTradesForWindowBreachInSedol(plan.Sedol, activeItems, value, false);
                }
            }
        }

        private DataGenerationPlan PlanInDateRange(EquityIntraDayTimeBarCollection value)
        {
            if (!_plan?.Any() ?? true)
            {
                return null;
            }

            if (value == null)
            {
                return null;
            }

            foreach (var plan in _plan)
            {
                if (plan.EquityInstructions.CommencementInUtc <= value.Epoch
                    && plan.EquityInstructions.TerminationInUtc >= value.Epoch)
                {
                    return plan;
                }
            }

            return null;
        }

        private void CreateHighProfitTradesForWindowBreachInSedol(
            string sedol,
            Stack<EquityIntraDayTimeBarCollection> frames,
            EquityIntraDayTimeBarCollection latestFrame,
            bool sellTrade)
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

            var headSecurity =
                latestFrame
                    .Securities
                    .FirstOrDefault(fram =>
                        string.Equals(
                            fram.Security.Identifiers.Sedol,
                            sedol,
                            StringComparison.InvariantCultureIgnoreCase));

            if (!securities.Any())
            {
                return;
            }

            var tradedVolume = securities.Sum(sec => sec.SpreadTimeBar.Volume.Traded);

            // select a suitably low % of the traded volume so we don't fire a huge amount of other rules =)
            tradedVolume = (int)((decimal)tradedVolume * 0.05m);
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
                (int) tradedVolume,
                (int) tradedVolume,
                null,
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

        protected override void _TerminateTradingStrategy()
        { }
    }
}
