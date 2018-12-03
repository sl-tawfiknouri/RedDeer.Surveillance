using System;
using System.IO;
using System.Linq;
using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.Interfaces;
using DataImport.Disk_IO.TradeFile.Interfaces;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using DomainV2.Files;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes;
using Utilities.Disk_IO.Interfaces;

namespace DataImport.Disk_IO.TradeFile
{
    public class UploadTradeFileMonitor : BaseUploadFileMonitor, IUploadTradeFileMonitor
    {
        private readonly ITradeOrderStream<Order> _stream;
        private readonly IUploadConfiguration _uploadConfiguration;
        private readonly IUploadTradeFileProcessor _fileProcessor;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly ILogger _logger;
        private readonly object _lock = new object();

        public UploadTradeFileMonitor(
            ITradeOrderStream<Order> stream,
            IUploadConfiguration uploadConfiguration,
            IReddeerDirectory directory,
            IUploadTradeFileProcessor fileProcessor,
            ISystemProcessContext systemProcessContext,
            ILogger<UploadTradeFileMonitor> logger) 
            : base(directory, logger, "Upload Trade File Monitor")
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _uploadConfiguration = uploadConfiguration ?? throw new ArgumentNullException(nameof(uploadConfiguration));
            _fileProcessor = fileProcessor ?? throw new ArgumentNullException(nameof(fileProcessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _systemProcessContext = systemProcessContext ?? throw new ArgumentNullException(nameof(systemProcessContext));
        }

        protected override string UploadDirectoryPath()
        {
            return _uploadConfiguration.RelayTradeFileUploadDirectoryPath;
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
                            SystemProcessOperationUploadFileType.TradeDataFile,
                            path);
                try
                {
                    var csvReadResults = _fileProcessor.Process(path);

                    if (csvReadResults == null
                        || (!csvReadResults.SuccessfulReads.Any() && !(csvReadResults.UnsuccessfulReads.Any())))
                    {
                        _logger.LogError($"Upload Trade File for {path} did not find any records or had zero successful and unsuccessful reads");
                        fileUpload.EndEvent().EndEvent();
                        return false;
                    }

                    if (csvReadResults.UnsuccessfulReads.Any())
                    {
                        FailedRead(path, csvReadResults, fileUpload);
                        return false;
                    }
                    else
                    {
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
            UploadFileProcessorResult<TradeOrderFrameCsv, TradeOrderFrame> csvReadResults,
            ISystemProcessOperationUploadFileContext fileUpload)
        {
            var originatingFileName = Path.GetFileNameWithoutExtension(path);

            _logger.LogError($"Upload Trade File had errors when processing file for {path} and has {csvReadResults.UnsuccessfulReads.Count} failed uploads. About to write records to logs.");

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
            UploadFileProcessorResult<TradeFileCsv, Order> csvReadResults,
            ISystemProcessOperationUploadFileContext fileUpload)
        {
            var uploadGuid = Guid.NewGuid().ToString();
            _logger.LogInformation($"Upload Trade File for {path} is about to submit {csvReadResults.SuccessfulReads?.Count} records to the trade upload stream");

            foreach (var item in csvReadResults.SuccessfulReads)
            {
                item.IsInputBatch = true;
                item.InputBatchId = uploadGuid;
                item.BatchSize = csvReadResults.SuccessfulReads.Count;

                _stream.Add(item);
            }
            _logger.LogInformation($"Upload Trade File for {path} has uploaded the csv records. Now about to delete {path}.");
            ReddeerDirectory.Delete(path);
            _logger.LogInformation($"Upload Trade File for {path} has deleted the file. Now about to check for unsuccessful reads.");

            _logger.LogInformation($"Upload Trade File successfully processed file for {path}. Did not find any unsuccessful reads.");
            fileUpload.EndEvent().EndEvent();
        }
    }
}