using Domain.Equity.Trading.Orders;
using Domain.Streams;

namespace Domain.Equity.Trading.Streams.Interfaces
{
    public interface ITradeOrderStream<T> : PublishingStream<T>
    {
    }
}
