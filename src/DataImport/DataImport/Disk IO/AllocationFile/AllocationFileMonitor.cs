using DataImport.Configuration.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes;
using System;
using Utilities.Disk_IO.Interfaces;

namespace DataImport.Disk_IO.AllocationFile
{
    public class AllocationFileMonitor : BaseUploadFileMonitor, IUploadAllocationFileMonitor
    {
        private readonly IUploadConfiguration _uploadConfiguration;
        private readonly ISystemProcessContext _systemProcessContext;

        private readonly object _lock = new object();

        public AllocationFileMonitor(
            ISystemProcessContext systemProcessContext,
            IUploadConfiguration uploadConfiguration,
            IReddeerDirectory directory,
            ILogger<AllocationFileMonitor> logger)
            : base(directory, logger, "Allocation File Monitor")
        {
            _systemProcessContext = systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            _uploadConfiguration = uploadConfiguration ?? throw new ArgumentNullException(nameof(uploadConfiguration));
        }

        protected override string UploadDirectoryPath()
        {
            Logger.LogInformation($"AllocationFileMonitor UploadDirectoryPath() return directory path {_uploadConfiguration.DataImportAllocationFileUploadDirectoryPath}");

            return _uploadConfiguration.DataImportAllocationFileUploadDirectoryPath;
        }

        public override bool ProcessFile(string path)
        {
            Logger.LogInformation($"AllocationFileMonitor Process File waiting to acquire lock");

            var opCtx = _systemProcessContext.CreateAndStartOperationContext();
            var fileUpload = opCtx.CreateAndStartUploadFileContext(SystemProcessOperationUploadFileType.AllocationDataFile, path);

            if (string.IsNullOrWhiteSpace(path))
            {
                Logger.LogInformation($"AllocationFileMonitor Process File had a null or empty file path. Returning false.");
                fileUpload.EndEvent().EndEventWithError("AllocationFileMonitor asked to process a null or empty file path");
                return false;
            }

            lock (_lock)
            {
                Logger.LogInformation($"AllocationFileMonitor Process File acquired lock");

                try
                {



                }
                catch (Exception e)
                {
                    Logger.LogError($"AllocationFileMonitor Process File encountered an exception and will be returning false. {path}", e);

                    fileUpload.EndEvent().EndEventWithError(e.Message);
                    return false;
                }

                Logger.LogInformation($"AllocationFileMonitor Process File returning true");
                fileUpload.EndEvent();
                return true;
            }
        }
    }
}
