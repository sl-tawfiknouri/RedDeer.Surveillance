using System;
using System.IO;
using System.Linq;
using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Relay.Configuration.Interfaces;
using Relay.Disk_IO.Interfaces;
using Relay.Disk_IO.TradeFile.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes;
using Utilities.Disk_IO.Interfaces;

namespace Relay.Disk_IO.TradeFile
{
    public class UploadTradeFileMonitor : BaseUploadFileMonitor, IUploadTradeFileMonitor
    {
        private readonly ITradeOrderStream<TradeOrderFrame> _stream;
        private readonly IUploadConfiguration _uploadConfiguration;
        private readonly IUploadTradeFileProcessor _fileProcessor;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly ILogger _logger;
        private readonly object _lock = new object();

        public UploadTradeFileMonitor(
            ITradeOrderStream<TradeOrderFrame> stream,
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

        protected override void ProcessFile(string path, string archivePath)
        {
            lock (_lock)
            {
                var opCtx = _systemProcessContext.CreateAndStartOperationContext();
                var fileUpload =
                    opCtx
                        .CreateAndStartUploadFileContext(
                            SystemProcessOperationUploadFileType.TradeDataFile,
                            path);

                try
                {
                    _logger.LogInformation($"Upload Trade File beginning process file for {path}");

                    var csvReadResults = _fileProcessor.Process(path);

                    if (csvReadResults == null
                        || (!csvReadResults.SuccessfulReads.Any() && !(csvReadResults.UnsuccessfulReads.Any())))
                    {
                        fileUpload.EndEvent().EndEvent();
                        return;
                    }

                    var uploadGuid = Guid.NewGuid().ToString();

                    foreach (var item in csvReadResults.SuccessfulReads)
                    {
                        item.IsInputBatch = true;
                        item.InputBatchId = uploadGuid;
                        item.BatchSize = csvReadResults.SuccessfulReads.Count;

                        _stream.Add(item);
                    }

                    ReddeerDirectory.Move(path, archivePath);

                    if (!csvReadResults.UnsuccessfulReads.Any())
                    {
                        _logger.LogInformation($"Upload Trade File successfully processed file for {path}");
                        fileUpload.EndEvent().EndEvent();
                        return;
                    }

                    _logger.LogInformation($"Upload Trade File had errors when processing file for {path}");

                    var originatingFileName = Path.GetFileNameWithoutExtension(path);

                    _fileProcessor.WriteFailedReadsToDisk(
                        GetFailedReadsPath(),
                        originatingFileName,
                        csvReadResults.UnsuccessfulReads);

                    fileUpload.EndEvent().EndEventWithError($"Had failed reads written to disk {GetFailedReadsPath()}");
                }
                catch (Exception e)
                {
                    _logger.LogError($"Upload Trade File Monitor encountered an exception in process file for {path}", e);
                    fileUpload.EndEvent().EndEventWithError(e.Message);
                }
            }
        }
    }
}