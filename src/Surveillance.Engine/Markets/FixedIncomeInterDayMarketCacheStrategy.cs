using SharedKernel.Contracts.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Engine.Rules.Markets
{
    public class FixedIncomeInterDayMarketCacheStrategy : IMarketDataCacheStrategy
    {
        private readonly IUniverseFixedIncomeInterDayCache _cache;

        public FixedIncomeInterDayMarketCacheStrategy(IUniverseFixedIncomeInterDayCache cache)
        {
            this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public DataSource DataSource => DataSource.RefinitivInterday;

        public IQueryableMarketDataResponse Query(MarketDataRequest request)
        {
            var rawResponse = this._cache.Get(request);
            return new FixedIncomeInterDayMarketDataResponse(rawResponse);
        }
    }
}
