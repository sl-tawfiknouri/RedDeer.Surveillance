using Microsoft.Extensions.Configuration;
using Surveillance.Configuration;
using Surveillance.Configuration.Interfaces;
using Surveillance.DataLayer.Configuration;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.System.DataLayer;
using Surveillance.System.DataLayer.Interfaces;

namespace RedDeer.Surveillance.App.Configuration
{
    public class Configuration
    {
        public INetworkConfiguration BuildNetworkConfiguration(IConfigurationRoot configurationBuilder)
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

        public IDataLayerConfiguration BuildDatabaseConfiguration(IConfigurationRoot configurationBuilder)
        {
            var networkConfiguration = new DataLayerConfiguration
            {
                IsEc2Instance = configurationBuilder.GetValue<bool?>("IsEc2Instance") ?? false,
                ScheduledRuleQueueName = configurationBuilder.GetValue<string>("ScheduledRuleQueueName"),
                ScheduleRuleDistributedWorkQueueName = configurationBuilder.GetValue<string>("ScheduleRuleDistributedWorkQueueName"),
                CaseMessageQueueName = configurationBuilder.GetValue<string>("CaseMessageQueueName"),
                ElasticSearchProtocol = configurationBuilder.GetValue<string>("ElasticSearchProtocol"),
                ElasticSearchDomain = configurationBuilder.GetValue<string>("ElasticSearchDomain"),
                ElasticSearchPort = configurationBuilder.GetValue<string>("ElasticSearchPort"),
                ClientServiceUrl = configurationBuilder.GetValue<string>("ClientServiceUrl"),
                SurveillanceUserApiAccessToken = configurationBuilder.GetValue<string>("SurveillanceUserApiAccessToken")
            };

            return networkConfiguration;
        }

        public IRuleConfiguration BuildRuleConfiguration(IConfigurationRoot configurationBuilder)
        {
            var ruleConfiguration = new RuleConfiguration
            {
                CancelledOrderDeduplicationDelaySeconds = configurationBuilder.GetValue<int?>("CancelledOrderDeduplicationDelaySeconds"),
                AutoScheduleRules = configurationBuilder.GetValue<bool?>("AutoScheduleRules")
            };

            return ruleConfiguration;
        }

        public ISystemDataLayerConfig BuildDataLayerConfig(IConfigurationRoot configurationBuilder)
        {
            var ruleConfiguration = new SystemDataLayerConfig
            {
                SurveillanceAuroraConnectionString = configurationBuilder.GetValue<string>("AuroraConnectionString")
            };

            return ruleConfiguration;
        }
    }
}
