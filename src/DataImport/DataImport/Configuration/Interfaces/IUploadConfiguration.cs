namespace DataImport.Configuration.Interfaces
{
    public interface IUploadConfiguration
    {
        string DataImportTradeFileUploadDirectoryPath { get; }
        string DataImportTradeFileFtpDirectoryPath { get; }

        string DataImportEquityFileUploadDirectoryPath { get; }
        string DataImportEquityFileFtpDirectoryPath { get; }

        string DataImportS3UploadQueueName { get; }

        bool AutoSchedule { get; }
    }
}
