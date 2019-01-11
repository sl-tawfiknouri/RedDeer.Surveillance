using System;
using Surveillance.Markets.Interfaces;
using Surveillance.Rules;

namespace Surveillance.Factories.Interfaces
{
    public interface IUniverseMarketCacheFactory
    {
        IUniverseMarketCache Build(TimeSpan window, RuleRunMode runMode);
    }
}