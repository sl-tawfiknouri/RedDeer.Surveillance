using Domain.Streams;

namespace Domain.Trades.Streams.Interfaces
{
    public interface ITradeOrderStream<T> : IPublishingStream<T>
    {
    }
}
