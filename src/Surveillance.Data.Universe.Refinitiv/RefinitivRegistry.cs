using Microsoft.Extensions.DependencyInjection;
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

    public static class RefinitivServiceCollectionExtensions
    {
        public static IServiceCollection AddRefinitivServices(this IServiceCollection services)
        {
            services
                .AddTransient<ITickPriceHistoryServiceClientFactory, TickPriceHistoryServiceClientFactory>()
                .AddTransient<IRefinitivTickPriceHistoryApi, RefinitivTickPriceHistoryApi>();

            return services;
        }
    }
}
