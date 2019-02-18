using Surveillance.Auditing.DataLayer.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace DataSynchroniser.Configuration
{
    public class Config : IAwsConfiguration, ISystemDataLayerConfig, IDataLayerConfiguration
    {
        public string DataSynchroniserRequestQueueName { get; set; }
        public string ScheduledRuleQueueName { get; set; }
        public string CaseMessageQueueName { get; set; }
        public string ScheduleRuleDistributedWorkQueueName { get; set; }
        public string UploadCoordinatorQueueName { get; set; }
        public string TestRuleRunUpdateQueueName { get; set; }
        public string AuroraConnectionString { get; set; }
        public string SurveillanceAuroraConnectionString { get; set; }

        public string SurveillanceUserApiAccessToken { get; set; }
        public string ClientServiceUrl { get; set; }
        public string BmllServiceUrl { get; set; }
    }
}
