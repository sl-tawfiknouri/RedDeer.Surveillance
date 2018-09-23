using Microsoft.Extensions.Configuration;

namespace TestHarness.App
{
    public static class ConfigurationHelper
    {
        public static  Configuration.Configuration BuildNetworkConfiguration(IConfigurationRoot configurationBuilder)
        {
            var networkConfiguration = new Configuration.Configuration
            {
                TradeWebsocketUriPort = configurationBuilder.GetValue<string>("TradeWebsocketUriPort"),
                TradeWebsocketUriDomain = configurationBuilder.GetValue<string>("TradeWebsocketUriDomain"),
                StockExchangeDomainUriDomainSegment = configurationBuilder.GetValue<string>("StockExchangeDomainUriDomain"),
                StockExchangeDomainUriPort = configurationBuilder.GetValue<string>("StockExchangeDomainUriPort"),
                IsEc2Instance = configurationBuilder.GetValue<bool?>("IsEc2Instance") ?? false,
                AwsSecretKey = configurationBuilder.GetValue<string>("AwsSecretKey"),
                AwsAccessKey = configurationBuilder.GetValue<string>("AwsAccessKey"),
                ScheduledRuleQueueName = configurationBuilder.GetValue<string>("ScheduledRuleQueueName"),
                CaseMessageQueueName = configurationBuilder.GetValue<string>("CaseMessageQueueName"),
            };

            return networkConfiguration;
        }
    }
}
