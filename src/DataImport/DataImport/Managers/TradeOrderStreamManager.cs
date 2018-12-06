using System;
using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.Interfaces;
using DataImport.Managers.Interfaces;
using DataImport.Network_IO;
using DataImport.Network_IO.RelaySubscribers.Interfaces;
using DataImport.Processors;
using DataImport.Recorders.Interfaces;
using DomainV2.Streams;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace DataImport.Managers
{
    public class TradeOrderStreamManager : ITradeOrderStreamManager
    {
        private readonly OrderStream<Order> _tradeOrderStream;
        private readonly ITradeRelaySubscriber _tradeRelaySubscriber;
        private readonly IWebsocketHostFactory _websocketHostFactory;
        private readonly INetworkConfiguration _networkConfiguration;
        private readonly IUploadTradeFileMonitorFactory _fileMonitorFactory;
        private readonly IRedDeerAuroraTradeRecorderAutoSchedule _tradeRecorder;

        private readonly ILogger<TradeProcessor<Order>> _tpLogger;
        private readonly ILogger<NetworkExchange> _exchangeLogger;

        public TradeOrderStreamManager(
            OrderStream<Order> tradeOrderStream,
            ITradeRelaySubscriber tradeRelaySubscriber,
            IWebsocketHostFactory websocketHostFactory,
            INetworkConfiguration networkConfiguration,
            IUploadTradeFileMonitorFactory fileMonitorFactory,
            IRedDeerAuroraTradeRecorderAutoSchedule tradeRecorder,
            ILogger<TradeProcessor<Order>> tpLogger,
            ILogger<NetworkExchange> exchangeLogger)
        {
            _tradeOrderStream = tradeOrderStream ?? throw new ArgumentNullException(nameof(tradeOrderStream));
            _tradeRelaySubscriber = tradeRelaySubscriber ?? throw new ArgumentNullException(nameof(tradeRelaySubscriber));
            _websocketHostFactory = websocketHostFactory ?? throw new ArgumentNullException(nameof(websocketHostFactory));

            _networkConfiguration =
                networkConfiguration 
                ?? throw new ArgumentNullException(nameof(networkConfiguration));

            _fileMonitorFactory = fileMonitorFactory ?? throw new ArgumentNullException(nameof(fileMonitorFactory));
            _tradeRecorder = tradeRecorder ?? throw new ArgumentNullException(nameof(tradeRecorder));

            _tpLogger = tpLogger ?? throw new ArgumentNullException(nameof(tpLogger));
            _exchangeLogger = exchangeLogger ?? throw new ArgumentNullException(nameof(exchangeLogger));
        }

        public IUploadTradeFileMonitor Initialise()
        {
            var unsubscriberFactory = new UnsubscriberFactory<Order>();
            var tradeProcessorOrderStream = new OrderStream<Order>(unsubscriberFactory);
            var tradeProcessor = new TradeProcessor<Order>(_tpLogger, tradeProcessorOrderStream);

            // hook the relay subscriber to begin communications with the outgoing network stream
            // //tradeProcessorOrderStream.Subscribe(_tradeRelaySubscriber);

            // hook the trade processor to receive the incoming network stream
            _tradeOrderStream.Subscribe(tradeProcessor);

            // hook up the data recorder
            _tradeOrderStream.Subscribe(_tradeRecorder);

            // hook the relay subscriber to begin communications with the outgoing network stream           
            //_tradeRelaySubscriber.Initiate(_networkConfiguration.SurveillanceServiceTradeDomain,_networkConfiguration.SurveillanceServiceTradePort);

            HostOverWebsockets();

            var fileMonitor = _fileMonitorFactory.Create(tradeProcessorOrderStream);
            fileMonitor.Initiate();

            return fileMonitor;
        }

        /// <summary>
        /// Save this function for when we do b pipe and want client vs cloud mode
        /// </summary>
        private void HostOverWebsockets()
        {
            // begin hosting connection for downstream processes (i.e. surveillance service)
            var networkDuplexer = new RelayTradeNetworkDuplexer(_tradeOrderStream);
            var exchange = new NetworkExchange(_websocketHostFactory, networkDuplexer, _exchangeLogger);

            exchange.Initialise($"ws://{_networkConfiguration.RelayServiceTradeDomain}:{_networkConfiguration.RelayServiceTradePort}");
        }
    }
}