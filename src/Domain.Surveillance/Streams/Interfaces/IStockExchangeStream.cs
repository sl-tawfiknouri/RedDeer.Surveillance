using Domain.Core.Markets.Collections;

namespace Domain.Surveillance.Streams.Interfaces
{
    public interface IStockExchangeStream : IPublishingStream<EquityIntraDayTimeBarCollection>
    {
    }
}