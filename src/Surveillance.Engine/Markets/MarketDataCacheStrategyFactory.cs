namespace Surveillance.Engine.Rules.Markets
{
    using Surveillance.Engine.Rules.Markets.Interfaces;

    public class MarketDataCacheStrategyFactory : IMarketDataCacheStrategyFactory
    {
        public IMarketDataCacheStrategy InterdayStrategy(IUniverseEquityInterDayCache cache)
        {
            return new InterdayMarketCacheStrategy(cache);
        }

        public IMarketDataCacheStrategy IntradayStrategy(IUniverseEquityIntradayCache cache)
        {
            return new IntradayMarketCacheStrategy(cache);
        }
    }
}