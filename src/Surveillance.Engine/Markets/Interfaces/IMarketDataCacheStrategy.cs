using DomainV2.Markets;

namespace Surveillance.Engine.Rules.Markets.Interfaces
{
    public interface IMarketDataCacheStrategy
    {
        IQueryableMarketDataResponse Query(MarketDataRequest request);
    }
}
