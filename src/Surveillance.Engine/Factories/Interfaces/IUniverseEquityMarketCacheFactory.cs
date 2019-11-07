namespace Surveillance.Engine.Rules.Factories.Interfaces
{
    using System;

    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.Rules;

    public interface IUniverseEquityMarketCacheFactory
    {
        IUniverseEquityInterDayCache BuildInterday(RuleRunMode runMode);

        IUniverseEquityIntraDayCache BuildIntraday(TimeSpan window, RuleRunMode runMode);
    }
}