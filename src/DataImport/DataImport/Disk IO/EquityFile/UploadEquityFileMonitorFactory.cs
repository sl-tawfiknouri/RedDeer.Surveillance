using System;
using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.EquityFile.Interfaces;
using DomainV2.Equity.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Surveillance.System.Auditing.Context.Interfaces;
using Utilities.Disk_IO.Interfaces;

namespace DataImport.Disk_IO.EquityFile
{
    public class UploadEquityFileMonitorFactory : IUploadEquityFileMonitorFactory
    {
        private readonly IUploadConfiguration _uploadConfiguration;
        private readonly IUploadEquityFileProcessor _uploadEquityFileProcessor;
        private readonly IReddeerDirectory _reddeerDirectory;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly ILogger<UploadEquityFileMonitor> _logger;

        public UploadEquityFileMonitorFactory(
            IUploadConfiguration uploadConfiguration,
            IUploadEquityFileProcessor uploadEquityFileProcessor,
            IReddeerDirectory reddeerDirectory,
            ISystemProcessContext systemProcessContext,
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

            _systemProcessContext =
                systemProcessContext
                ?? throw new ArgumentNullException(nameof(systemProcessContext));

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
                _systemProcessContext,
                _logger);
        }
    }
}
