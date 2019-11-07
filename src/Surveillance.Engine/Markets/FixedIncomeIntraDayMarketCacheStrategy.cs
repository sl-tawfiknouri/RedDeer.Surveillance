using SharedKernel.Contracts.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Engine.Rules.Markets
{
    public class FixedIncomeIntraDayMarketCacheStrategy : IMarketDataCacheStrategy
    {
        private readonly IUniverseFixedIncomeIntraDayCache _cache;

        public FixedIncomeIntraDayMarketCacheStrategy(IUniverseFixedIncomeIntraDayCache cache)
        {
            this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public DataSource DataSource => DataSource.RefinitivIntraday;

        public IQueryableMarketDataResponse Query(MarketDataRequest request)
        {
            var rawResponse = this._cache.GetForLatestDayOnly(request);
            return new FixedIncomeIntraDayMarketDataResponse(rawResponse);
        }
    }
}
