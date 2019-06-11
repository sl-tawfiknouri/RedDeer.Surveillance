using Microsoft.Extensions.DependencyInjection;

namespace Surveillance.Api.App.Logging
{
    public static class LoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddRequestResponseLoggingMiddleware(this IServiceCollection services)
            => services.AddTransient<RequestResponseLoggingMiddleware>();
    }
}
