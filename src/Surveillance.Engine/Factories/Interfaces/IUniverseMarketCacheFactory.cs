namespace Surveillance.Engine.Rules.Factories.Interfaces
{
    using System;

    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.Rules;

    public interface IUniverseMarketCacheFactory
    {
        IUniverseEquityInterDayCache BuildInterday(RuleRunMode runMode);

        IUniverseEquityIntradayCache BuildIntraday(TimeSpan window, RuleRunMode runMode);
    }
}