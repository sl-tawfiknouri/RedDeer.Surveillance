using System;
using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.EquityFile.Interfaces;
using DataImport.Managers.Interfaces;
using DataImport.Network_IO;
using DataImport.Recorders.Interfaces;
using DomainV2.Equity.Frames;
using DomainV2.Equity.Streams;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Streams;
using Microsoft.Extensions.Logging;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace DataImport.Managers
{
    public class StockExchangeStreamManager : IStockExchangeStreamManager
    {
        private readonly IWebsocketHostFactory _websocketHostFactory;
        private readonly INetworkConfiguration _networkConfiguration;
        private readonly IUploadEquityFileMonitorFactory _equityFileMonitorFactory;
        private readonly IRedDeerAuroraStockExchangeRecorder _stockExchangeRecorder;

        private readonly ILogger<NetworkExchange> _exchangeLogger;

        public StockExchangeStreamManager(
            IWebsocketHostFactory websocketHostFactory,
            INetworkConfiguration networkConfiguration,
            IUploadEquityFileMonitorFactory equityFileMonitorFactory,
            IRedDeerAuroraStockExchangeRecorder stockExchangeRecorder,
            ILogger<NetworkExchange> exchangeLogger)
        {
            _websocketHostFactory = websocketHostFactory ?? throw new ArgumentNullException(nameof(websocketHostFactory));
            _networkConfiguration = networkConfiguration ?? throw new ArgumentNullException(nameof(networkConfiguration));
            _exchangeLogger = exchangeLogger ?? throw new ArgumentNullException(nameof(exchangeLogger));
            _stockExchangeRecorder = stockExchangeRecorder ?? throw new ArgumentNullException(nameof(stockExchangeRecorder));
            _equityFileMonitorFactory =
                equityFileMonitorFactory
                ?? throw new ArgumentNullException(nameof(equityFileMonitorFactory));
        }

        public IUploadEquityFileMonitor Initialise()
        {
            var unsubscriberFactory = new UnsubscriberFactory<ExchangeFrame>();
            var stockExchangeStream = new StockExchangeStream(unsubscriberFactory); // from stock processor TO relay

            //Initiate communication with downstream process (surveillance service)
            //stockExchangeStream.Subscribe(_equityRelaySubscriber);

            // hook up the data recorder
            stockExchangeStream.Subscribe(_stockExchangeRecorder);

            //Initiate communication with downstream process (surveillance service)
            //_equityRelaySubscriber.Initiate(
            //    _networkConfiguration.SurveillanceServiceEquityDomain,
            //    _networkConfiguration.SurveillanceServiceEquityPort);

            // begin hosting connection for upstream processes (i.e. test harness etc)
            HostOverWebsockets(stockExchangeStream);

            // set up trading file monitor and wire it into the stream
            var fileMonitor = _equityFileMonitorFactory.Build(stockExchangeStream);
            fileMonitor.Initiate();

            return fileMonitor;
        }

        private void HostOverWebsockets(IStockExchangeStream stockExchangeStream)
        {
            var networkDuplexer = new RelayEquityNetworkDuplexer(stockExchangeStream);
            var exchange = new NetworkExchange(_websocketHostFactory, networkDuplexer, _exchangeLogger);
            exchange.Initialise(
                $"ws://{_networkConfiguration.RelayServiceEquityDomain}:{_networkConfiguration.RelayServiceEquityPort}");
        }
    }
}
