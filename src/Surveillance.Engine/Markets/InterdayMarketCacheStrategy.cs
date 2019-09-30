namespace Surveillance.Engine.Rules.Markets
{
    using System;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Engine.Rules.Markets.Interfaces;

    public class InterdayMarketCacheStrategy : IMarketDataCacheStrategy
    {
        private readonly IUniverseEquityInterDayCache _cache;

        public InterdayMarketCacheStrategy(IUniverseEquityInterDayCache cache)
        {
            this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public DataSource DataSource { get; } = DataSource.AnyInterday;

        public IQueryableMarketDataResponse Query(MarketDataRequest request)
        {
            var rawResponse = this._cache.Get(request);
            return new InterdayMarketDataResponse(rawResponse);
        }
    }
}