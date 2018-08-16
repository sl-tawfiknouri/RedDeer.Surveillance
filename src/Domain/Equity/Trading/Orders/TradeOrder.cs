using System;

namespace Domain.Equity.Trading.Orders
{
    /// <summary>
    /// A trade order to trade securities in a market
    /// </summary>
    public class TradeOrder
    {
        public TradeOrder(
            OrderType orderType,
            Market.Market market,
            Security security,
            Price? limit,
            int volume,
            OrderDirection direction,
            OrderStatus orderStatus)
        {
            OrderType = OrderType;
            Market = market;
            Security = security;
            Limit = limit;
            Volume = volume;
            Direction = direction;
            OrderStatus = orderStatus;

            if (orderType == OrderType.Limit
                && limit == null)
            {
                throw new ArgumentException(nameof(orderType));
            }
        }

        public OrderType OrderType { get; }

        public Market.Market Market { get; }

        public Security Security { get; }

        public Price? Limit { get; }

        public int Volume { get; }

        public OrderDirection Direction { get; }

        public OrderStatus OrderStatus { get; }
    }
}
