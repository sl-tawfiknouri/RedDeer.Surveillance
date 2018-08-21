using Domain.Equity.Trading.Frames;
using Domain.Streams;

namespace Domain.Equity.Trading.Streams.Interfaces
{
    public interface IStockExchangeStream : PublishingStream<ExchangeFrame>
    {
    }
}