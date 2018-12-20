using System;
using Surveillance.Markets.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface IUniverseMarketCacheFactory
    {
        IUniverseMarketCache Build(TimeSpan window);
    }
}