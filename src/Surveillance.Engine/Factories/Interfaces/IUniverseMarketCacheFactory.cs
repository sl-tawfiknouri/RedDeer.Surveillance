using System;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.Rules;

namespace Surveillance.Engine.Rules.Factories.Interfaces
{
    public interface IUniverseMarketCacheFactory
    {
        IUniverseEquityIntradayCache BuildIntraday(TimeSpan window, RuleRunMode runMode);
        IUniverseEquityInterDayCache BuildInterday(RuleRunMode runMode);
    }
}