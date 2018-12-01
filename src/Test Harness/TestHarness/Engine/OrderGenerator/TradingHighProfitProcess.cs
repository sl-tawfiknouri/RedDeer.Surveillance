using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity.Frames;
using Domain.Market;
using Domain.Market.Interfaces;
using Domain.Trades.Orders;
using NLog;
using TestHarness.Engine.OrderGenerator.Strategies.Interfaces;
using TestHarness.Engine.Plans;

namespace TestHarness.Engine.OrderGenerator
{
    public class TradingHighProfitProcess : BaseTradingProcess
    {
        private readonly object _lock = new object();
        private readonly IMarketHistoryStack _marketHistoryStack;
        private readonly IReadOnlyCollection<DataGenerationPlan> _plan;

        public TradingHighProfitProcess(
            IReadOnlyCollection<DataGenerationPlan> plan,
            ITradeStrategy<TradeOrderFrame> orderStrategy,
            ILogger logger)
            : base(logger, orderStrategy)
        {

            _marketHistoryStack = new MarketHistoryStack(TimeSpan.FromHours(1));
            _plan = plan ?? new DataGenerationPlan[0];
        }

        protected override void _InitiateTrading()
        { }

        public override void OnNext(ExchangeFrame value)
        {
            if (value == null)
            {
                return;
            }

            if (!_plan?.Any() ?? true)
            {
                return;
            }

            _marketHistoryStack.Add(value, value.TimeStamp);

            var plan = PlanInDateRange(value);
            if (plan == null)
            {
                return;
            }

            lock (_lock)
            {

                _marketHistoryStack.ArchiveExpiredActiveItems(value.TimeStamp);
                var activeItems = _marketHistoryStack.ActiveMarketHistory();

                if (plan.EquityInstructions.TerminationInUtc == value.TimeStamp)
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

        private DataGenerationPlan PlanInDateRange(ExchangeFrame value)
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
                if (plan.EquityInstructions.CommencementInUtc <= value.TimeStamp
                    && plan.EquityInstructions.TerminationInUtc >= value.TimeStamp)
                {
                    return plan;
                }
            }

            return null;
        }

        private void CreateHighProfitTradesForWindowBreachInSedol(
            string sedol,
            Stack<ExchangeFrame> frames,
            ExchangeFrame latestFrame,
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

            var tradedVolume = securities.Sum(sec => sec.Volume.Traded);

            // select a suitably low % of the traded volume so we don't fire a huge amount of other rules =)
            tradedVolume = (int)((decimal)tradedVolume * 0.05m);
            var tradeTime = latestFrame.TimeStamp;

            var volumeFrame = new TradeOrderFrame(
                null,
                OrderType.Market,
                headSecurity.Market,
                headSecurity.Security,
                headSecurity.Spread.Price,
                headSecurity.Spread.Price,
                (int)tradedVolume,
                (int)tradedVolume,
                sellTrade ? OrderPosition.Sell : OrderPosition.Buy,
                OrderStatus.Fulfilled,
                sellTrade ? tradeTime.AddMinutes(1) : tradeTime,
                sellTrade ? tradeTime.AddMinutes(1) : tradeTime,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                headSecurity.Spread.Price.Currency.Value);

            TradeStream.Add(volumeFrame);
        }

        protected override void _TerminateTradingStrategy()
        { }
    }
}
