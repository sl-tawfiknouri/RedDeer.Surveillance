namespace Surveillance.Engine.Rules.Markets
{
    using System;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Engine.Rules.Markets.Interfaces;

    public class IntradayMarketCacheStrategy : IMarketDataCacheStrategy
    {
        private readonly IUniverseEquityIntradayCache _cache;

        public IntradayMarketCacheStrategy(IUniverseEquityIntradayCache cache)
        {
            this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public DataSource DataSource { get; } = DataSource.AllIntraday;

        public IQueryableMarketDataResponse Query(MarketDataRequest request)
        {
            var rawResponse = this._cache.GetForLatestDayOnly(request);
            return new IntradayMarketDataResponse(rawResponse);
        }
    }
}