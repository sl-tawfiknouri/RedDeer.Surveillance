namespace Domain.Core.Trading.Execution
{
    using System;

    using Domain.Core.Trading.Orders;

    public class OrderAnalysis : IOrderAnalysis
    {
        public OrderAnalysis(Order order, PriceSentiment sentiment)
        {
            this.Order = order ?? throw new ArgumentNullException(nameof(order));
            this.Sentiment = sentiment;
        }

        public Order Order { get; }

        public PriceSentiment Sentiment { get; }
    }
}