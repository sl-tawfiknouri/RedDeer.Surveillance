using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using Microsoft.Extensions.Logging;
using Relay.Configuration.Interfaces;
using Relay.Disk_IO.Interfaces;
using Relay.Disk_IO.TradeFile.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Utilities.Disk_IO.Interfaces;

namespace Relay.Disk_IO.TradeFile
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

        public IUploadTradeFileMonitor Create(ITradeOrderStream<TradeOrderFrame> stream)
        {
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
