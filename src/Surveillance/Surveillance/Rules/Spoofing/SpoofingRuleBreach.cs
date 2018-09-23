using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Equity;
using Domain.Trades.Orders;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Trades;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.Spoofing
{
    public class SpoofingRuleBreach : ISpoofingRuleBreach
    {
        public SpoofingRuleBreach(
            TimeSpan window,
            ITradePosition fulfilledTradePosition,
            ITradePosition cancelledTradePosition,
            Security security,
            TradeOrderFrame mostRecentTrade)
        {
            Window = window;
            Security = security;
            MostRecentTrade = mostRecentTrade;

            var totalTrades = fulfilledTradePosition.Get().ToList();
            totalTrades.AddRange(cancelledTradePosition.Get());
            Trades = new TradePosition(totalTrades, null, null, null);

            TradesInFulfilledPosition =
                fulfilledTradePosition
                ?? new TradePosition(new List<TradeOrderFrame>(), null, null, null);

            CancelledTrades =
                cancelledTradePosition
                ?? new TradePosition(new List<TradeOrderFrame>(), null, null, null);
        }

        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public ITradePosition TradesInFulfilledPosition { get; }
        public ITradePosition CancelledTrades { get; }
        public Security Security { get; }

        /// <summary>
        /// The trade whose fulfillment triggered the rule breach. This is a constituent of trades but not cancelled trades.
        /// </summary>
        public TradeOrderFrame MostRecentTrade { get; }
    }
}
