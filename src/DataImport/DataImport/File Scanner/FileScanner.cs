namespace DataImport.File_Scanner
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using DataImport.File_Scanner.Interfaces;

    using Microsoft.Extensions.Logging;

    using Surveillance.Auditing.DataLayer.Processes;
    using Surveillance.Auditing.DataLayer.Repositories.Interfaces;

    public class FileScanner : IFileScanner
    {
        private readonly ILogger<FileScanner> _logger;

        private readonly ISystemProcessOperationUploadFileRepository _uploadFileRepository;

        public FileScanner(
            ISystemProcessOperationUploadFileRepository uploadFileRepository,
            ILogger<FileScanner> logger)
        {
            this._uploadFileRepository =
                uploadFileRepository ?? throw new ArgumentNullException(nameof(uploadFileRepository));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Scan()
        {
            this._logger.LogInformation("FileScanner scan initiated");

            var yesterday = DateTime.UtcNow.AddDays(-1);
            var fileUploads = await this._uploadFileRepository.GetOnDate(yesterday);

            var uploadedEtlFile =
                fileUploads.Any(i => i.FileType == (int)SystemProcessOperationUploadFileType.EtlDataFile);
            var uploadedOrdersFile =
                fileUploads.Any(i => i.FileType == (int)SystemProcessOperationUploadFileType.OrderDataFile);
            var uploadedAllocationsFile = fileUploads.Any(
                i => i.FileType == (int)SystemProcessOperationUploadFileType.AllocationDataFile);

            if ((uploadedOrdersFile || uploadedEtlFile) && uploadedAllocationsFile)
            {
                this._logger.LogInformation("FileScanner scan found both Orders and Allocations files");
                return;
            }

            if (!uploadedOrdersFile && !uploadedEtlFile)
                this._logger.LogError(
                    $"FileScanner Scan found a missing Orders File for {yesterday.Date}. Expected to find a file. CLIENTSERVICES");

            if (!uploadedAllocationsFile)
                this._logger.LogError(
                    $"FileScanner Scan found a missing Order Allocations File for {yesterday.Date}. Expected to find a file. CLIENTSERVICES");
        }
    }
}