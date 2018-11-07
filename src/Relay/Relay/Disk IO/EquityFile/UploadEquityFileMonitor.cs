using System;
using System.IO;
using System.Linq;
using Domain.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Relay.Configuration.Interfaces;
using Relay.Disk_IO.EquityFile.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes;
using Utilities.Disk_IO.Interfaces;

namespace Relay.Disk_IO.EquityFile
{
    public class UploadEquityFileMonitor : BaseUploadFileMonitor, IUploadEquityFileMonitor
    {
        private readonly IStockExchangeStream _stream;
        private readonly IUploadConfiguration _uploadConfiguration;
        private readonly IUploadEquityFileProcessor _fileProcessor;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly ILogger _logger;
        private readonly object _lock = new object();

        public UploadEquityFileMonitor(
            IStockExchangeStream stream,
            IUploadConfiguration uploadConfiguration,
            IUploadEquityFileProcessor fileProcessor,
            IReddeerDirectory directory,
            ISystemProcessContext systemContext,
            ILogger logger,
            string uploadFileMonitorName) 
            : base(directory, logger, uploadFileMonitorName)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _uploadConfiguration = uploadConfiguration ?? throw new ArgumentNullException(nameof(uploadConfiguration));
            _fileProcessor = fileProcessor ?? throw new ArgumentNullException(nameof(fileProcessor));
            _systemProcessContext = systemContext ?? throw new ArgumentNullException(nameof(systemContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override string UploadDirectoryPath()
        {
            return _uploadConfiguration.RelayEquityFileUploadDirectoryPath;
        }

        protected override void ProcessFile(string path)
        {
            lock (_lock)
            {
                _logger.LogInformation($"Process File Initiating in Upload Equity File Monitor for {path}");

                var opCtx = _systemProcessContext.CreateAndStartOperationContext();
                var fileUpload =
                    opCtx
                        .CreateAndStartUploadFileContext(
                            SystemProcessOperationUploadFileType.MarketDataFile,
                            path);
                try
                {
                    var csvReadResults = _fileProcessor.Process(path);

                    if (csvReadResults == null
                        || (!csvReadResults.SuccessfulReads.Any() && !(csvReadResults.UnsuccessfulReads.Any())))
                    {
                        _logger.LogInformation($"Process File did not find any records for {path}");
                        fileUpload.EndEvent().EndEvent();
                        return;
                    }

                    var orderedSuccessfulReads = csvReadResults.SuccessfulReads.OrderBy(sr => sr.TimeStamp).ToList();

                    if (orderedSuccessfulReads.Any())
                    {
                        _logger.LogInformation($"Upload equity file monitor had successful reads, beginning to add to stream ({orderedSuccessfulReads.Count})");
                    }

                    foreach (var item in orderedSuccessfulReads)
                    {
                        _stream.Add(item);
                    }

                    _logger.LogInformation($"Upload equity file monitor uploaded {orderedSuccessfulReads.Count} records. Now deleting {path}.");
                    ReddeerDirectory.DeleteFile(path);
                    _logger.LogInformation($"Upload equity file monitor deleted processed files. Now checking for unsuccessful reads ({csvReadResults.UnsuccessfulReads.Count})");

                    if (!csvReadResults.UnsuccessfulReads.Any())
                    {
                        _logger.LogInformation($"Process File success for {path}. Had zero unsuccessful reads.");
                        fileUpload.EndEvent().EndEvent();
                        return;
                    }

                    _logger.LogInformation($"Process File failure for {path}. Detected {csvReadResults.UnsuccessfulReads.Count} failed reads.");
                    var originatingFileName = Path.GetFileNameWithoutExtension(path);

                    _fileProcessor.WriteFailedReadsToDisk(
                        GetFailedReadsPath(),
                        originatingFileName,
                        csvReadResults.UnsuccessfulReads);

                    _logger.LogInformation($"Process File completed with failed reads written to {GetFailedReadsPath()} for {path}");
                    fileUpload.EventException($"Had failed reads written to disk {GetFailedReadsPath()}");
                    fileUpload.EndEvent().EndEvent();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Upload Equity File Monitor encountered and swallowed an exception whilst processing {path}", e);
                    fileUpload.EventException(e);
                    fileUpload.EndEvent().EndEvent();
                }
            }
        }
    }
}
