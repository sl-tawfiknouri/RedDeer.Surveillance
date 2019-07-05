﻿using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;

namespace Surveillance.Reddeer.ApiClient.Configuration
{
    public class ApiClientConfiguration : IApiClientConfiguration
    {
        public bool IsEc2Instance { get; set; }
        public string DataSynchroniserRequestQueueName { get; set; }
        public string ScheduledRuleQueueName { get; set; }
        public string CaseMessageQueueName { get; set; }
        public string ScheduleRuleDistributedWorkQueueName { get; set; }
        public string UploadCoordinatorQueueName { get; set; }
        public string TestRuleRunUpdateQueueName { get; set; }
        public string EmailServiceSendEmailQueueName { get; set; }
        public string SurveillanceUserApiAccessToken { get; set; }
        public string ClientServiceUrl { get; set; }
        public string BmllServiceUrl { get; set; }
        public string AuroraConnectionString { get; set; }
        public string ScheduleRuleCancellationQueueName { get; set; }
        public string ScheduleDelayedRuleRunQueueName { get; set; }
    }
}
