using Microsoft.Extensions.Logging;
using Relay.Managers.Interfaces;
using Relay.Network_IO;
using Relay.Network_IO.RelaySubscribers.Interfaces;
using System;
using Domain.Equity.Frames;
using Domain.Equity.Streams;
using Domain.Equity.Streams.Interfaces;
using Domain.Streams;
using Relay.Configuration.Interfaces;
using Relay.Disk_IO.EquityFile.Interfaces;
using Relay.Processors;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Relay.Managers
{
    public class StockExchangeStreamManager : IStockExchangeStreamManager
    {
        private readonly IStockExchangeStream _stockExchangeStream;
        private readonly IEquityRelaySubscriber _equityRelaySubscriber;
        private readonly IWebsocketHostFactory _websocketHostFactory;
        private readonly INetworkConfiguration _networkConfiguration;
        private readonly IUploadEquityFileMonitorFactory _equityFileMonitorFactory;

        private readonly ILogger<EquityProcessor> _epLogger;
        private readonly ILogger<NetworkExchange> _exchangeLogger;

        public StockExchangeStreamManager(
            IStockExchangeStream stockExchangeStream,
            IEquityRelaySubscriber equityRelaySubscriber,
            IWebsocketHostFactory websocketHostFactory,
            INetworkConfiguration networkConfiguration,
            IUploadEquityFileMonitorFactory equityFileMonitorFactory,
            ILogger<EquityProcessor> epLogger,
            ILogger<NetworkExchange> exchangeLogger)
        {
            _stockExchangeStream = stockExchangeStream ?? throw new ArgumentNullException(nameof(stockExchangeStream));
            _equityRelaySubscriber = equityRelaySubscriber ?? throw new ArgumentNullException(nameof(equityRelaySubscriber));
            _websocketHostFactory = websocketHostFactory ?? throw new ArgumentNullException(nameof(websocketHostFactory));
            _networkConfiguration = networkConfiguration ?? throw new ArgumentNullException(nameof(networkConfiguration));
            _epLogger = epLogger ?? throw new ArgumentNullException(nameof(epLogger));
            _exchangeLogger = exchangeLogger ?? throw new ArgumentNullException(nameof(exchangeLogger));
            _equityFileMonitorFactory =
                equityFileMonitorFactory
                ?? throw new ArgumentNullException(nameof(equityFileMonitorFactory));
        }

        public void Initialise()
        {
            var unsubscriberFactory = new UnsubscriberFactory<ExchangeFrame>();
            var stockExchangeStream = new StockExchangeStream(unsubscriberFactory); // from stock processor TO relay
            var equityProcessor = new EquityProcessor(_epLogger, stockExchangeStream);
            stockExchangeStream.Subscribe(_equityRelaySubscriber);

            //Initiate communication with downstream process (surv service)
            _equityRelaySubscriber.Initiate(
                _networkConfiguration.SurveillanceServiceEquityDomain,
                _networkConfiguration.SurveillanceServiceEquityPort);

            // hook the equity processor to receive the incoming network stream
            _stockExchangeStream.Subscribe(equityProcessor);

            // begin hosting connection for upstream processes (i.e. test harness etc)
            var networkDuplexer = new RelayEquityNetworkDuplexer(_stockExchangeStream);
            var exchange = new NetworkExchange(_websocketHostFactory, networkDuplexer, _exchangeLogger);
            exchange.Initialise(
                $"ws://{_networkConfiguration.RelayServiceEquityDomain}:{_networkConfiguration.RelayServiceEquityPort}");

            // set up trading file monitor and wire it into the stream
            var fileMonitor = _equityFileMonitorFactory.Build(stockExchangeStream);
            fileMonitor.Initiate();
        }
    }
}
