using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Domain.Surveillance.Rules;
using Domain.Surveillance.Rules.Interfaces;
using GraphQL;
using GraphQL.Authorization;
using GraphQL.Server;
using GraphQL.Types;
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
using Surveillance.Api.App.Authorization;
using Surveillance.Api.App.Exceptions;
using Surveillance.Api.App.Infrastructure;
using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
using Surveillance.Api.DataAccess.Abstractions.Repositories;
using Surveillance.Api.DataAccess.DbContexts.Factory;
using Surveillance.Api.DataAccess.Repositories;

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
            AddRateLimiting(services);
            AddJwtAuth(services);

            services.AddScoped<ISchema>(s =>
            {
                var schema = new SurveillanceSchema(new FuncDependencyResolver(s.GetRequiredService));
                return schema;
            });

            services.AddScoped<IClaimsManifest, ClaimsManifest>();
            services.AddSingleton<IActiveRulesService, ActiveRulesService>();
            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddScoped<IProvideClaimsPrincipal, GraphQlUserContext>();
            services.AddScoped<IGraphQlDbContextFactory, GraphQlDbContextFactory>();

            services.AddScoped<IMarketRepository, MarketRepository>();
            services.AddScoped<IRuleBreachRepository, RuleBreachRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ISystemProcessOperationRuleRunRepository, SystemProcessOperationRuleRunRepository>();
            services.AddScoped<ISystemProcessOperationUploadFileRepository, SystemProcessOperationUploadFileRepository>();
            services.AddScoped<ISystemProcessOperationDataSynchroniserRepository, SystemProcessOperationDataSynchroniserRepository>();
            services.AddScoped<ISystemProcessOperationDistributeRuleRepository, SystemProcessOperationDistributeRuleRepository>();
            services.AddScoped<ISystemProcessOperationRepository, SystemProcessOperationRepository>();
            services.AddScoped<ISystemProcessRepository, SystemProcessRepository>();
            services.AddScoped<IFinancialInstrumentRepository, FinancialInstrumentRepository>();

            var manifest = new ClaimsManifest();

            services.AddGraphQlAuth(_ =>
            {
                _.AddPolicy(
                    PolicyManifest.AdminPolicy,
                    p => p.RequireClaim(PolicyManifest.ClaimName, manifest.SurveillanceReaderPrivilege, manifest.SurveillanceWriterPrivilege));

                _.AddPolicy(
                    PolicyManifest.AdminReaderPolicy,
                    p => p.RequireClaim(PolicyManifest.ClaimName, manifest.SurveillanceReaderPrivilege));

                _.AddPolicy(
                    PolicyManifest.AdminWriterPolicy,
                    p => p.RequireClaim(PolicyManifest.ClaimName, manifest.SurveillanceWriterPrivilege));

                _.AddPolicy(
                    PolicyManifest.UserPolicy,
                    p => p.RequireClaim(PolicyManifest.ClaimName, manifest.SurveillanceReader, manifest.SurveillanceWriter));

                _.AddPolicy(
                    PolicyManifest.UserReaderPolicy,
                    p => p.RequireClaim(PolicyManifest.ClaimName, manifest.SurveillanceReader));

                _.AddPolicy(
                    PolicyManifest.UserWriterPolicy,
                    p => p.RequireClaim(PolicyManifest.ClaimName, manifest.SurveillanceWriter));
            });

            services
                .AddGraphQL(o =>
                {
                    o.ExposeExceptions = _environment.IsDevelopment();
                    o.EnableMetrics = true;
                })
                .AddUserContextBuilder(i => new GraphQlUserContext { User = i.User })
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

                    options.RequireHttpsMetadata = true;

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

                            context.Response.StatusCode = 401;
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseIpRateLimiting();
            app.UseResponseCompression();

            app.UseAuthentication();

            // Stopping execution when UnAuthenticated
            app.Use(async (context, next) =>
            {
                if (context.Response.StatusCode != 401)
                {
                    await next.Invoke();
                }
            });

            app.UseGraphQL<ISchema>("/graphql/surveillance");
        }
    }
}
