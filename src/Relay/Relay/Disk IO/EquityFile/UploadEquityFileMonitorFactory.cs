using System;
using Domain.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Relay.Configuration.Interfaces;
using Relay.Disk_IO.EquityFile.Interfaces;
using Utilities.Disk_IO.Interfaces;

namespace Relay.Disk_IO.EquityFile
{
    public class UploadEquityFileMonitorFactory : IUploadEquityFileMonitorFactory
    {
        private readonly IUploadConfiguration _uploadConfiguration;
        private readonly IUploadEquityFileProcessor _uploadEquityFileProcessor;
        private readonly IReddeerDirectory _reddeerDirectory;
        private readonly ILogger<UploadEquityFileMonitor> _logger;

        public UploadEquityFileMonitorFactory(
            IUploadConfiguration uploadConfiguration,
            IUploadEquityFileProcessor uploadEquityFileProcessor,
            IReddeerDirectory reddeerDirectory,
            ILogger<UploadEquityFileMonitor> logger)
        {
            _uploadConfiguration =
                uploadConfiguration
                ?? throw new ArgumentNullException(nameof(uploadConfiguration));

            _uploadEquityFileProcessor =
                uploadEquityFileProcessor
                ?? throw new ArgumentNullException(nameof(uploadEquityFileProcessor));

            _reddeerDirectory =
                reddeerDirectory
                ?? throw new ArgumentNullException(nameof(reddeerDirectory));

            _logger =
                logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUploadEquityFileMonitor Build(IStockExchangeStream exchangeStream)
        {
            return new UploadEquityFileMonitor(
                exchangeStream,
                _uploadConfiguration,
                _uploadEquityFileProcessor,
                _reddeerDirectory,
                _logger,
                "Upload Equity File Monitor");
        }
    }
}
