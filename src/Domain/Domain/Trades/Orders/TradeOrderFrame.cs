using System;
using Domain.Equity;
using Domain.Market;

namespace Domain.Trades.Orders
{
    /// <summary>
    /// A trade order to trade securities in a market.
    /// Not necessarily a 'trade'; merely the order to do so.
    /// </summary>
    public class TradeOrderFrame
    {
        public TradeOrderFrame(
            OrderType orderType,
            StockExchange market,
            Security security,
            Price? limit,
            Price? executedPrice,
            int fulfilledVolume,
            int orderedVolume,
            OrderPosition position,
            OrderStatus orderStatus,
            DateTime statusChangedOn,
            DateTime tradeSubmittedOn,
            string traderId,
            string tradeClientAttributionId,
            string accountId,
            string dealerInstructions,
            string partyBrokerId,
            string counterPartyBrokerId,
            string tradeRationale,
            string tradeStrategy)
        {
            OrderType = orderType;
            Market = market;
            Security = security;
            Limit = limit;
            ExecutedPrice = executedPrice;
            FulfilledVolume = fulfilledVolume;
            OrderedVolume = orderedVolume;
            Position = position;
            OrderStatus = orderStatus;
            StatusChangedOn = statusChangedOn;
            TradeSubmittedOn = tradeSubmittedOn;
            TraderId = traderId ?? string.Empty;
            TradeClientAttributionId = tradeClientAttributionId ?? string.Empty;
            AccountId = accountId ?? string.Empty;
            DealerInstructions = dealerInstructions ?? string.Empty;
            PartyBrokerId = partyBrokerId ?? string.Empty;
            CounterPartyBrokerId = counterPartyBrokerId ?? string.Empty;
            TradeRationale = tradeRationale ?? string.Empty;
            TradeStrategy = tradeStrategy ?? string.Empty;

            if (orderType == OrderType.Limit
                && limit == null)
            {
                throw new ArgumentException(nameof(orderType));
            }
        }

        /// <summary>
        /// The type of order i.e. market / limit
        /// </summary>
        public OrderType OrderType { get; set;}

        /// <summary>
        /// The market the security is being traded on
        /// </summary>
        public StockExchange Market { get; set; }

        /// <summary>
        /// The security being traded
        /// </summary>
        public Security Security { get; set; }

        /// <summary>
        /// If its a limit order, the limit price
        /// </summary>
        public Price? Limit { get; set; }

        /// <summary>
        /// Trade initially submitted on
        /// </summary>
        public DateTime TradeSubmittedOn { get; set; }

        /// <summary>
        /// Last update to the order (i.e. placed -> cancelled; placed -> fulfilled)
        /// </summary>
        public DateTime StatusChangedOn { get; set; }

        /// <summary>
        /// The amount that was requested to trade
        /// </summary>
        public int OrderedVolume { get; set; }

        /// <summary>
        /// The amount that was traded
        /// </summary>
        public int FulfilledVolume { get; set; }
        
        /// <summary>
        /// This is the price the trade was executed at; if several prices use the weighted average
        /// </summary>
        public Price? ExecutedPrice { get; set; }

        /// <summary>
        /// The currency for the executed price
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string OrderCurrency { get; set; }

        /// <summary>
        /// Buy or Sell
        /// </summary>
        public OrderPosition Position { get; set; }

        /// <summary>
        /// Status of the order (placed/cancelled/fulfilled)
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// A client identifier of the trader placing the order
        /// </summary>
        public string TraderId { get; set; }

        /// <summary>
        /// The client the trader is trading on behalf of
        /// </summary>
        public string TradeClientAttributionId { get; set; }

        /// <summary>
        /// The client account traded for
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// The broker submitting the trade to the market
        /// </summary>
        public string PartyBrokerId { get; set; }

        /// <summary>
        /// The counter party broker matching the order
        /// </summary>
        public string CounterPartyBrokerId { get; set; }

        /// <summary>
        /// The instruction notes passed to the dealer
        /// </summary>
        public string DealerInstructions { get; set; }

        /// <summary>
        /// The traders rationalisation for the trade
        /// </summary>
        public string TradeRationale { get; set; }

        /// <summary>
        /// The strategy behind the trade
        /// </summary>
        public string TradeStrategy { get; set; }

        public override string ToString()
        {
            return $"Market({Market.Id.Id}) Time({StatusChangedOn.ToLongTimeString()}) Security({Security.Identifiers}) Direction({Position}) Order({OrderType}) Volume({FulfilledVolume}) Limit({Limit?.Value}) Status({OrderStatus}) Trader({TraderId}) SubmittedOn({TradeSubmittedOn.ToLongTimeString()})";
        }
    }
}
