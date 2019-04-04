using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Domain.Surveillance.Rules;
using Domain.Surveillance.Rules.Interfaces;
using GraphQL;
using GraphQL.Server;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Security.Core;
using Security.Core.Abstractions;
using Security.Core.Services;
using Surveillance.Api.App.Exceptions;
using Surveillance.Api.App.Infrastructure;
using Surveillance.Api.App.Infrastructure.Interfaces;

namespace Surveillance.Api.App
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;
        private readonly ILogger _logger;

        public Startup(
            IConfiguration configuration,
            IHostingEnvironment environment,
            ILogger<Startup> logger)
        {
            Configuration = configuration;
            _environment = environment;
            _logger = logger;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();

            services.AddOptions();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            AddRateLimiting(services);
            AddJwtAuth(services);

            services.AddScoped<ISurveillanceAuthorisation, SurveillanceAuthorisation>();
            services.AddScoped<SurveillanceSchema>();
            services.AddScoped<IClaimsManifest, ClaimsManifest>();
            services.AddSingleton<IActiveRulesService, ActiveRulesService>();
            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));

            var manifest = new ClaimsManifest();

            services
                .AddGraphQL(o =>
                {
                    o.ExposeExceptions = _environment.IsDevelopment();
                    o.EnableMetrics = true;
                })
                .AddGraphTypes(ServiceLifetime.Scoped)
                .AddDataLoader();
        }

        private void AddJwtAuth(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {

                    options.RequireHttpsMetadata = !_environment.IsDevelopment();

                    var domainManifest = new DomainManifest();
                    var environment = Configuration.GetValue<string>("Environment");
                    var client = Configuration.GetValue<string>("Customer");
                    var clientIssuer = new ComponentName(environment, client, domainManifest.ClientService);
                    var serialiser = new ComponentNameSerialiserService();

                    var serialisedClientIssuer = serialiser.Serialise(clientIssuer);

                    var validIssuers = new[] { serialisedClientIssuer };
                    var validAudiences = new[] { serialisedClientIssuer };

                    var issuerSigningKeys = new List<SecurityKey>();
                    var secretKey = Configuration.GetValue<string>("Secret-Key-Jwt");

                    if (!string.IsNullOrEmpty(secretKey))
                    {
                        issuerSigningKeys.Add(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)));
                    }
                    else
                    {
                        _logger.LogError($"No secret key found for JWT");
                        throw new JwtMissingSecurityException();
                    }

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.FromMinutes(1),
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidIssuers = validIssuers,
                        ValidateAudience = true,
                        ValidAudiences = validAudiences,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeys = issuerSigningKeys
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = (context) =>
                        {
                            _logger.LogError(context.Exception, $"Authentication Failed for Identity: {context.Principal?.Identity?.Name}");
                            context.Fail("Invalid JWT token");
                            return Task.CompletedTask;
                        },

                        OnMessageReceived = (context) =>
                        {
                            _logger.LogDebug($"Authentication Message Received for Identity: {context.Principal?.Identity?.Name}");
                            return Task.CompletedTask;
                        },

                        OnTokenValidated = (context) =>
                        {
                            _logger.LogDebug($"Authentication Token Validated for Identity: {context.Principal?.Identity?.Name}");
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        private void AddRateLimiting(IServiceCollection services)
        {
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseIpRateLimiting();
            app.UseResponseCompression();

            app.UseAuthentication();
            app.UseGraphQL<SurveillanceSchema>("/graphql/surveillance");
        }
    }
}
