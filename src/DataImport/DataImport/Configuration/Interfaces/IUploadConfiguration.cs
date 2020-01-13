namespace DataImport.Configuration.Interfaces
{
    public interface IUploadConfiguration
    {
        bool AutoSchedule { get; }

        string DataImportAllocationFileFtpDirectoryPath { get; }

        string DataImportAllocationFileUploadDirectoryPath { get; }

        string DataImportEquityFileFtpDirectoryPath { get; }

        string DataImportEquityFileUploadDirectoryPath { get; }

        string DataImportEtlFailureNotifications { get; }

        string DataImportEtlFileFtpDirectoryPath { get; }

        string DataImportEtlFileUploadDirectoryPath { get; }

        string DataImportS3UploadQueueName { get; }

        string DataImportTradeFileFtpDirectoryPath { get; }

        string DataImportTradeFileUploadDirectoryPath { get; }

        string DataImportTradeFileDirectoryPattern { get; }

        string DataImportAllocationFileDirectoryPattern { get; }

        string DataImportEtlFileDirectoryPattern { get; }
        
        string DataImportIgnoreFileDirectoryPattern { get; }
    }
}