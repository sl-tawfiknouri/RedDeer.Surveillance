using System;
using DataImport;
using DataImport.Configuration;
using DataImport.Configuration.Interfaces;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using StructureMap;
using Surveillance.Auditing;
using Surveillance.Auditing.Context;
using Surveillance.Auditing.DataLayer;
using Surveillance.Auditing.DataLayer.Interfaces;
using Surveillance.Auditing.DataLayer.Processes;
using Surveillance.DataLayer;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.Reddeer.ApiClient;
using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;

namespace RedDeer.DataImport.DataImport.App
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
            container.Inject(typeof(IUploadConfiguration), builtConfig);
            container.Inject(typeof(ISystemDataLayerConfig), builtConfig);
            SystemProcessContext.ProcessType = SystemProcessType.DataImportService;

            var builtDataConfig = BuildDataConfiguration();
            container.Inject(typeof(IAwsConfiguration), builtDataConfig);
            container.Inject(typeof(IDataLayerConfiguration), builtDataConfig);

            var builtClientApiConfig = BuildApiClientConfiguration();
            container.Inject(typeof(IApiClientConfiguration), builtClientApiConfig);

            container.Configure(config =>
            {
                config.IncludeRegistry<DataImportRegistry>();
                config.IncludeRegistry<AppRegistry>();
                config.IncludeRegistry<SystemSystemDataLayerRegistry>();
                config.IncludeRegistry<SurveillanceSystemAuditingRegistry>();
                config.IncludeRegistry<DataLayerRegistry>();
                config.IncludeRegistry<ReddeerApiClientRegistry>();
                config.Populate(services);
            });

            container.Inject(typeof(ISystemDataLayerConfig), builtConfig);
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
                await context.Response.WriteAsync("Red Deer Surveillance Data Import Application");
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

        private static IDataLayerConfiguration BuildDataConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var builder = new ConfigBuilder.ConfigBuilder();
            return builder.BuildData(configurationBuilder);
        }

        private static IApiClientConfiguration BuildApiClientConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var builder = new ConfigBuilder.ConfigBuilder();
            return builder.BuildApi(configurationBuilder);
        }
    }
}