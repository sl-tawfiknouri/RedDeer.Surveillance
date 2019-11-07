namespace Surveillance.Engine.Rules.Markets
{
    using System;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Engine.Rules.Markets.Interfaces;

    public class EquityInterDayMarketCacheStrategy : IMarketDataCacheStrategy
    {
        private readonly IUniverseEquityInterDayCache _cache;

        public EquityInterDayMarketCacheStrategy(IUniverseEquityInterDayCache cache)
        {
            this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public DataSource DataSource => DataSource.AnyInterday;

        public IQueryableMarketDataResponse Query(MarketDataRequest request)
        {
            var rawResponse = this._cache.Get(request);
            return new EquityInterDayMarketDataResponse(rawResponse);
        }
    }
}