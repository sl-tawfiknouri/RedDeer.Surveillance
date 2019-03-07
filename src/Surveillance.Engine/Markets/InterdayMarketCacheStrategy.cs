using System;
using SharedKernel.Contracts.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;

namespace Surveillance.Engine.Rules.Markets
{
    public class InterdayMarketCacheStrategy : IMarketDataCacheStrategy
    {
        private readonly IUniverseEquityInterDayCache _cache;

        public InterdayMarketCacheStrategy(IUniverseEquityInterDayCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public IQueryableMarketDataResponse Query(MarketDataRequest request)
        {
            var rawResponse = _cache.Get(request);
            return new InterdayMarketDataResponse(rawResponse);
        }
    }
}
