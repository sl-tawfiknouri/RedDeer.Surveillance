using System;
using System.IO;
using System.Linq;
using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.EquityFile.Interfaces;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Equity.TimeBars;
using Microsoft.Extensions.Logging;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.System.DataLayer.Processes;
using Utilities.Disk_IO.Interfaces;

namespace DataImport.Disk_IO.EquityFile
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
            ILogger<UploadEquityFileMonitor> logger)
            : base(directory, logger, "UploadEquityFileMonitor")
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _uploadConfiguration = uploadConfiguration ?? throw new ArgumentNullException(nameof(uploadConfiguration));
            _fileProcessor = fileProcessor ?? throw new ArgumentNullException(nameof(fileProcessor));
            _systemProcessContext = systemContext ?? throw new ArgumentNullException(nameof(systemContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override string UploadDirectoryPath()
        {
            return _uploadConfiguration.DataImportEquityFileUploadDirectoryPath;
        }

        public override bool ProcessFile(string path)
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
                        return false;
                    }

                    if (csvReadResults.UnsuccessfulReads.Any())
                    {
                        _logger.LogInformation($"UploadEquityFileMonitor had unsuccessful reads count of {csvReadResults.UnsuccessfulReads.Count}");
                        FailedReads(path, csvReadResults, fileUpload);
                        return false;
                    }
                    else
                    {
                        _logger.LogInformation($"UploadEquityFileMonitor had successful reads count of {csvReadResults.SuccessfulReads.Count}");
                        SuccessfulReads(path, csvReadResults, fileUpload);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"Upload Equity File Monitor encountered and swallowed an exception whilst processing {path}", e);
                    fileUpload.EndEvent().EndEventWithError(e.Message);
                    return false;
                }
            }
        }

        private void SuccessfulReads(
            string path,
            UploadFileProcessorResult<FinancialInstrumentTimeBarCsv, EquityIntraDayTimeBarCollection> csvReadResults,
            ISystemProcessOperationUploadFileContext fileUpload)
        {
            var orderedSuccessfulReads = csvReadResults.SuccessfulReads.OrderBy(sr => sr.Epoch).ToList();
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

            _logger.LogInformation($"Process File success for {path}. Had zero unsuccessful reads.");
            fileUpload.EndEvent().EndEvent();
        }

        private void FailedReads(
            string path,
            UploadFileProcessorResult<FinancialInstrumentTimeBarCsv, EquityIntraDayTimeBarCollection> csvReadResults,
            ISystemProcessOperationUploadFileContext fileUpload)
        {
            var originatingFileName = Path.GetFileNameWithoutExtension(path);

            _logger.LogError($"UploadEquityFileMonitor Process File failure for {path}. Detected {csvReadResults.UnsuccessfulReads.Count} failed reads.");

            foreach (var failedRead in csvReadResults.UnsuccessfulReads)
            {
                _logger.LogInformation($"UploadEquityFileMonitor could not parse row {failedRead.RowId} of {originatingFileName}");
            }

            _logger.LogInformation($"Upload equity file monitor had failed reads. Now deleting {path}.");
            ReddeerDirectory.DeleteFile(path);
            _logger.LogInformation($"Upload equity file monitor deleted processed files.");

            fileUpload.EndEvent().EndEvent();
        }
    }
}
