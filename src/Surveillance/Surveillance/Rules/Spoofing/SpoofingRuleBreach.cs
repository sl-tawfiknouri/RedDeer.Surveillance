using System;
using System.Linq;
using DomainV2.Financial;
using DomainV2.Trading;
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
            FinancialInstrument security,
            Order mostRecentTrade)
        {
            Window = window;
            Security = security;
            MostRecentTrade = mostRecentTrade;

            var totalTrades = fulfilledTradePosition.Get().ToList();
            totalTrades.AddRange(cancelledTradePosition.Get());
            Trades = new TradePosition(totalTrades);
            TradesInFulfilledPosition = fulfilledTradePosition;
            CancelledTrades = cancelledTradePosition;
        }

        public TimeSpan Window { get; }
        public ITradePosition Trades { get; }
        public ITradePosition TradesInFulfilledPosition { get; }
        public ITradePosition CancelledTrades { get; }
        public FinancialInstrument Security { get; }

        /// <summary>
        /// The trade whose fulfillment triggered the rule breach. This is a constituent of trades but not cancelled trades.
        /// </summary>
        public Order MostRecentTrade { get; }
    }
}
