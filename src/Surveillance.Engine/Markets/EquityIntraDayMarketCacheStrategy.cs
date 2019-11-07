namespace Surveillance.Engine.Rules.Markets
{
    using System;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Engine.Rules.Markets.Interfaces;

    public class EquityIntraDayMarketCacheStrategy : IMarketDataCacheStrategy
    {
        private readonly IUniverseEquityIntraDayCache _cache;

        public EquityIntraDayMarketCacheStrategy(IUniverseEquityIntraDayCache cache)
        {
            this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public DataSource DataSource => DataSource.AnyIntraday;

        public IQueryableMarketDataResponse Query(MarketDataRequest request)
        {
            var rawResponse = this._cache.GetForLatestDayOnly(request);
            return new EquityIntraDayMarketDataResponse(rawResponse);
        }
    }
}