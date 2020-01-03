using Surveillance.Data.Universe.Refinitiv.Interfaces;

namespace DataImport.Configuration
{
    using DataImport.Configuration.Interfaces;

    using Surveillance.Auditing.DataLayer.Interfaces;

    public class Configuration : ISystemDataLayerConfig, IUploadConfiguration, IRefinitivTickPriceHistoryApiConfig
    {
        public string AuroraConnectionString { get; set; }

        public string OverrideMigrationsFolder { get; set; }

        public bool AutoSchedule { get; set; }

        public string CaseMessageQueueName { get; set; }

        public string ClientServiceUrl { get; set; }

        public string DataImportAllocationFileFtpDirectoryPath { get; set; }

        public string DataImportAllocationFileUploadDirectoryPath { get; set; }

        public string DataImportEquityFileFtpDirectoryPath { get; set; }

        public string DataImportEquityFileUploadDirectoryPath { get; set; }

        public string DataImportEtlFailureNotifications { get; set; }

        public string DataImportEtlFileFtpDirectoryPath { get; set; }

        public string DataImportEtlFileUploadDirectoryPath { get; set; }

        public string DataImportS3UploadQueueName { get; set; }

        public string DataImportTradeFileFtpDirectoryPath { get; set; }

        public string DataImportTradeFileUploadDirectoryPath { get; set; }

        // data layer
        public string ScheduledRuleQueueName { get; set; }

        public string ScheduleRuleDistributedWorkQueueName { get; set; }

        public string SurveillanceAuroraConnectionString { get; set; }

        public string SurveillanceUserApiAccessToken { get; set; }
        
        public string RefinitivTickPriceHistoryApiAddress { get; set; }

        public string RefinitivTickPriceHistoryApiJwtBearerTokenSymetricSecurityKey { get; set; }

        public int RefinitivTickPriceHistoryApiPollingSeconds { get; set; }
        
        public int RefinitivTickPriceHistoryApiTimeOutDurationSeconds { get; set; }
    }
}