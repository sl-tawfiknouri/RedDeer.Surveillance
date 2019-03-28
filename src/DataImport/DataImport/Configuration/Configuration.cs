using DataImport.Configuration.Interfaces;
using Surveillance.Auditing.DataLayer.Interfaces;

namespace DataImport.Configuration
{
    public class Configuration :
        ISystemDataLayerConfig,
        IUploadConfiguration
    {     
        public string DataImportTradeFileUploadDirectoryPath { get; set; }
        public string DataImportTradeFileFtpDirectoryPath { get; set; }
        public string DataImportEquityFileFtpDirectoryPath { get; set; }

        public string DataImportEquityFileUploadDirectoryPath { get; set; }
        public string DataImportS3UploadQueueName { get; set; }

        public string DataImportAllocationFileUploadDirectoryPath { get; set; }
        public string DataImportAllocationFileFtpDirectoryPath { get; set; }

        public string DataImportEtlFileFtpDirectoryPath { get; set; }
        public string DataImportEtlFileUploadDirectoryPath { get; set; }

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
