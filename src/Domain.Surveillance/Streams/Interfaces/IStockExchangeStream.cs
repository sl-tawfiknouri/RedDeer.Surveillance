using Domain.Equity.TimeBars;
using Domain.Streams;

namespace Domain.Equity.Streams.Interfaces
{
    public interface IStockExchangeStream : IPublishingStream<EquityIntraDayTimeBarCollection>
    {
    }
}