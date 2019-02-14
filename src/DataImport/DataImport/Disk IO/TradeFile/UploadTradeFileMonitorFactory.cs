using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.Interfaces;
using DataImport.Disk_IO.TradeFile.Interfaces;
using DomainV2.Streams;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Surveillance.Auditing.Context.Interfaces;
using Utilities.Disk_IO.Interfaces;

namespace DataImport.Disk_IO.TradeFile
{
    public class UploadTradeFileMonitorFactory : IUploadTradeFileMonitorFactory
    {
        private readonly IUploadConfiguration _uploadConfiguration;
        private readonly IReddeerDirectory _reddeerDirectory;
        private readonly IUploadTradeFileProcessor _fileProcessor;
        private readonly ISystemProcessContext _systemProcessContext;
        private readonly ILogger<UploadTradeFileMonitor> _logger;

        public UploadTradeFileMonitorFactory(
            IUploadConfiguration uploadConfiguration,
            IReddeerDirectory reddeerDirectory,
            IUploadTradeFileProcessor fileProcessor,
            ISystemProcessContext systemProcessContext,
            ILogger<UploadTradeFileMonitor> logger)
        {
            _uploadConfiguration = uploadConfiguration;
            _reddeerDirectory = reddeerDirectory;
            _fileProcessor = fileProcessor;
            _systemProcessContext = systemProcessContext;
            _logger = logger;
        }

        public IUploadTradeFileMonitor Create(OrderStream<Order> stream)
        {
            _logger.LogInformation($"UploadTradeFileMonitorFactory creating a new trade file monitor");
            return new UploadTradeFileMonitor(
                stream,
                _uploadConfiguration,
                _reddeerDirectory,
                _fileProcessor,
                _systemProcessContext,
                _logger);
        }
    }
}
