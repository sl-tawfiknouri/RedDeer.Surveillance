using Microsoft.Extensions.DependencyInjection;
using Surveillance.Data.Universe.Refinitiv.Interfaces;

namespace Surveillance.Data.Universe.Refinitiv
{
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
