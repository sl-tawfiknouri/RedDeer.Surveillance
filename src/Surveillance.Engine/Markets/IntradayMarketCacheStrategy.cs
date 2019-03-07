using System;
using SharedKernel.Contracts.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;

namespace Surveillance.Engine.Rules.Markets
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
            var rawResponse = _cache.GetForLatestDayOnly(request);
            return new IntradayMarketDataResponse(rawResponse);
        }
    }
}
