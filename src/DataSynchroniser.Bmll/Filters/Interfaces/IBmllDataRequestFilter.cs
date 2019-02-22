using Domain.Markets;

namespace DataSynchroniser.Api.Bmll.Filters.Interfaces
{
    public interface IBmllDataRequestFilter
    {
        bool ValidAssetType(MarketDataRequest request);
    }
}