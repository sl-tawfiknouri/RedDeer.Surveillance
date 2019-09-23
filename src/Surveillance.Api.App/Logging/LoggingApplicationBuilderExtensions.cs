namespace Surveillance.Api.App.Logging
{
    using Microsoft.AspNetCore.Builder;

    public static class LoggingApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRequestResponseLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}