namespace Surveillance.Engine.Rules.Markets.Interfaces
{
    using SharedKernel.Contracts.Markets;

    public interface IMarketDataCacheStrategy
    {
        DataSource DataSource { get; }

        IQueryableMarketDataResponse Query(MarketDataRequest request);
    }
}