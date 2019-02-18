using Domain.Markets;

namespace DataSynchroniser.Api.Factset.Filters.Interfaces
{
    public interface IMarketDataRequestFilter
    {
        bool Filter(MarketDataRequest request);
    }
}