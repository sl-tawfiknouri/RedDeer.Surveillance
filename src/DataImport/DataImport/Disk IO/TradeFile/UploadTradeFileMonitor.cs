using System;
using System.IO;
using System.Linq;
using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.Interfaces;
using DataImport.Disk_IO.TradeFile.Interfaces;
using DataImport.MessageBusIO.Interfaces;
using DataImport.Services.Interfaces;
using Domain.Contracts;
using Domain.Trading;
using Microsoft.Extensions.Logging;
using SharedKernel.Files.Orders;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Auditing.DataLayer.Processes;
using Surveillance.DataLayer.Aurora.Files.Interfaces;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;
using Utilities.Disk_IO.Interfaces;

namespace DataImport.Disk_IO.TradeFile
{
    public class UploadTradeFileMonitor : BaseUploadFileMonitor, IUploadTradeFileMonitor
    {
        private readonly IUploadConfiguration _uploadConfiguration;
        private readonly IUploadTradeFileProcessor _fileProcessor;
        private readonly IEnrichmentService _enrichmentService;
        private readonly IOrdersRepository _ordersRepository;
        private readonly IFileUploadOrdersRepository _fileUploadOrdersRepository;
        private readonly IUploadCoordinatorMessageSender _fileUploadMessageSender;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly ILogger _logger;
        private readonly object _lock = new object();

        public UploadTradeFileMonitor(
            IUploadConfiguration uploadConfiguration,
            IReddeerDirectory directory,
            IUploadTradeFileProcessor fileProcessor,
            IEnrichmentService enrichmentService,
            IOrdersRepository ordersRepository,
            IFileUploadOrdersRepository fileUploadOrdersRepository,
            IUploadCoordinatorMessageSender fileUploadMessageSender,
            ISystemProcessContext systemProcessContext,
            ILogger<UploadTradeFileMonitor> logger) 
            : base(directory, logger, "Upload Trade File Monitor")
        {
            _uploadConfiguration = uploadConfiguration ?? throw new ArgumentNullException(nameof(uploadConfiguration));
            _fileProcessor = fileProcessor ?? throw new ArgumentNullException(nameof(fileProcessor));
            _enrichmentService = enrichmentService ?? throw new ArgumentNullException(nameof(enrichmentService));
            _ordersRepository = ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
            _fileUploadOrdersRepository = fileUploadOrdersRepository ?? throw new ArgumentNullException(nameof(fileUploadOrdersRepository));
            _fileUploadMessageSender = fileUploadMessageSender ?? throw new ArgumentNullException(nameof(fileUploadMessageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _systemProcessContext = systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
        }

        protected override string UploadDirectoryPath()
        {
            return _uploadConfiguration.DataImportTradeFileUploadDirectoryPath;
        }

        public override bool ProcessFile(string path)
        {
            lock (_lock)
            {
                _logger.LogInformation($"Upload Trade File beginning process file for {path}");

                var opCtx = _systemProcessContext.CreateAndStartOperationContext();
                var fileUpload =
                    opCtx
                        .CreateAndStartUploadFileContext(
                            SystemProcessOperationUploadFileType.OrderDataFile,
                            path);
                try
                {
                    var csvReadResults = _fileProcessor.Process(path);

                    if (csvReadResults == null
                        || (!csvReadResults.SuccessfulReads.Any() && !(csvReadResults.UnsuccessfulReads.Any())))
                    {
                        _logger.LogError($"Upload Trade File for {path} did not find any records or had zero successful and unsuccessful reads. Empty file. CLIENTSERVICE");
                        fileUpload.EndEvent().EndEvent();
                        return false;
                    }

                    if (csvReadResults.UnsuccessfulReads.Any())
                    {
                        _logger.LogInformation($"Upload Trade File Monitor had unsuccessful reads {csvReadResults.UnsuccessfulReads.Count}");
                        FailedRead(path, csvReadResults, fileUpload);
                        return false;
                    }
                    else
                    {
                        _logger.LogInformation($"Upload Trade File Monitor had successful reads {csvReadResults.SuccessfulReads.Count}");
                        SuccessfulRead(path, csvReadResults, fileUpload);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"Upload Trade File Monitor encountered an exception in process file for {path}", e);
                    fileUpload.EndEvent().EndEventWithError(e.Message);

                    return false;
                }
            }
        }

        private void FailedRead(
            string path,
            UploadFileProcessorResult<OrderFileContract, Order> csvReadResults,
            ISystemProcessOperationUploadFileContext fileUpload)
        {
            var originatingFileName = Path.GetFileNameWithoutExtension(path);

            _logger.LogError($"Upload Trade File had errors when processing file {path} and has {csvReadResults.UnsuccessfulReads.Count} failed uploads. About to write records to logs.");

            foreach (var row in csvReadResults.UnsuccessfulReads)
            {
                _logger.LogInformation($"UploadTradeFileMonitor could not parse row {row.RowId} of {originatingFileName}.");
            }

            _logger.LogInformation($"Upload Trade File for {path} has errors and will not commit to further processing. Now about to delete {path}.");
            ReddeerDirectory.Delete(path);
            _logger.LogInformation($"Upload Trade File for {path} has deleted the file.");

            fileUpload.EndEvent().EndEventWithError($"Had failed reads ({csvReadResults.UnsuccessfulReads.Count}) written to disk {GetFailedReadsPath()}");
        }

        private void SuccessfulRead(
            string path,
            UploadFileProcessorResult<OrderFileContract, Order> csvReadResults,
            ISystemProcessOperationUploadFileContext fileUpload)
        {
            var uploadGuid = Guid.NewGuid().ToString();
            _logger.LogInformation($"Upload Trade File for {path} is about to submit {csvReadResults.SuccessfulReads?.Count} records to the orders repository.");

            foreach (var item in csvReadResults.SuccessfulReads)
            {
                item.IsInputBatch = true;
                item.InputBatchId = uploadGuid;
                item.BatchSize = csvReadResults.SuccessfulReads.Count;
                _ordersRepository.Create(item).Wait();
            }

            _logger.LogInformation($"Upload Trade File for {path} has uploaded the csv records. Now about to link uploaded orders to file upload id.");
            InsertFileUploadOrderIds(csvReadResults, fileUpload);
            _logger.LogInformation($"Upload Trade File for {path} has uploaded the csv records. Now about to enrich the security data.");
            _enrichmentService.Scan();
            _logger.LogInformation($"Upload Trade File for {path} has enriched the csv records. Now about to delete {path}.");
            ReddeerDirectory.Delete(path);
            _logger.LogInformation($"Upload Trade File for {path} has deleted the file. Now about to check for unsuccessful reads.");

            _logger.LogInformation($"Upload Trade File successfully processed file for {path}. Did not find any unsuccessful reads.");
            fileUpload.EndEvent().EndEvent();
        }

        private void InsertFileUploadOrderIds(
            UploadFileProcessorResult<OrderFileContract, Order> csvReadResults,
            ISystemProcessOperationUploadFileContext fileUpload)
        {
            if (csvReadResults.SuccessfulReads == null
                || !csvReadResults.SuccessfulReads.Any())
            {
                return;
            }

            if (fileUpload?.FileUpload?.Id == null)
            {
                return;
            }

            var orderIds = 
                csvReadResults
                    .SuccessfulReads
                    .Select(i => i.ReddeerOrderId?.ToString())
                    .Where(i => !string.IsNullOrWhiteSpace(i))
                    .ToList();

            _logger.LogInformation($"Upload Trade File for {fileUpload.FileUpload.Id} has uploaded the {orderIds.Count} csv records. Now about to save the link between the file upload and orders");
            _fileUploadOrdersRepository.Create(orderIds, fileUpload.FileUpload.Id).Wait();
            _logger.LogInformation($"Upload Trade File for {fileUpload.FileUpload.Id} has uploaded the {orderIds.Count} csv records. Completed saving the link between the file upload and orders");

            var uploadMessage = new AutoScheduleMessage();
            _fileUploadMessageSender.Send(uploadMessage).Wait();
        }
    }
}