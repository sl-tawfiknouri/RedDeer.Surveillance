﻿using Microsoft.Extensions.Configuration;

namespace TestHarness.App
{
    public static class ConfigurationHelper
    {
        public static  Configuration.Configuration BuildNetworkConfiguration(IConfigurationRoot configurationBuilder)
        {
            var networkConfiguration = new Configuration.Configuration
            {
                IsEc2Instance = configurationBuilder.GetValue<bool?>("IsEc2Instance") ?? false,
                ScheduledRuleQueueName = configurationBuilder.GetValue<string>("ScheduledRuleQueueName"),
                CaseMessageQueueName = configurationBuilder.GetValue<string>("CaseMessageQueueName"),
                ScheduleRuleDistributedWorkQueueName = configurationBuilder.GetValue<string>("ScheduleRuleDistributedWorkQueueName"),
                AuroraConnectionString = configurationBuilder.GetValue<string>("AuroraConnectionString"),
                ClientServiceUrl = configurationBuilder.GetValue<string>("ClientServiceUrl"),
                SurveillanceUserApiAccessToken = configurationBuilder.GetValue<string>("SurveillanceUserApiAccessToken"),
                DataSynchroniserRequestQueueName = configurationBuilder.GetValue<string>("DataSynchronizerQueueName")
            };

            return networkConfiguration;
        }
    }
}
