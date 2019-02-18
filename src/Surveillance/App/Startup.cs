using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedDeer.Surveillance.App.ScriptRunner.Interfaces;
using StructureMap;
using Surveillance;
using Surveillance.Auditing;
using Surveillance.Auditing.Context;
using Surveillance.Auditing.DataLayer;
using Surveillance.Auditing.DataLayer.Interfaces;
using Surveillance.Auditing.DataLayer.Processes;
using Surveillance.DataLayer;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.Engine.DataCoordinator;
using Surveillance.Engine.DataCoordinator.Configuration.Interfaces;
using Surveillance.Engine.RuleDistributor;
using Surveillance.Engine.Rules;
using Utilities.Aws_IO.Interfaces;

namespace RedDeer.Surveillance.App
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var container = new Container();
            
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var configBuilder = new Configuration.Configuration();
            var dbConfiguration = configBuilder.BuildDatabaseConfiguration(configurationBuilder);
            container.Inject(typeof(IDataLayerConfiguration), dbConfiguration);
            container.Inject(typeof(IAwsConfiguration), dbConfiguration);
            container.Inject(typeof(IRuleConfiguration), configBuilder.BuildRuleConfiguration(configurationBuilder));
            container.Inject(typeof(ISystemDataLayerConfig), configBuilder.BuildDataLayerConfig(configurationBuilder));
            SystemProcessContext.ProcessType = SystemProcessType.SurveillanceService;

            container.Configure(config =>
            {
                config.IncludeRegistry<SystemSystemDataLayerRegistry>();
                config.IncludeRegistry<SurveillanceSystemAuditingRegistry>();
                config.IncludeRegistry<DataLayerRegistry>();
                config.IncludeRegistry<SurveillanceRegistry>();
                config.IncludeRegistry<RuleDistributorRegistry>();
                config.IncludeRegistry<DataCoordinatorRegistry>();
                config.IncludeRegistry<RuleRegistry>();
                config.IncludeRegistry<AppRegistry>();
                config.Populate(services);
            });

            container.GetInstance<IScriptRunner>();

            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Red Deer Surveillance Service App");
            });
        }
    }
}
