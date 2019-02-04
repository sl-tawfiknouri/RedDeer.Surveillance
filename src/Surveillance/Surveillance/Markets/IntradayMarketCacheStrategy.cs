using System;
using DomainV2.Markets;
using Surveillance.Markets.Interfaces;

namespace Surveillance.Markets
{
    public class IntradayMarketCacheStrategy : IMarketDataCacheStrategy
    {
        private readonly IUniverseEquityIntradayCache _cache;

        public IntradayMarketCacheStrategy(IUniverseEquityIntradayCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public IQueryableMarketDataResponse Query(MarketDataRequest request)
        {
            var rawResponse = _cache.Get(request);
            return new IntradayMarketDataResponse(rawResponse);
        }
    }
}
