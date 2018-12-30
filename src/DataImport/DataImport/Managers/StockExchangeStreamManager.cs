using System;
using DataImport.Disk_IO.EquityFile.Interfaces;
using DataImport.Managers.Interfaces;
using DataImport.Recorders.Interfaces;
using DomainV2.Equity.Frames;
using DomainV2.Equity.Streams;
using DomainV2.Streams;

namespace DataImport.Managers
{
    public class StockExchangeStreamManager : IStockExchangeStreamManager
    {
        private readonly IUploadEquityFileMonitorFactory _equityFileMonitorFactory;
        private readonly IRedDeerAuroraStockExchangeRecorder _stockExchangeRecorder;
        
        public StockExchangeStreamManager(
            IUploadEquityFileMonitorFactory equityFileMonitorFactory,
            IRedDeerAuroraStockExchangeRecorder stockExchangeRecorder)
        {
            _stockExchangeRecorder = stockExchangeRecorder ?? throw new ArgumentNullException(nameof(stockExchangeRecorder));
            _equityFileMonitorFactory =
                equityFileMonitorFactory
                ?? throw new ArgumentNullException(nameof(equityFileMonitorFactory));
        }

        public IUploadEquityFileMonitor Initialise()
        {
            var unsubscriberFactory = new UnsubscriberFactory<ExchangeFrame>();
            var stockExchangeStream = new StockExchangeStream(unsubscriberFactory); // from stock processor TO relay

            // hook up the data recorder
            stockExchangeStream.Subscribe(_stockExchangeRecorder);

            // set up trading file monitor and wire it into the stream
            var fileMonitor = _equityFileMonitorFactory.Build(stockExchangeStream);
            fileMonitor.Initiate();

            return fileMonitor;
        }
    }
}
