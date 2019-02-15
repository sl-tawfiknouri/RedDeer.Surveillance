using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.AllocationFile.Interfaces;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataImport.MessageBusIO.Interfaces;
using DomainV2.Contracts;
using DomainV2.Files;
using DomainV2.Files.AllocationFile;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Auditing.DataLayer.Processes;
using Surveillance.Auditing.DataLayer.Processes.Interfaces;
using Surveillance.DataLayer.Aurora.Files.Interfaces;
using Surveillance.DataLayer.Aurora.Trade.Interfaces;
using Utilities.Disk_IO.Interfaces;

namespace DataImport.Disk_IO.AllocationFile
{
    public class AllocationFileMonitor : BaseUploadFileMonitor, IUploadAllocationFileMonitor
    {
        private readonly IAllocationFileProcessor _allocationFileProcessor;
        private readonly IUploadConfiguration _uploadConfiguration;
        private readonly IOrderAllocationRepository _allocationRepository;
        private readonly IFileUploadOrderAllocationRepository _fileUploadRepository;
        private readonly IUploadCoordinatorMessageSender _messageSender;
        private readonly ISystemProcessContext _systemProcessContext;

        private readonly object _lock = new object();

        public AllocationFileMonitor(
            IAllocationFileProcessor fileProcessor,
            ISystemProcessContext systemProcessContext,
            IUploadConfiguration uploadConfiguration,
            IReddeerDirectory directory,
            IOrderAllocationRepository repository,
            IFileUploadOrderAllocationRepository fileUploadRepository,
            IUploadCoordinatorMessageSender messageSender,
            ILogger<AllocationFileMonitor> logger)
            : base(directory, logger, "Allocation File Monitor")
        {
            _allocationFileProcessor = fileProcessor ?? throw new ArgumentNullException(nameof(fileProcessor));
            _systemProcessContext = systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            _uploadConfiguration = uploadConfiguration ?? throw new ArgumentNullException(nameof(uploadConfiguration));
            _allocationRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _fileUploadRepository = fileUploadRepository ?? throw new ArgumentNullException(nameof(fileUploadRepository));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
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
                    var csvReadResults = _allocationFileProcessor.Process(path);

                    if (csvReadResults == null
                        || (!csvReadResults.SuccessfulReads.Any() && !(csvReadResults.UnsuccessfulReads.Any())))
                    {
                        Logger.LogError($"AllocationFileMonitor for {path} did not find any records or had zero successful and unsuccessful reads. Empty File. CLIENTSERVICE");
                        fileUpload.EndEvent().EndEvent();
                        return false;
                    }

                    if (csvReadResults.UnsuccessfulReads.Any())
                    {
                        Logger.LogInformation($"AllocationFileMonitor had unsuccessful reads {csvReadResults.UnsuccessfulReads.Count}");
                        FailedRead(path, csvReadResults, fileUpload);
                        return false;
                    }
                    else
                    {
                        Logger.LogInformation($"AllocationFileMonitor had successful reads {csvReadResults.SuccessfulReads.Count}");
                        SuccessfulRead(path, csvReadResults, fileUpload);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError($"AllocationFileMonitor Process File encountered an exception and will be returning false. {path}", e);

                    fileUpload.EndEvent().EndEventWithError(e.Message);
                    return false;
                }
            }
        }

        private void FailedRead(
            string path,
            UploadFileProcessorResult<AllocationFileCsv, OrderAllocation> csvReadResults,
            ISystemProcessOperationUploadFileContext fileUpload)
        {
            var originatingFileName = Path.GetFileNameWithoutExtension(path);

            Logger.LogError($"AllocationFileMonitor had errors when processing file {path} and has {csvReadResults.UnsuccessfulReads.Count} failed uploads. About to write records to logs.");

            foreach (var row in csvReadResults.UnsuccessfulReads)
            {
                Logger.LogInformation($"AllocationFileMonitor could not parse row {row.RowId} of {originatingFileName}.");
            }

            Logger.LogInformation($"AllocationFileMonitor for {path} has errors and will not commit to further processing. Now about to delete {path}.");
            ReddeerDirectory.Delete(path);
            Logger.LogInformation($"AllocationFileMonitor for {path} has deleted the file.");

            fileUpload.EndEvent().EndEventWithError($"AllocationFileMonitor had failed reads ({csvReadResults.UnsuccessfulReads.Count}) written to disk {GetFailedReadsPath()}");
        }

        private void SuccessfulRead(
            string path,
            UploadFileProcessorResult<AllocationFileCsv, OrderAllocation> csvReadResults,
            ISystemProcessOperationUploadFileContext fileUpload)
        {
            Logger.LogInformation($"AllocationFileMonitor for {path} is about to submit {csvReadResults.SuccessfulReads?.Count} records to the trade upload stream");
            var allocationIds = _allocationRepository.Create(csvReadResults.SuccessfulReads).Result;
            LinkFileUploadDataToUpload(fileUpload?.FileUpload, allocationIds);
            Logger.LogInformation($"AllocationFileMonitor for {path} has uploaded the csv records. Now about to delete {path}.");
            ReddeerDirectory.Delete(path);
            Logger.LogInformation($"AllocationFileMonitor for {path} has deleted the file. Now about to check for unsuccessful reads.");

            Logger.LogInformation($"AllocationFileMonitor successfully processed file for {path}. Did not find any unsuccessful reads.");
            fileUpload?.EndEvent()?.EndEvent();
        }

        private void LinkFileUploadDataToUpload(ISystemProcessOperationUploadFile fileUploadId, IReadOnlyCollection<string> allocationIds)
        {
            if (allocationIds == null
                || !allocationIds.Any())
            {
                Logger.LogInformation($"AllocationFileMonitor had no inserted allocation ids to link the file upload to.");
                return;
            }

            if (fileUploadId?.Id != null)
            {
                _fileUploadRepository.Create(allocationIds, fileUploadId.Id).Wait();

                var uploadMessage = new AutoScheduleMessage();

                _messageSender.Send(uploadMessage).Wait();
            }
            else
            {
                Logger.LogInformation($"AllocationFileMonitor received a null or empty file upload id. Exiting");
            }
        }
    }
}
