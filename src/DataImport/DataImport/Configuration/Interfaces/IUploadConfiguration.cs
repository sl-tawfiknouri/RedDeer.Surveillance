namespace DataImport.Configuration.Interfaces
{
    public interface IUploadConfiguration
    {
        string DataImportTradeFileUploadDirectoryPath { get; }
        string DataImportTradeFileFtpDirectoryPath { get; }

        string DataImportEquityFileUploadDirectoryPath { get; }
        string DataImportEquityFileFtpDirectoryPath { get; }
        string DataImportEtlFailureNotifications { get; }

        string DataImportAllocationFileUploadDirectoryPath { get; }
        string DataImportAllocationFileFtpDirectoryPath { get; }

        string DataImportEtlFileUploadDirectoryPath { get; }
        string DataImportEtlFileFtpDirectoryPath { get; }

        string DataImportS3UploadQueueName { get; }

        bool AutoSchedule { get; }
    }
}
