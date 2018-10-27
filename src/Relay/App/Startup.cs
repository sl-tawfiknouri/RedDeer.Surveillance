using System;
using Domain.Equity.Frames.Interfaces;
using Domain.Trades.Orders.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Relay;
using Relay.Configuration;
using Relay.Configuration.Interfaces;
using StructureMap;
using Surveillance.System.Auditing;
using Surveillance.System.DataLayer;
using Surveillance.System.DataLayer.Interfaces;

namespace RedDeer.Relay.Relay.App
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var container = new Container();

            var builtConfig = BuildConfiguration();
            container.Inject(typeof(INetworkConfiguration), builtConfig);
            container.Inject(typeof(IUploadConfiguration), builtConfig);
            container.Inject(typeof(ITradeOrderCsvConfig), builtConfig);
            container.Inject(typeof(ISecurityTickCsvConfig), builtConfig);
            container.Inject(typeof(ISystemDataLayerConfig), builtConfig);

            container.Configure(config =>
            {
                config.IncludeRegistry<RelayRegistry>();
                config.IncludeRegistry<AppRegistry>();
                config.IncludeRegistry<SystemSystemDataLayerRegistry>();
                config.IncludeRegistry<SurveillanceSystemAuditingRegistry>();
                config.Populate(services);
            });

            container.Inject(typeof(ISystemDataLayerConfig), builtConfig);

            return container.GetInstance<IServiceProvider>();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Red Deer Surveillance Relay App");
            });
        }

        private static Configuration BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var builder = new ConfigBuilder.ConfigBuilder();
            return builder.Build(configurationBuilder);
        }
    }
}