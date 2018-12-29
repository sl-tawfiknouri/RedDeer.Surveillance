using TestHarness.Configuration.Interfaces;

namespace TestHarness.Configuration
{
    public class Configuration : INetworkConfiguration
    {
        public string ScheduledRuleQueueName {get; set;}
        public string CaseMessageQueueName { get; set; }
        public string ScheduleRuleDistributedWorkQueueName { get; set; }
        public string ScheduledRuleDeadLetterQueueName { get; set; }
        public string CaseMessageDeadLetterQueueName { get; set; }
        public string ScheduleRuleDistributedWorkDeadLetterQueueName { get; set; }
        public string AuroraConnectionString { get; set; }

        public bool IsEc2Instance { get; set; }

        public string ClientServiceUrl { get; set; }
        public string SurveillanceUserApiAccessToken { get; set; }
    }
}
