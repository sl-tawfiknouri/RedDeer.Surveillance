﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using StructureMap;
using ThirdPartySurveillanceDataSynchroniser;
using ThirdPartySurveillanceDataSynchroniser.Configuration;

namespace RedDeer.ThirdPartySurveillanceDataSynchroniser.App
{
    public class Startup
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Logger.Log(LogLevel.Info, $"StartUp configure services registering dependency injection");
            var container = new Container();

            var builtConfig = BuildConfiguration();
            //container.Inject(typeof(ISystemDataLayerConfig), builtConfig);
           // SystemProcessContext.ProcessType = SystemProcessType.RelayService;

            // var builtDataConfig = BuildDataConfiguration();
            // container.Inject(typeof(IAwsConfiguration), builtDataConfig);
            // container.Inject(typeof(IDataLayerConfiguration), builtDataConfig);

            container.Configure(config =>
            {
                config.IncludeRegistry<DataSynchroniserRegistry>();
                config.IncludeRegistry<AppRegistry>();
                config.Populate(services);
            });

            // container.Inject(typeof(ISystemDataLayerConfig), builtConfig);
            Logger.Log(LogLevel.Info, $"Startup configure services completed registering dependency injection");

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
                await context.Response.WriteAsync("Red Deer Third Party Surveillance Data Synchroniser Application");
            });
        }

        private static Config BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var builder = new ConfigBuilder.ConfigBuilder();
            return builder.Build(configurationBuilder);
        }

        //private static IDataLayerConfiguration BuildDataConfiguration()
        //{
        //    var configurationBuilder = new ConfigurationBuilder()
        //        .AddJsonFile("appsettings.json", true, true)
        //        .Build();

        //    var builder = new ConfigBuilder.ConfigBuilder();
        //    return builder.BuildData(configurationBuilder);
        //}
    }
}