using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using Surveillance;
using Surveillance.Configuration;
using Surveillance.Configuration.Interfaces;
using Surveillance.DataLayer;
using Surveillance.DataLayer.Configuration;
using Surveillance.DataLayer.Configuration.Interfaces;
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

            var dbConfiguration = BuildDatabaseConfiguration(configurationBuilder);
            container.Inject(typeof(INetworkConfiguration), BuildNetworkConfiguration(configurationBuilder));
            container.Inject(typeof(IElasticSearchConfiguration), dbConfiguration);
            container.Inject(typeof(IAwsConfiguration), dbConfiguration);

            container.Configure(config =>
            {
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

        private static INetworkConfiguration BuildNetworkConfiguration(IConfigurationRoot configurationBuilder)
        {
            var networkConfiguration = new NetworkConfiguration
            {
                SurveillanceServiceEquityDomain = configurationBuilder.GetValue<string>("SurveillanceServiceEquityDomain"),
                SurveillanceServiceEquityPort = configurationBuilder.GetValue<string>("SurveillanceServiceEquityPort"),

                SurveillanceServiceTradeDomain = configurationBuilder.GetValue<string>("SurveillanceServiceTradeDomain"),
                SurveillanceServiceTradePort = configurationBuilder.GetValue<string>("SurveillanceServiceTradePort"),
            };

            return networkConfiguration;
        }

        private static IElasticSearchConfiguration BuildDatabaseConfiguration(IConfigurationRoot configurationBuilder)
        {
            var networkConfiguration = new ElasticSearchConfiguration
            {
                IsEc2Instance = configurationBuilder.GetValue<bool?>("IsEc2Instance") ?? false,
                AwsSecretKey = configurationBuilder.GetValue<string>("AwsSecretKey"),
                AwsAccessKey = configurationBuilder.GetValue<string>("AwsAccessKey"),
                ScheduledRuleQueueName = configurationBuilder.GetValue<string>("ScheduledRuleQueueName"),
                ElasticSearchProtocol = configurationBuilder.GetValue<string>("ElasticSearchProtocol"),
                ElasticSearchDomain = configurationBuilder.GetValue<string>("ElasticSearchDomain"),
                ElasticSearchPort = configurationBuilder.GetValue<string>("ElasticSearchPort")
            };

            return networkConfiguration;
        }
    }
}
