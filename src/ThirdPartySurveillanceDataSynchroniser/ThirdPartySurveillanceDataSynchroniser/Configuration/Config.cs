namespace DataSynchroniser.Configuration
{
    using Infrastructure.Network.Aws.Interfaces;

    using Surveillance.Auditing.DataLayer.Interfaces;
    using Surveillance.DataLayer.Configuration.Interfaces;
    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;

    public class Config : IAwsConfiguration, ISystemDataLayerConfig, IDataLayerConfiguration, IApiClientConfiguration
    {
        public string AuroraConnectionString { get; set; }

        public string BmllServiceUrl { get; set; }

        public string CaseMessageQueueName { get; set; }

        public string ClientServiceUrl { get; set; }

        public string DataSynchroniserRequestQueueName { get; set; }

        public string EmailServiceSendEmailQueueName { get; set; }

        public string ScheduleDelayedRuleRunQueueName { get; set; }

        public string ScheduledRuleQueueName { get; set; }

        public string ScheduleRuleCancellationQueueName { get; set; }

        public string ScheduleRuleDistributedWorkQueueName { get; set; }

        public string SurveillanceAuroraConnectionString { get; set; }

        public string SurveillanceUserApiAccessToken { get; set; }

        public string TestRuleRunUpdateQueueName { get; set; }

        public string UploadCoordinatorQueueName { get; set; }
    }
}