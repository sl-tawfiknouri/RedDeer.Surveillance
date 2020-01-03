namespace DataImport.Disk_IO.AllocationFile
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using DataImport.Configuration.Interfaces;
    using DataImport.Disk_IO.AllocationFile.Interfaces;
    using DataImport.MessageBusIO.Interfaces;

    using Domain.Core.Trading.Orders;

    using Infrastructure.Network.Disk.Interfaces;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Queues;
    using SharedKernel.Files.Allocations;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Auditing.DataLayer.Processes;
    using Surveillance.Auditing.DataLayer.Processes.Interfaces;
    using Surveillance.DataLayer.Aurora.Files.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders.Interfaces;

    public class AllocationFileMonitor : BaseUploadFileMonitor, IUploadAllocationFileMonitor
    {
        private readonly IAllocationFileProcessor _allocationFileProcessor;

        private readonly IOrderAllocationRepository _allocationRepository;

        private readonly IFileUploadOrderAllocationRepository _fileUploadRepository;

        private readonly object _lock = new object();

        private readonly IUploadCoordinatorMessageSender _messageSender;

        private readonly ISystemProcessContext _systemProcessContext;

        private readonly IUploadConfiguration _uploadConfiguration;

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
            this._allocationFileProcessor = fileProcessor ?? throw new ArgumentNullException(nameof(fileProcessor));
            this._systemProcessContext =
                systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            this._uploadConfiguration =
                uploadConfiguration ?? throw new ArgumentNullException(nameof(uploadConfiguration));
            this._allocationRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            this._fileUploadRepository =
                fileUploadRepository ?? throw new ArgumentNullException(nameof(fileUploadRepository));
            this._messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        }

        public override bool ProcessFile(string path)
        {
            this.Logger.LogInformation("AllocationFileMonitor Process File waiting to acquire lock");

            var opCtx = this._systemProcessContext.CreateAndStartOperationContext();
            var fileUpload = opCtx.CreateAndStartUploadFileContext(
                SystemProcessOperationUploadFileType.AllocationDataFile,
                path);

            if (string.IsNullOrWhiteSpace(path))
            {
                this.Logger.LogInformation(
                    "AllocationFileMonitor Process File had a null or empty file path. Returning false.");
                fileUpload.EndEvent()
                    .EndEventWithError("AllocationFileMonitor asked to process a null or empty file path");
                return false;
            }

            lock (this._lock)
            {
                this.Logger.LogInformation("AllocationFileMonitor Process File acquired lock");

                try
                {
                    var csvReadResults = this._allocationFileProcessor.Process(path);

                    if (csvReadResults == null
                        || !csvReadResults.SuccessfulReads.Any() && !csvReadResults.UnsuccessfulReads.Any())
                    {
                        this.Logger.LogError(
                            $"AllocationFileMonitor for {path} did not find any records or had zero successful and unsuccessful reads. Empty File. CLIENTSERVICE");
                        fileUpload.EndEvent().EndEvent();
                        return false;
                    }

                    if (csvReadResults.UnsuccessfulReads.Any())
                    {
                        this.Logger.LogInformation(
                            $"AllocationFileMonitor had unsuccessful reads {csvReadResults.UnsuccessfulReads.Count}");
                        this.FailedRead(path, csvReadResults, fileUpload);
                        return false;
                    }

                    this.Logger.LogInformation(
                        $"AllocationFileMonitor had successful reads {csvReadResults.SuccessfulReads.Count}");
                    this.SuccessfulRead(path, csvReadResults, fileUpload);
                    return true;
                }
                catch (Exception e)
                {
                    this.Logger.LogError(e, $"AllocationFileMonitor Process File encountered an exception and will be returning false. {path}");

                    fileUpload.EndEvent().EndEventWithError(e.Message);
                    return false;
                }
            }
        }

        protected override string UploadDirectoryPath()
        {
            this.Logger.LogInformation(
                $"AllocationFileMonitor UploadDirectoryPath() return directory path {this._uploadConfiguration.DataImportAllocationFileUploadDirectoryPath}");

            return this._uploadConfiguration.DataImportAllocationFileUploadDirectoryPath;
        }

        private void FailedRead(
            string path,
            UploadFileProcessorResult<AllocationFileContract, OrderAllocation> csvReadResults,
            ISystemProcessOperationUploadFileContext fileUpload)
        {
            var originatingFileName = Path.GetFileNameWithoutExtension(path);

            this.Logger.LogError(
                $"AllocationFileMonitor had errors when processing file {path} and has {csvReadResults.UnsuccessfulReads.Count} failed uploads. About to write records to logs.");

            foreach (var row in csvReadResults.UnsuccessfulReads)
                this.Logger.LogInformation(
                    $"AllocationFileMonitor could not parse row {row.RowId} of {originatingFileName}.");

            this.Logger.LogInformation(
                $"AllocationFileMonitor for {path} has errors and will not commit to further processing. Now about to delete {path}.");
            this.ReddeerDirectory.Delete(path);
            this.Logger.LogInformation($"AllocationFileMonitor for {path} has deleted the file.");

            fileUpload.EndEvent().EndEventWithError(
                $"AllocationFileMonitor had failed reads ({csvReadResults.UnsuccessfulReads.Count}) written to disk {this.GetFailedReadsPath()}");
        }

        private void LinkFileUploadDataToUpload(
            ISystemProcessOperationUploadFile fileUploadId,
            IReadOnlyCollection<string> allocationIds)
        {
            if (allocationIds == null || !allocationIds.Any())
            {
                this.Logger.LogInformation(
                    "AllocationFileMonitor had no inserted allocation ids to link the file upload to.");
                return;
            }

            if (fileUploadId?.Id != null)
            {
                this._fileUploadRepository.Create(allocationIds, fileUploadId.Id).Wait();

                var uploadMessage = new AutoScheduleMessage();

                this._messageSender.Send(uploadMessage).Wait();
            }
            else
            {
                this.Logger.LogInformation("AllocationFileMonitor received a null or empty file upload id. Exiting");
            }
        }

        private void SuccessfulRead(
            string path,
            UploadFileProcessorResult<AllocationFileContract, OrderAllocation> csvReadResults,
            ISystemProcessOperationUploadFileContext fileUpload)
        {
            this.Logger.LogInformation(
                $"AllocationFileMonitor for {path} is about to submit {csvReadResults.SuccessfulReads?.Count} records to the trade upload stream");
            var allocationIds = this._allocationRepository.Create(csvReadResults.SuccessfulReads).Result;
            this.LinkFileUploadDataToUpload(fileUpload?.FileUpload, allocationIds);
            this.Logger.LogInformation(
                $"AllocationFileMonitor for {path} has uploaded the csv records. Now about to delete {path}.");
            this.ReddeerDirectory.Delete(path);
            this.Logger.LogInformation(
                $"AllocationFileMonitor for {path} has deleted the file. Now about to check for unsuccessful reads.");

            this.Logger.LogInformation(
                $"AllocationFileMonitor successfully processed file for {path}. Did not find any unsuccessful reads.");
            fileUpload?.EndEvent()?.EndEvent();
        }
    }
}