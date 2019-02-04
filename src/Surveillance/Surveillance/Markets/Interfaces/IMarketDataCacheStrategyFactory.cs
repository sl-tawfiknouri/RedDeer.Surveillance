namespace Surveillance.Markets.Interfaces
{
    public interface IMarketDataCacheStrategyFactory
    {
        IMarketDataCacheStrategy InterdayStrategy(IUniverseEquityInterDayCache cache);
        IMarketDataCacheStrategy IntradayStrategy(IUniverseEquityIntradayCache cache);
    }
}
