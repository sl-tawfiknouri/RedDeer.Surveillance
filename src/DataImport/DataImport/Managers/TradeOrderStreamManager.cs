using System;
using DataImport.Disk_IO.Interfaces;
using DataImport.Managers.Interfaces;
using DataImport.Recorders.Interfaces;
using DomainV2.Streams;
using DomainV2.Trading;

namespace DataImport.Managers
{
    public class TradeOrderStreamManager : ITradeOrderStreamManager
    {
        private readonly OrderStream<Order> _tradeOrderStream;
        private readonly IUploadTradeFileMonitorFactory _fileMonitorFactory;
        private readonly IRedDeerAuroraTradeRecorderAutoSchedule _tradeRecorder;

        public TradeOrderStreamManager(
            OrderStream<Order> tradeOrderStream,
            IUploadTradeFileMonitorFactory fileMonitorFactory,
            IRedDeerAuroraTradeRecorderAutoSchedule tradeRecorder)
        {
            _tradeOrderStream = tradeOrderStream ?? throw new ArgumentNullException(nameof(tradeOrderStream));
            _fileMonitorFactory = fileMonitorFactory ?? throw new ArgumentNullException(nameof(fileMonitorFactory));
            _tradeRecorder = tradeRecorder ?? throw new ArgumentNullException(nameof(tradeRecorder));
        }

        public IUploadTradeFileMonitor Initialise()
        {
            // hook up the data recorder
            _tradeOrderStream.Subscribe(_tradeRecorder);

            var fileMonitor = _fileMonitorFactory.Create(_tradeOrderStream);
            fileMonitor.Initiate();

            return fileMonitor;
        }
    }
}