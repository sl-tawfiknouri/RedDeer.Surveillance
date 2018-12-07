namespace DataImport.Configuration.Interfaces
{
    public interface IUploadConfiguration
    {
        string RelayTradeFileUploadDirectoryPath { get; }
        string RelayTradeFileFtpDirectoryPath { get; }

        string RelayEquityFileUploadDirectoryPath { get; }
        string RelayEquityFileFtpDirectoryPath { get; }

        string RelayS3UploadQueueName { get; }

        bool AutoSchedule { get; }
    }
}
