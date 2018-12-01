using DataImport.Configuration.Interfaces;

namespace DataImport.Configuration
{
    public class Configuration : 
        INetworkConfiguration,
        IUploadConfiguration
    {
        public string RelayServiceEquityDomain { get; set; }
        public string RelayServiceEquityPort { get; set; }

        public string RelayServiceTradeDomain { get; set; }
        public string RelayServiceTradePort { get; set; }

        public string RelayTradeFileUploadDirectoryPath { get; set; }
        public string RelayTradeFileFtpDirectoryPath { get; set; }
        public string RelayEquityFileFtpDirectoryPath { get; set; }

        public string RelayEquityFileUploadDirectoryPath { get; set; }
        public string RelayS3UploadQueueName { get; set; }
        public bool AutoSchedule { get; set; }

        public string SurveillanceAuroraConnectionString { get; set; }

        public string IsDeployedOnClientMachine { get; set; }
    
        // data layer
        public string ScheduledRuleQueueName { get; set; }
        public string CaseMessageQueueName { get; set; }
        public string ScheduleRuleDistributedWorkQueueName { get; set; }
        public string AuroraConnectionString { get; set; }
        public string SurveillanceUserApiAccessToken { get; set; }
        public string ClientServiceUrl { get; set; }
    }
}
