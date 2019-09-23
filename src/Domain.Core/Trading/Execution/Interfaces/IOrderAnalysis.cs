namespace Domain.Core.Trading.Execution
{
    using Domain.Core.Trading.Orders;

    public interface IOrderAnalysis
    {
        Order Order { get; }

        PriceSentiment Sentiment { get; }
    }
}