namespace DataImport.Disk_IO.TradeFile
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using DataImport.Configuration.Interfaces;
    using DataImport.Disk_IO.Interfaces;
    using DataImport.Disk_IO.TradeFile.Interfaces;
    using DataImport.MessageBusIO.Interfaces;
    using DataImport.Services.Interfaces;

    using Domain.Core.Trading.Orders;

    using Infrastructure.Network.Disk.Interfaces;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Contracts.Queues;
    using SharedKernel.Files.Orders;
    using SharedKernel.Files.Orders.Interfaces;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Auditing.DataLayer.Processes;
    using Surveillance.DataLayer.Aurora.Files.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders.Interfaces;

    public class UploadTradeFileMonitor : BaseUploadFileMonitor, IUploadTradeFileMonitor
    {
        private readonly IEnrichmentService _enrichmentService;

        private readonly IUploadTradeFileProcessor _fileProcessor;

        private readonly IUploadCoordinatorMessageSender _fileUploadMessageSender;

        private readonly IFileUploadOrdersRepository _fileUploadOrdersRepository;

        private readonly object _lock = new object();

        private readonly ILogger _logger;

        private readonly IOmsVersioner _omsVersioner;

        private readonly IOrdersRepository _ordersRepository;

        private readonly ISystemProcessContext _systemProcessContext;

        private readonly IUploadConfiguration _uploadConfiguration;

        public UploadTradeFileMonitor(
            IUploadConfiguration uploadConfiguration,
            IReddeerDirectory directory,
            IUploadTradeFileProcessor fileProcessor,
            IEnrichmentService enrichmentService,
            IOrdersRepository ordersRepository,
            IFileUploadOrdersRepository fileUploadOrdersRepository,
            IUploadCoordinatorMessageSender fileUploadMessageSender,
            ISystemProcessContext systemProcessContext,
            IOmsVersioner omsVersioner,
            ILogger<UploadTradeFileMonitor> logger)
            : base(directory, logger, "Upload Trade File Monitor")
        {
            this._uploadConfiguration =
                uploadConfiguration ?? throw new ArgumentNullException(nameof(uploadConfiguration));
            this._fileProcessor = fileProcessor ?? throw new ArgumentNullException(nameof(fileProcessor));
            this._enrichmentService = enrichmentService ?? throw new ArgumentNullException(nameof(enrichmentService));
            this._ordersRepository = ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
            this._fileUploadOrdersRepository = fileUploadOrdersRepository
                                               ?? throw new ArgumentNullException(nameof(fileUploadOrdersRepository));
            this._fileUploadMessageSender = fileUploadMessageSender
                                            ?? throw new ArgumentNullException(nameof(fileUploadMessageSender));
            this._systemProcessContext =
                systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
            this._omsVersioner = omsVersioner ?? throw new ArgumentNullException(nameof(omsVersioner));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override bool ProcessFile(string path)
        {
            lock (this._lock)
            {
                this._logger.LogInformation($"Upload Trade File beginning process file for {path}");

                var opCtx = this._systemProcessContext.CreateAndStartOperationContext();
                var fileUpload = opCtx.CreateAndStartUploadFileContext(
                    SystemProcessOperationUploadFileType.OrderDataFile,
                    path);
                try
                {
                    var csvReadResults = this._fileProcessor.Process(path);

                    if (csvReadResults == null
                        || !csvReadResults.SuccessfulReads.Any() && !csvReadResults.UnsuccessfulReads.Any())
                    {
                        this._logger.LogError(
                            $"Upload Trade File for {path} did not find any records or had zero successful and unsuccessful reads. Empty file. CLIENTSERVICE");
                        fileUpload.EndEvent().EndEvent();
                        return false;
                    }

                    if (csvReadResults.UnsuccessfulReads.Any())
                    {
                        this._logger.LogInformation(
                            $"Upload Trade File Monitor had unsuccessful reads {csvReadResults.UnsuccessfulReads.Count}");
                        this.FailedRead(path, csvReadResults, fileUpload);
                        return false;
                    }

                    this._logger.LogInformation(
                        $"Upload Trade File Monitor had successful reads {csvReadResults.SuccessfulReads.Count}");
                    this.SuccessfulRead(path, csvReadResults, fileUpload);
                    return true;
                }
                catch (Exception e)
                {
                    this._logger.LogError(
                        $"Upload Trade File Monitor encountered an exception in process file for {path}",
                        e);
                    fileUpload.EndEvent().EndEventWithError(e.Message);

                    return false;
                }
            }
        }

        protected override string UploadDirectoryPath()
        {
            return this._uploadConfiguration.DataImportTradeFileUploadDirectoryPath;
        }

        private void FailedRead(
            string path,
            UploadFileProcessorResult<OrderFileContract, Order> csvReadResults,
            ISystemProcessOperationUploadFileContext fileUpload)
        {
            var originatingFileName = Path.GetFileNameWithoutExtension(path);

            this._logger.LogError(
                $"Upload Trade File had errors when processing file {path} and has {csvReadResults.UnsuccessfulReads.Count} failed uploads. About to write records to logs.");

            foreach (var row in csvReadResults.UnsuccessfulReads)
                this._logger.LogInformation(
                    $"UploadTradeFileMonitor could not parse row {row.RowId} of {originatingFileName}.");

            this._logger.LogInformation(
                $"Upload Trade File for {path} has errors and will not commit to further processing. Now about to delete {path}.");
            this.ReddeerDirectory.Delete(path);
            this._logger.LogInformation($"Upload Trade File for {path} has deleted the file.");

            fileUpload.EndEvent().EndEventWithError(
                $"Had failed reads ({csvReadResults.UnsuccessfulReads.Count}) written to disk {this.GetFailedReadsPath()}");
        }

        private void InsertFileUploadOrderIds(
            IReadOnlyCollection<Order> orders,
            ISystemProcessOperationUploadFileContext fileUpload)
        {
            if (orders == null || !orders.Any()) return;

            if (fileUpload?.FileUpload?.Id == null) return;

            var orderIds = orders.Select(i => i.ReddeerOrderId?.ToString()).Where(i => !string.IsNullOrWhiteSpace(i))
                .ToList();

            this._logger.LogInformation(
                $"Upload Trade File for {fileUpload.FileUpload.Id} has uploaded the {orderIds.Count} csv records. Now about to save the link between the file upload and orders");
            this._fileUploadOrdersRepository.Create(orderIds, fileUpload.FileUpload.Id).Wait();
            this._logger.LogInformation(
                $"Upload Trade File for {fileUpload.FileUpload.Id} has uploaded the {orderIds.Count} csv records. Completed saving the link between the file upload and orders");

            var uploadMessage = new AutoScheduleMessage();
            this._fileUploadMessageSender.Send(uploadMessage).Wait();
        }

        private void SuccessfulRead(
            string path,
            UploadFileProcessorResult<OrderFileContract, Order> csvReadResults,
            ISystemProcessOperationUploadFileContext fileUpload)
        {
            var uploadGuid = Guid.NewGuid().ToString();
            var orders = this._omsVersioner.ProjectOmsVersion(csvReadResults.SuccessfulReads) ?? new Order[0];
            this._logger.LogInformation(
                $"Upload Trade File for {path} is about to submit {orders?.Count} records to the orders repository.");

            foreach (var item in orders)
            {
                item.IsInputBatch = true;
                item.InputBatchId = uploadGuid;
                item.BatchSize = orders.Count;
                this._ordersRepository.Create(item).Wait();
            }

            this._logger.LogInformation(
                $"Upload Trade File for {path} has uploaded the csv records. Now about to link uploaded orders to file upload id.");
            this.InsertFileUploadOrderIds(orders, fileUpload);
            this._logger.LogInformation(
                $"Upload Trade File for {path} has uploaded the csv records. Now about to enrich the security data.");
            this._enrichmentService.Scan();
            this._logger.LogInformation(
                $"Upload Trade File for {path} has enriched the csv records. Now about to delete {path}.");
            this.ReddeerDirectory.Delete(path);
            this._logger.LogInformation(
                $"Upload Trade File for {path} has deleted the file. Now about to check for unsuccessful reads.");

            this._logger.LogInformation(
                $"Upload Trade File successfully processed file for {path}. Did not find any unsuccessful reads.");
            fileUpload.EndEvent().EndEvent();
        }
    }
}