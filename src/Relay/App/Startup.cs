using System;
using System.IO;
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

            container.Configure(config =>
            {
                config.IncludeRegistry<RelayRegistry>();
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
                await context.Response.WriteAsync("Red Deer Surveillance Relay App");
            });
        }

        private static Configuration BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var networkConfiguration = new Configuration
            {
                RelayServiceEquityDomain = configurationBuilder.GetValue<string>("RelayServiceEquityDomain"),
                RelayServiceEquityPort = configurationBuilder.GetValue<string>("RelayServiceEquityPort"),
                SurveillanceServiceEquityDomain = configurationBuilder.GetValue<string>("SurveillanceServiceEquityDomain"),
                SurveillanceServiceEquityPort = configurationBuilder.GetValue<string>("SurveillanceServiceEquityPort"),

                RelayServiceTradeDomain = configurationBuilder.GetValue<string>("RelayServiceTradeDomain"),
                RelayServiceTradePort = configurationBuilder.GetValue<string>("RelayServiceTradePort"),
                SurveillanceServiceTradeDomain = configurationBuilder.GetValue<string>("SurveillanceServiceTradeDomain"),
                SurveillanceServiceTradePort = configurationBuilder.GetValue<string>("SurveillanceServiceTradePort"),

                RelayTradeFileUploadDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), configurationBuilder.GetValue<string>("RelayTradeFileUploadDirectoryPath")),
                RelayEquityFileUploadDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(),
                    configurationBuilder.GetValue<string>("RelayEquityFileUploadDirectoryPath")),

                StatusChangedOnFieldName = configurationBuilder.GetValue<string>("StatusChangedOnFieldName"),
                MarketIdFieldName = configurationBuilder.GetValue<string>("MarketIdFieldName"),
                MarketAbbreviationFieldName = configurationBuilder.GetValue<string>("MarketAbbreviationFieldName"),
                MarketNameFieldName = configurationBuilder.GetValue<string>("MarketNameFieldName"),
                SecurityNameFieldName = configurationBuilder.GetValue<string>("SecurityNameFieldName"),
                OrderTypeFieldName = configurationBuilder.GetValue<string>("OrderTypeFieldName"),
                OrderDirectionFieldName = configurationBuilder.GetValue<string>("OrderDirectionFieldName"),
                OrderStatusFieldName = configurationBuilder.GetValue<string>("OrderStatusFieldName"),
                VolumeFieldName = configurationBuilder.GetValue<string>("VolumeFieldName"),
                LimitPriceFieldName = configurationBuilder.GetValue<string>("LimitPriceFieldName"),
                SecurityClientIdentifierFieldName = configurationBuilder.GetValue<string>("SecurityClientIdentifierFieldName"),
                SecuritySedolFieldName = configurationBuilder.GetValue<string>("SecuritySedolFieldName"),
                SecurityIsinFieldName = configurationBuilder.GetValue<string>("SecurityIsinFieldName"),
                SecurityFigiFieldName = configurationBuilder.GetValue<string>("SecurityFigiFieldName"),


                SecurityTickTimestampFieldName = configurationBuilder.GetValue<string>("SecurityTickTimestampFieldName"),
                SecurityTickMarketIdentifierCodeFieldName = configurationBuilder.GetValue<string>("SecurityTickMarketIdentifierCodeFieldName"),
                SecurityTickMarketNameFieldName = configurationBuilder.GetValue<string>("SecurityTickMarketNameFieldName"),
                SecurityTickClientIdentifierFieldName = configurationBuilder.GetValue<string>("SecurityTickClientIdentifierFieldName"),
                SecurityTickSedolFieldName = configurationBuilder.GetValue<string>("SecurityTickSedolFieldName"),
                SecurityTickIsinFieldName = configurationBuilder.GetValue<string>("SecurityTickIsinFieldName"),
                SecurityTickFigiFieldName = configurationBuilder.GetValue<string>("SecurityTickFigiFieldName"),
                SecurityTickCfiFieldName = configurationBuilder.GetValue<string>("SecurityTickCifiFieldName"),
                SecurityTickTickerSymbolFieldName = configurationBuilder.GetValue<string>("SecurityTickTickerSymbolFieldName"),
                SecurityTickSecurityNameFieldName = configurationBuilder.GetValue<string>("SecurityTickSecurityNameFieldName"),
                SecurityTickSpreadAskFieldName = configurationBuilder.GetValue<string>("SecurityTickSpreadAskFieldName"),
                SecurityTickSpreadBidFieldName = configurationBuilder.GetValue<string>("SecurityTickSpreadBidFieldName"),
                SecurityTickSpreadPriceFieldName = configurationBuilder.GetValue<string>("SecurityTickSpreadPriceFieldName"),
                SecurityTickVolumeTradedFieldName = configurationBuilder.GetValue<string>("SecurityTickVolumeTradedFieldName"),
                SecurityTickMarketCapFieldName = configurationBuilder.GetValue<string>("SecurityTickMarketCapFieldName"),
                SecurityTickSpreadCurrencyFieldName = configurationBuilder.GetValue<string>("SecurityTickSpreadCurrencyFieldName"),
            };

            return networkConfiguration;
        }
    }
}
