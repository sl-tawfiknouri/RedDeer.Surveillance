using Domain.Core.Trading.Orders;

namespace Domain.Core.Trading.Execution
{
    public interface IOrderAnalysis
    {
        Order Order { get; }
        PriceSentiment Sentiment { get; }
    }
}