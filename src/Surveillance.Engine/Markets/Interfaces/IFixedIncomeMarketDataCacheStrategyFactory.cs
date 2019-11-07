using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Engine.Rules.Markets.Interfaces
{
    public interface IFixedIncomeMarketDataCacheStrategyFactory
    {
        IMarketDataCacheStrategy InterdayStrategy(IUniverseFixedIncomeInterDayCache cache);

        IMarketDataCacheStrategy IntradayStrategy(IUniverseFixedIncomeIntraDayCache cache);
    }
}
