namespace Surveillance.Api.App
{
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
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using RedDeer.Extensions.Security.Authentication.Jwt.Abstractions;
    using RedDeer.Security.Core;
    using RedDeer.Security.Core.Abstractions;
    using RedDeer.Security.Core.Services;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.App.Exceptions;
    using Surveillance.Api.App.Infrastructure;
    using Surveillance.Api.App.Logging;
    using Surveillance.Api.App.Middlewares;
    using Surveillance.Api.App.Services;
    using Surveillance.Api.DataAccess.Abstractions.DbContexts.Factory;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;
    using Surveillance.Api.DataAccess.Abstractions.Services;
    using Surveillance.Api.DataAccess.DbContexts.Factory;
    using Surveillance.Api.DataAccess.Repositories;
    using Surveillance.Api.DataAccess.Services;
    using Surveillance.Data.Universe.Refinitiv;
    using Surveillance.Data.Universe.Refinitiv.Interfaces;

    public class Startup
    {
        private readonly IHostEnvironment _environment;

        private readonly ILogger _logger;

        private readonly IStartupConfig _startupConfig;

        public Startup(
            IConfiguration configuration,
            IHostEnvironment environment,
            ILogger<Startup> logger,
            IStartupConfig startupConfig)
        {
            this.Configuration = configuration;
            this._environment = environment;
            this._logger = logger;
            this._startupConfig = startupConfig;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            else app.UseHsts();

            app.UseHttpsRedirection();
            app.UseIpRateLimiting();
            app.UseResponseCompression();

            app.UseAuthentication();

            // Stopping execution when UnAuthenticated
            app.UseMiddleware<UnAuthenticatedMiddleware>();

            app.UseWhen(
                x => x.Request.Path.Value.StartsWith("/graphql/surveillance"),
                appBuilder =>
                    {
                        if (this.Configuration.GetValue<bool>("UseRequestResponseLoggingMiddleware"))
                            appBuilder.UseRequestResponseLoggingMiddleware();

                        appBuilder.UseGraphQL<ISchema>(string.Empty);
                    });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IJwtTokenService, RedDeer.Extensions.Security.Authentication.Jwt.JwtTokenService>();
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddRequestResponseLoggingMiddleware();

            services.AddResponseCompression();

            services.AddOptions();
            services.AddMemoryCache();
            this.AddRateLimiting(services);
            this.AddJwtAuth(services);

            services.AddScoped<ISchema>(
                s =>
                    {
                        var schema = new SurveillanceSchema(new FuncDependencyResolver(s.GetRequiredService));
                        return schema;
                    });

            services.AddSingleton<IActiveRulesService, ActiveRulesService>();
            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddScoped<IProvideClaimsPrincipal, GraphQlUserContext>();
            services.AddScoped<IGraphQlDbContextFactory, GraphQlDbContextFactory>();
            services.AddScoped<ITimeZoneService, TimeZoneService>();

            services.AddScoped<IMarketRepository, MarketRepository>();
            services.AddScoped<IBrokerRepository, BrokerRepository>();
            services.AddScoped<IRuleBreachRepository, RuleBreachRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ISystemProcessOperationRuleRunRepository, SystemProcessOperationRuleRunRepository>();
            services
                .AddScoped<ISystemProcessOperationUploadFileRepository, SystemProcessOperationUploadFileRepository>();
            services
                .AddScoped<ISystemProcessOperationDataSynchroniserRepository,
                    SystemProcessOperationDataSynchroniserRepository>();
            services
                .AddScoped<ISystemProcessOperationDistributeRuleRepository,
                    SystemProcessOperationDistributeRuleRepository>();
            services.AddScoped<ISystemProcessOperationRepository, SystemProcessOperationRepository>();
            services.AddScoped<ISystemProcessRepository, SystemProcessRepository>();
            services.AddScoped<IFinancialInstrumentRepository, FinancialInstrumentRepository>();

            services.AddTransient<IRefinitivTickPriceHistoryService, RefinitivTickPriceHistoryService>();
            services.AddRefinitivServices();

            services.AddTransient<IRefinitivTickPriceHistoryApiConfig>((sp) => new RefinitivTickPriceHistoryApiConfig
            {
                RefinitivTickPriceHistoryApiAddress = this.Configuration["RefinitivTickPriceHistoryApiAddress"],
                RefinitivTickPriceHistoryApiJwtBearerTokenSymetricSecurityKey = this.Configuration["RefinitivTickPriceHistoryApiJwtBearerTokenSymetricSecurityKey"],
                RefinitivTickPriceHistoryApiPollingSeconds = this.Configuration.GetValue("RefinitivTickPriceHistoryApiPollingSeconds", 60),
                RefinitivTickPriceHistoryApiTimeOutDurationSeconds = this.Configuration.GetValue("RefinitivTickPriceHistoryApiTimeOutDurationSeconds", 600)
            });

            // Test services should be added at end of list so that they can override defaults
            if (this._startupConfig.IsTest) this._startupConfig.ConfigureTestServices(services);

            var jwtTokenService = new JwtTokenService();

            services.AddGraphQlAuth(
                _ =>
                    {
                        _.AddPolicy(
                            PolicyManifest.AdminPolicy,
                            p => p.RequireClaim(
                                jwtTokenService.ClaimType,
                                new Claim(ScopeType.Surveillance, AccessType.AdminRead).ToString(),
                                new Claim(ScopeType.Surveillance, AccessType.AdminWrite).ToString()));

                        _.AddPolicy(
                            PolicyManifest.AdminReaderPolicy,
                            p => p.RequireClaim(
                                jwtTokenService.ClaimType,
                                new Claim(ScopeType.Surveillance, AccessType.AdminRead).ToString()));

                        _.AddPolicy(
                            PolicyManifest.AdminWriterPolicy,
                            p => p.RequireClaim(
                                jwtTokenService.ClaimType,
                                new Claim(ScopeType.Surveillance, AccessType.AdminWrite).ToString()));

                        _.AddPolicy(
                            PolicyManifest.UserPolicy,
                            p => p.RequireClaim(
                                jwtTokenService.ClaimType,
                                new Claim(ScopeType.Surveillance, AccessType.Read).ToString(),
                                new Claim(ScopeType.Surveillance, AccessType.Write).ToString()));

                        _.AddPolicy(
                            PolicyManifest.UserReaderPolicy,
                            p => p.RequireClaim(
                                jwtTokenService.ClaimType,
                                new Claim(ScopeType.Surveillance, AccessType.Read).ToString()));

                        _.AddPolicy(
                            PolicyManifest.UserWriterPolicy,
                            p => p.RequireClaim(
                                jwtTokenService.ClaimType,
                                new Claim(ScopeType.Surveillance, AccessType.Write).ToString()));
                    });

            services.AddGraphQL(
                    o =>
                        {
                            o.ExposeExceptions = this._environment.IsDevelopment();
                            o.EnableMetrics = true;
                        }).AddUserContextBuilder(i => new GraphQlUserContext { User = i.User })
                .AddGraphTypes(ServiceLifetime.Scoped).AddDataLoader();
        }

        private void AddJwtAuth(IServiceCollection services)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.RequireHttpsMetadata = true;

                        var environment = this.Configuration.GetValue<string>("Environment");
                        var client = this.Configuration.GetValue<string>("Customer");

                        this._logger.LogInformation(
                            $"Retrieved configuration (Environment: '{environment}', Customer: '{client}').");

                        var clientIssuer = new ComponentName(environment, client, ScopeType.ClientService);
                        var serialisedClientIssuer = clientIssuer.ToString();

                        var validIssuers = new[] { serialisedClientIssuer };
                        var validAudiences = new[] { serialisedClientIssuer };

                        this._logger.LogInformation(
                            $"JWT configuration (ValidIssuers: '{string.Join(";", validIssuers)}', ValidAudiences: '{string.Join(";", validAudiences)}').");

                        var issuerSigningKeys = new List<SecurityKey>();
                        var secretKey = this.Configuration.GetValue<string>("Secret-Key-Jwt");

                        if (!string.IsNullOrEmpty(secretKey))
                        {
                            issuerSigningKeys.Add(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)));
                        }
                        else
                        {
                            this._logger.LogError("No secret key found for JWT");
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
                            OnAuthenticationFailed = context =>
                            {
                                this._logger.LogWarning(context.Exception,$"Authentication Failed for Identity: {context.Principal?.Identity?.Name}");

                                context.Response.StatusCode = 401;
                                context.Fail("Invalid JWT token");

                                return Task.CompletedTask;
                            },
                            OnMessageReceived = context =>
                            {
                                this._logger.LogDebug($"Authentication Message Received for Identity: {context.Principal?.Identity?.Name}");
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                this._logger.LogDebug($"Authentication Token Validated for Identity: {context.Principal?.Identity?.Name}");
                                return Task.CompletedTask;
                            }
                        };
                    });
        }

        private void AddRateLimiting(IServiceCollection services)
        {
            services.Configure<IpRateLimitOptions>(this.Configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(this.Configuration.GetSection("IpRateLimitPolicies"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}