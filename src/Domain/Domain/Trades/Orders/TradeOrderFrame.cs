using System;
using Domain.Equity;
using Domain.Market;

namespace Domain.Trades.Orders
{
    /// <summary>
    /// A trade order to trade securities in a market
    /// </summary>
    public class TradeOrderFrame
    {
        public TradeOrderFrame(
            OrderType orderType,
            StockExchange market,
            Security security,
            Price? limit,
            int volume,
            OrderDirection direction,
            OrderStatus orderStatus,
            DateTime date)
        {
            OrderType = orderType;
            Market = market;
            Security = security;
            Limit = limit;
            Volume = volume;
            Direction = direction;
            OrderStatus = orderStatus;
            StatusChangedOn = date;

            if (orderType == OrderType.Limit
                && limit == null)
            {
                throw new ArgumentException(nameof(orderType));
            }
        }

        public OrderType OrderType { get; set;}

        public StockExchange Market { get; set; }

        public Security Security { get; set; }

        public Price? Limit { get; set; }

        public DateTime StatusChangedOn { get; set; }

        public int Volume { get; set; }

        public OrderDirection Direction { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public override string ToString()
        {
            return $"Market({Market.Id.Id}) Time({StatusChangedOn.ToLongTimeString()}) Security({Security.Id.Id}) Direction({Direction}) Order({OrderType}) Volume({Volume}) Limit({Limit?.Value}) Status({OrderStatus})";
        }
    }
}
