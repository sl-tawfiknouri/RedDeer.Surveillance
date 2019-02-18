using Domain.Markets;

namespace DataSynchroniser.Api.Bmll.Filters.Interfaces
{
    public interface IMarketDataRequestFilter
    {
        bool Filter(MarketDataRequest request);
    }
}