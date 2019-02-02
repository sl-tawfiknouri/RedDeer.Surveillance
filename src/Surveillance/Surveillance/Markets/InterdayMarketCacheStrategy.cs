using System;
using DomainV2.Markets;
using Surveillance.Markets.Interfaces;

namespace Surveillance.Markets
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
