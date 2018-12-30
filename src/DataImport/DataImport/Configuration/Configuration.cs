using DataImport.Configuration.Interfaces;
using Surveillance.System.DataLayer.Interfaces;

namespace DataImport.Configuration
{
    public class Configuration :
        ISystemDataLayerConfig,
        IUploadConfiguration
    {     
        public string RelayTradeFileUploadDirectoryPath { get; set; }
        public string RelayTradeFileFtpDirectoryPath { get; set; }
        public string RelayEquityFileFtpDirectoryPath { get; set; }

        public string RelayEquityFileUploadDirectoryPath { get; set; }
        public string RelayS3UploadQueueName { get; set; }
        public bool AutoSchedule { get; set; }
        public string SurveillanceAuroraConnectionString { get; set; }
    
        // data layer
        public string ScheduledRuleQueueName { get; set; }
        public string CaseMessageQueueName { get; set; }
        public string ScheduleRuleDistributedWorkQueueName { get; set; }
        public string AuroraConnectionString { get; set; }
        public string SurveillanceUserApiAccessToken { get; set; }
        public string ClientServiceUrl { get; set; }
    }
}
