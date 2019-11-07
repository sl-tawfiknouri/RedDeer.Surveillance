namespace Surveillance.Engine.Rules.Markets
{
    using SharedKernel.Contracts.Markets;
    using Surveillance.Engine.Rules.Markets.Interfaces;

    public class EquityMarketDataCacheStrategyFactory : IEquityMarketDataCacheStrategyFactory
    {
        public IMarketDataCacheStrategy InterdayStrategy(IUniverseEquityInterDayCache cache)
        {
            return new EquityInterDayMarketCacheStrategy(cache);
        }

        public IMarketDataCacheStrategy IntradayStrategy(IUniverseEquityIntraDayCache cache)
        {
            return new EquityIntraDayMarketCacheStrategy(cache);
        }
    }
}