using System;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets;
using Surveillance.Markets.Interfaces;

namespace Surveillance.Factories
{
    public class UniverseMarketCacheFactory : IUniverseMarketCacheFactory
    {
        public IUniverseMarketCache Build(TimeSpan window)
        {
            return new UniverseMarketCache(window);
        }
    }
}
