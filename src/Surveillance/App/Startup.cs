namespace RedDeer.Surveillance.App
{
    using System;

    using global::Surveillance;
    using global::Surveillance.Auditing;
    using global::Surveillance.Auditing.Context;
    using global::Surveillance.Auditing.DataLayer;
    using global::Surveillance.Auditing.DataLayer.Interfaces;
    using global::Surveillance.Auditing.DataLayer.Processes;
    using global::Surveillance.Data.Universe.Refinitiv;
    using global::Surveillance.Data.Universe.Refinitiv.Interfaces;
    using global::Surveillance.DataLayer;
    using global::Surveillance.DataLayer.Configuration.Interfaces;
    using global::Surveillance.Engine.DataCoordinator;
    using global::Surveillance.Engine.DataCoordinator.Configuration.Interfaces;
    using global::Surveillance.Engine.RuleDistributor;
    using global::Surveillance.Engine.Rules;
    using global::Surveillance.Reddeer.ApiClient;
    using global::Surveillance.Reddeer.ApiClient.Configuration.Interfaces;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using RedDeer.Surveillance.App.ScriptRunner.Interfaces;

    using StructureMap;

    public class Startup
    {
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseMvc();

            app.Run(async context => { await context.Response.WriteAsync("Red Deer Surveillance Service App"); });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var container = new Container();

            var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();

            var configBuilder = new Configuration.Configuration();
            var dbConfiguration = configBuilder.BuildDatabaseConfiguration(configurationBuilder);
            container.Inject(typeof(IDataLayerConfiguration), dbConfiguration);
            container.Inject(typeof(IAwsConfiguration), dbConfiguration);

            var apiConfiguration = configBuilder.BuildApiClientConfiguration(configurationBuilder);
            container.Inject(typeof(IApiClientConfiguration), apiConfiguration);

            container.Inject(typeof(IRuleConfiguration), configBuilder.BuildRuleConfiguration(configurationBuilder));
            container.Inject(typeof(ISystemDataLayerConfig), configBuilder.BuildDataLayerConfig(configurationBuilder));
            container.Inject(typeof(IRefinitivTickPriceHistoryApiConfig), configBuilder.BuildRefinitivTickPriceHistoryApiConfig(configurationBuilder));

            SystemProcessContext.ProcessType = SystemProcessType.SurveillanceService;

            container.Configure(
                config =>
                    {
                        config.IncludeRegistry<SystemSystemDataLayerRegistry>();
                        config.IncludeRegistry<SurveillanceSystemAuditingRegistry>();
                        config.IncludeRegistry<DataLayerRegistry>();
                        config.IncludeRegistry<SurveillanceRegistry>();
                        config.IncludeRegistry<RuleDistributorRegistry>();
                        config.IncludeRegistry<DataCoordinatorRegistry>();
                        config.IncludeRegistry<RuleRegistry>();
                        config.IncludeRegistry<ReddeerApiClientRegistry>();
                        config.IncludeRegistry<AppRegistry>();
                        config.IncludeRegistry<RefinitivRegistry>();
                        config.Populate(services);
                    });

            container.GetInstance<IScriptRunner>();

            return container.GetInstance<IServiceProvider>();
        }
    }
}