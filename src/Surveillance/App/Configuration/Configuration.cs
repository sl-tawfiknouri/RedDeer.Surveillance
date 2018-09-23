﻿using Microsoft.Extensions.Configuration;
using Surveillance.Configuration;
using Surveillance.Configuration.Interfaces;
using Surveillance.DataLayer.Configuration;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace RedDeer.Surveillance.App.Configuration
{
    public static class Configuration
    {
        public static INetworkConfiguration BuildNetworkConfiguration(IConfigurationRoot configurationBuilder)
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

        public static IElasticSearchConfiguration BuildDatabaseConfiguration(IConfigurationRoot configurationBuilder)
        {
            var networkConfiguration = new ElasticSearchConfiguration
            {
                IsEc2Instance = configurationBuilder.GetValue<bool?>("IsEc2Instance") ?? false,
                AwsSecretKey = configurationBuilder.GetValue<string>("AwsSecretKey"),
                AwsAccessKey = configurationBuilder.GetValue<string>("AwsAccessKey"),
                ScheduledRuleQueueName = configurationBuilder.GetValue<string>("ScheduledRuleQueueName"),
                CaseMessageQueueName = configurationBuilder.GetValue<string>("CaseMessageQueueName"),
                ElasticSearchProtocol = configurationBuilder.GetValue<string>("ElasticSearchProtocol"),
                ElasticSearchDomain = configurationBuilder.GetValue<string>("ElasticSearchDomain"),
                ElasticSearchPort = configurationBuilder.GetValue<string>("ElasticSearchPort")
            };

            return networkConfiguration;
        }

        public static IRuleConfiguration BuildRuleConfiguration(IConfigurationRoot configurationBuilder)
        {
            var ruleConfiguration = new RuleConfiguration
            {
                CancelledOrderDeduplicationDelaySeconds = configurationBuilder.GetValue<int?>("CancelledOrderDeduplicationDelaySeconds")
            };

            return ruleConfiguration;
        }
    }
}
