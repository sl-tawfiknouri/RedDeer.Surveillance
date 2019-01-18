using System;
using Surveillance.Markets.Interfaces;
using Surveillance.Rules;

namespace Surveillance.Factories.Interfaces
{
    public interface IUniverseMarketCacheFactory
    {
        IUniverseEquityIntradayCache Build(TimeSpan window, RuleRunMode runMode);
        IUniverseEquityInterDayCache BuildInterday(RuleRunMode runMode);
    }
}