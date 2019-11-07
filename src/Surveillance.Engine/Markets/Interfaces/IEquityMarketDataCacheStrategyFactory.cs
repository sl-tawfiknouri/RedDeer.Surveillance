using SharedKernel.Contracts.Markets;

namespace Surveillance.Engine.Rules.Markets.Interfaces
{
    public interface IEquityMarketDataCacheStrategyFactory
    {
        IMarketDataCacheStrategy InterdayStrategy(IUniverseEquityInterDayCache cache);

        IMarketDataCacheStrategy IntradayStrategy(IUniverseEquityIntraDayCache cache);
    }
}