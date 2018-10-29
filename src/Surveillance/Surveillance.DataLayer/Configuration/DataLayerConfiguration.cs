using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Configuration
{
    public class DataLayerConfiguration : IDataLayerConfiguration
    {
        public bool IsEc2Instance { get; set; }
        public string ScheduledRuleQueueName { get; set; }
        public string ElasticSearchDomain { get; set; }
        public string ElasticSearchPort { get; set; }
        public string ElasticSearchProtocol { get; set; }
        public string CaseMessageQueueName { get; set; }
        public string ScheduleRuleDistributedWorkQueueName { get; set; }
        public string SurveillanceUserApiAccessToken { get; set; }
        public string ClientServiceUrl { get; set; }
        public string AuroraConnectionString { get; set; }

        // Dead letter queues
        public string ScheduledRuleDeadLetterQueueName { get; set; }
        public string CaseMessageDeadLetterQueueName { get; set; }
        public string ScheduleRuleDistributedWorkDeadLetterQueueName { get; set; }
    }
}
