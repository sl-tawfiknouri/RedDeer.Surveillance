﻿using System;
using System.Collections.Generic;
using System.Linq;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Financial.Interfaces;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
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
            ITradeStrategy<Order> orderStrategy,
            ILogger logger)
            : base(logger, orderStrategy)
        {

            _marketHistoryStack = new MarketHistoryStack(TimeSpan.FromHours(1));
            _plan = plan ?? new DataGenerationPlan[0];
        }

        protected override void _InitiateTrading()
        { }

        public override void OnNext(MarketTimeBarCollection value)
        {
            if (value == null)
            {
                return;
            }

            if (!_plan?.Any() ?? true)
            {
                return;
            }

            _marketHistoryStack.Add(value, value.Epoch);

            var plan = PlanInDateRange(value);
            if (plan == null)
            {
                return;
            }

            lock (_lock)
            {

                _marketHistoryStack.ArchiveExpiredActiveItems(value.Epoch);
                var activeItems = _marketHistoryStack.ActiveMarketHistory();

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

        private DataGenerationPlan PlanInDateRange(MarketTimeBarCollection value)
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
            Stack<MarketTimeBarCollection> frames,
            MarketTimeBarCollection latestFrame,
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
                sellTrade ? tradeTime.AddMinutes(1) : tradeTime,
                sellTrade ? tradeTime.AddMinutes(1) : tradeTime,
                null,
                null,
                null,
                sellTrade ? tradeTime.AddMinutes(1) : tradeTime,
                OrderTypes.MARKET,
                sellTrade ? OrderPositions.SELL : OrderPositions.BUY,
                headSecurity.SpreadTimeBar.Price.Currency,
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
                null,
                null,
                new Trade[0]);

            TradeStream.Add(volume);
        }

        protected override void _TerminateTradingStrategy()
        { }
    }
}
