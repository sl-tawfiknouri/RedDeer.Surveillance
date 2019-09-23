namespace Surveillance.Api.App.Authorization
{
    using System;

    using GraphQL.Authorization;
    using GraphQL.Validation;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    public static class GraphQlAuthorizationExtensions
    {
        public static void AddGraphQlAuth(this IServiceCollection services, Action<AuthorizationSettings> configure)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthorizationEvaluator, AuthorizationEvaluator>();
            services.AddTransient<IValidationRule, AuthorizationValidationRule>();

            services.AddSingleton(
                s =>
                    {
                        var authSettings = new AuthorizationSettings();
                        configure(authSettings);
                        return authSettings;
                    });
        }
    }
}