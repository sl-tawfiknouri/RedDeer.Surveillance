namespace Domain.Surveillance.Streams.Interfaces
{
    using Domain.Core.Markets.Collections;

    public interface IStockExchangeStream : IPublishingStream<EquityIntraDayTimeBarCollection>
    {
    }
}