using DomainV2.Markets;

namespace Surveillance.Markets.Interfaces
{
    public interface IMarketDataCacheStrategy
    {
        IQueryableMarketDataResponse Query(MarketDataRequest request);
    }
}
