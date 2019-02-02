using System;
using Surveillance.Markets.Interfaces;
using Surveillance.Rules;

namespace Surveillance.Factories.Interfaces
{
    public interface IUniverseMarketCacheFactory
    {
        IUniverseEquityIntradayCache BuildIntraday(TimeSpan window, RuleRunMode runMode);
        IUniverseEquityInterDayCache BuildInterday(RuleRunMode runMode);
    }
}