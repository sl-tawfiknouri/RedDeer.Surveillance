﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using Surveillance;
using Surveillance.Configuration.Interfaces;
using Surveillance.DataLayer;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.System.Auditing;
using Surveillance.System.DataLayer;
using Surveillance.System.DataLayer.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace RedDeer.Surveillance.App
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var container = new Container();
            
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var configBuilder = new Configuration.Configuration();
            var dbConfiguration = configBuilder.BuildDatabaseConfiguration(configurationBuilder);
            container.Inject(typeof(INetworkConfiguration), configBuilder.BuildNetworkConfiguration(configurationBuilder));
            container.Inject(typeof(IDataLayerConfiguration), dbConfiguration);
            container.Inject(typeof(IAwsConfiguration), dbConfiguration);
            container.Inject(typeof(IRuleConfiguration), configBuilder.BuildRuleConfiguration(configurationBuilder));
            container.Inject(typeof(ISystemDataLayerConfig), configBuilder.BuildDataLayerConfig(configurationBuilder));

            container.Configure(config =>
            {
                config.IncludeRegistry<SystemSystemDataLayerRegistry>();
                config.IncludeRegistry<SurveillanceSystemAuditingRegistry>();
                config.IncludeRegistry<DataLayerRegistry>();
                config.IncludeRegistry<SurveillanceRegistry>();
                config.IncludeRegistry<AppRegistry>();
                config.Populate(services);
            });


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
                await context.Response.WriteAsync("Red Deer Surveillance Service App");
            });
        }
    }
}
