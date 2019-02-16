using System;
using System.Linq;
using System.Threading.Tasks;
using DataImport.File_Scanner.Interfaces;
using DomainV2.Files;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.DataLayer.Repositories.Interfaces;

namespace DataImport.File_Scanner
{
    public class FileScanner : IFileScanner
    {
        private readonly ISystemProcessOperationUploadFileRepository _uploadFileRepository;
        private readonly ILogger<FileScanner> _logger;

        public FileScanner(
            ISystemProcessOperationUploadFileRepository uploadFileRepository,
            ILogger<FileScanner> logger)
        {
            _uploadFileRepository = uploadFileRepository ?? throw new ArgumentNullException(nameof(uploadFileRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Scan()
        {
            _logger.LogInformation($"FileScanner scan initiated");

            var yesterday = DateTime.UtcNow.AddDays(-1);
            var fileUploads = await _uploadFileRepository.GetOnDate(yesterday);

            var uploadedOrdersFile = fileUploads.Any(i => i.FileType == (int) UploadedFileType.OrdersFile);
            var uploadedAllocationsFile = fileUploads.Any(i => i.FileType == (int)UploadedFileType.AllocationFile);

            if (uploadedOrdersFile && uploadedAllocationsFile)
            {
                _logger.LogInformation($"FileScanner scan found both Orders and Allocations files");
                return;
            }

            if (!uploadedOrdersFile)
            {
                _logger.LogError($"FileScanner Scan found a missing Orders File for {yesterday.Date}. Expected to find a file. CLIENTSERVICES");
            }

            if (!uploadedOrdersFile)
            {
                _logger.LogError($"FileScanner Scan found a missing Order Allocations File for {yesterday.Date}. Expected to find a file. CLIENTSERVICES");
            }
        }
    }
}
