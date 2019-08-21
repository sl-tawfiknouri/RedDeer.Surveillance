namespace DataSynchroniser.Api.Bmll.Filters.Interfaces
{
    using SharedKernel.Contracts.Markets;

    public interface IBmllDataRequestFilter
    {
        bool ValidAssetType(MarketDataRequest request);
    }
}