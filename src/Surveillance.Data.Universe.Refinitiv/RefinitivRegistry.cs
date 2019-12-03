using StructureMap;
using Surveillance.Data.Universe.Refinitiv.Interfaces;

namespace Surveillance.Data.Universe.Refinitiv
{
    public class RefinitivRegistry : Registry
    {
        public RefinitivRegistry()
        {
            this.For<ITickPriceHistoryServiceClientFactory>().Use<TickPriceHistoryServiceClientFactory>();
            this.For<IRefinitivTickPriceHistoryApi>().Use<RefinitivTickPriceHistoryApi>();
        }
    }
}
