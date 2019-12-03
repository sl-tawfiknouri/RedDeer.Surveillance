using Surveillance.Engine.Rules.Markets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Engine.Rules.Markets
{
    public class FixedIncomeMarketDataCacheStrategyFactory : IFixedIncomeMarketDataCacheStrategyFactory
    {
        public IMarketDataCacheStrategy InterdayStrategy(IUniverseFixedIncomeInterDayCache cache)
        {
            return new FixedIncomeInterDayMarketCacheStrategy(cache);
        }

        public IMarketDataCacheStrategy IntradayStrategy(IUniverseFixedIncomeIntraDayCache cache)
        {
            return new FixedIncomeIntraDayMarketCacheStrategy(cache);
        }
    }
}
