using Domain.Markets;

namespace DataSynchroniser.Api.Factset.Filters.Interfaces
{
    public interface IFactsetDataRequestFilter
    {
        bool ValidAssetType(MarketDataRequest request);
    }
}