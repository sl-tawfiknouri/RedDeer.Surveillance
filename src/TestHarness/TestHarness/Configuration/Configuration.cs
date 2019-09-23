namespace TestHarness.Configuration
{
    using TestHarness.Configuration.Interfaces;

    public class Configuration : INetworkConfiguration
    {
        public string AuroraConnectionString { get; set; }

        public string CaseMessageDeadLetterQueueName { get; set; }

        public string CaseMessageQueueName { get; set; }

        public string ClientServiceUrl { get; set; }

        public string DataSynchroniserRequestQueueName { get; set; }

        public string EmailServiceSendEmailQueueName { get; set; }

        public bool IsEc2Instance { get; set; }

        public string ScheduleDelayedRuleRunQueueName { get; set; }

        public string ScheduledRuleDeadLetterQueueName { get; set; }

        public string ScheduledRuleQueueName { get; set; }

        public string ScheduleRuleCancellationQueueName { get; set; }

        public string ScheduleRuleDistributedWorkDeadLetterQueueName { get; set; }

        public string ScheduleRuleDistributedWorkQueueName { get; set; }

        public string SurveillanceUserApiAccessToken { get; set; }

        public string TestRuleRunUpdateQueueName { get; set; }

        public string UploadCoordinatorQueueName { get; set; }
    }
}