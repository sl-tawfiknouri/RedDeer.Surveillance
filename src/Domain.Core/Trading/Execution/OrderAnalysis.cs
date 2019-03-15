using System;
using Domain.Core.Trading.Orders;

namespace Domain.Core.Trading.Execution
{
    public class OrderAnalysis : IOrderAnalysis
    {
        public OrderAnalysis(
            Order order,
            PriceSentiment sentiment)
        {
            Order = order ?? throw new ArgumentNullException(nameof(order));
            Sentiment = sentiment;
        }

        public Order Order { get; }
        public PriceSentiment Sentiment { get; }
    }
}
