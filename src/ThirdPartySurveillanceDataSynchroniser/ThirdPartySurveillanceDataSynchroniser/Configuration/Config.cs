using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.System.DataLayer.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Configuration
{
    public class Config : IAwsConfiguration, ISystemDataLayerConfig, IDataLayerConfiguration
    {
        public string DataSynchroniserRequestQueueName { get; set; }
        public string ScheduledRuleQueueName { get; set; }
        public string CaseMessageQueueName { get; set; }
        public string ScheduleRuleDistributedWorkQueueName { get; set; }
        public string AuroraConnectionString { get; set; }
        public string SurveillanceAuroraConnectionString { get; set; }


        public string SurveillanceUserApiAccessToken { get; set; }
        public string ClientServiceUrl { get; set; }
        public string BmllServiceUrl { get; set; }
    }
}
