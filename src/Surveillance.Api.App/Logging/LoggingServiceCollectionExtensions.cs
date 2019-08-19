namespace Surveillance.Api.App.Logging
{
    using Microsoft.Extensions.DependencyInjection;

    public static class LoggingServiceCollectionExtensions
    {
        public static IServiceCollection AddRequestResponseLoggingMiddleware(this IServiceCollection services)
        {
            return services.AddTransient<RequestResponseLoggingMiddleware>();
        }
    }
}