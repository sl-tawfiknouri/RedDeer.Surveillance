using Surveillance.Markets.Interfaces;

namespace Surveillance.Markets
{
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
