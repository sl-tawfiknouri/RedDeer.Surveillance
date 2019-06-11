using Microsoft.AspNetCore.Builder;

namespace Surveillance.Api.App.Logging
{
    public static class LoggingApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRequestResponseLoggingMiddleware(this IApplicationBuilder builder)
            => builder.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}
