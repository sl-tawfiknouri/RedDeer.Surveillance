using Utilities.Aws_IO.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Configuration
{
    public class Config : IAwsConfiguration
    {
        public string DataSynchroniserRequestQueueName { get; set; }
        public string ScheduledRuleQueueName { get; set; }
        public string CaseMessageQueueName { get; set; }
        public string ScheduleRuleDistributedWorkQueueName { get; set; }
        public string AuroraConnectionString { get; set; }
    }
}
