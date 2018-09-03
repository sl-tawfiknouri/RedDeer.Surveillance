using System.Threading;
using Microsoft.Extensions.Configuration;
using NLog;

namespace TestHarness.App
{
    public class Program
    {
        static void Main(string[] args)
        {
            var networkConfiguration = BuildConfiguration();

            LogManager.LoadConfiguration("nlog.config");
            Bootstrapper.Start(networkConfiguration);

            Thread.Sleep(50);
        }

        private static Configuration.Configuration BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var networkConfiguration = new Configuration.Configuration
            {
                TradeWebsocketUriPort = configurationBuilder.GetValue<string>("TradeWebsocketUriPort"),
                TradeWebsocketUriDomain = configurationBuilder.GetValue<string>("TradeWebsocketUriDomain"),
                StockExchangeDomainUriDomainSegment = configurationBuilder.GetValue<string>("StockExchangeDomainUriDomain"),
                StockExchangeDomainUriPort = configurationBuilder.GetValue<string>("StockExchangeDomainUriPort"),
            };

            return networkConfiguration;
        }
    }
}