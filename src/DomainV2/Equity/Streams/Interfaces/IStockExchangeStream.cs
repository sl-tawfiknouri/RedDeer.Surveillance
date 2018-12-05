using DomainV2.Equity.Frames;
using DomainV2.Streams;

namespace DomainV2.Equity.Streams.Interfaces
{
    public interface IStockExchangeStream : IPublishingStream<ExchangeFrame>
    {
    }
}