using System;
using System.IO;
using System.Linq;
using Domain.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Relay.Configuration.Interfaces;
using Relay.Disk_IO.EquityFile.Interfaces;
using Utilities.Disk_IO.Interfaces;

namespace Relay.Disk_IO.EquityFile
{
    public class UploadEquityFileMonitor : BaseUploadFileMonitor, IUploadEquityFileMonitor
    {
        private readonly IStockExchangeStream _stream;
        private readonly IUploadConfiguration _uploadConfiguration;
        private readonly IUploadEquityFileProcessor _fileProcessor;
        private readonly object _lock = new object();

        public UploadEquityFileMonitor(
            IStockExchangeStream stream,
            IUploadConfiguration uploadConfiguration,
            IUploadEquityFileProcessor fileProcessor,
            IReddeerDirectory directory,
            ILogger logger,
            string uploadFileMonitorName) 
            : base(directory, logger, uploadFileMonitorName)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _uploadConfiguration = uploadConfiguration ?? throw new ArgumentNullException(nameof(uploadConfiguration));
            _fileProcessor = fileProcessor ?? throw new ArgumentNullException(nameof(fileProcessor));
        }

        protected override string UploadDirectoryPath()
        {
            return _uploadConfiguration.RelayEquityFileUploadDirectoryPath;
        }

        protected override void ProcessFile(string path, string archivePath)
        {
            lock (_lock)
            {
                var csvReadResults = _fileProcessor.Process(path);

                if (csvReadResults == null
                    || (!csvReadResults.SuccessfulReads.Any() && !(csvReadResults.UnsuccessfulReads.Any())))
                {
                    return;
                }

                foreach (var item in csvReadResults.SuccessfulReads)
                {
                    _stream.Add(item);
                }

                ReddeerDirectory.Move(path, archivePath);

                if (!csvReadResults.UnsuccessfulReads.Any())
                {
                    return;
                }

                var originatingFileName = Path.GetFileNameWithoutExtension(path);

                _fileProcessor.WriteFailedReadsToDisk(
                    GetFailedReadsPath(),
                    originatingFileName,
                    csvReadResults.UnsuccessfulReads);
            }
        }
    }
}
