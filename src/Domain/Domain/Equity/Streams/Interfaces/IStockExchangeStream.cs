using Domain.Equity.Frames;
using Domain.Streams;

namespace Domain.Equity.Streams.Interfaces
{
    public interface IStockExchangeStream : IPublishingStream<ExchangeFrame>
    {
    }
}