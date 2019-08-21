namespace DataSynchroniser.Api.Factset.Filters.Interfaces
{
    using SharedKernel.Contracts.Markets;

    public interface IFactsetDataRequestFilter
    {
        bool ValidAssetType(MarketDataRequest request);
    }
}