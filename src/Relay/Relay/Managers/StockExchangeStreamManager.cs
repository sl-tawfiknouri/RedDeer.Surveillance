using Microsoft.Extensions.Logging;
using Relay.Configuration;
using Relay.Managers.Interfaces;
using Relay.Network_IO;
using Relay.Network_IO.RelaySubscribers.Interfaces;
using System;
using Domain.Equity.Frames;
using Domain.Equity.Streams;
using Domain.Equity.Streams.Interfaces;
using Domain.Streams;
using Relay.Processors;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Relay.Managers
{
    public class StockExchangeStreamManager : IStockExchangeStreamManager
    {
        private IStockExchangeStream _stockExchangeStream;
        private IEquityRelaySubscriber _equityRelaySubscriber;
        private IWebsocketHostFactory _websocketHostFactory;
        private INetworkConfiguration _networkConfiguration;

        private ILogger<EquityProcessor> _epLogger;
        private ILogger<NetworkExchange> _exchLogger;

        public StockExchangeStreamManager(
            IStockExchangeStream stockExchangeStream,
            IEquityRelaySubscriber equityRelaySubscriber,
            IWebsocketHostFactory websocketHostFactory,
            INetworkConfiguration networkConfiguration,
            ILogger<EquityProcessor> epLogger,
            ILogger<NetworkExchange> exchLogger)
        {
            _stockExchangeStream = stockExchangeStream ?? throw new ArgumentNullException(nameof(stockExchangeStream));
            _equityRelaySubscriber = equityRelaySubscriber ?? throw new ArgumentNullException(nameof(equityRelaySubscriber));
            _websocketHostFactory = websocketHostFactory ?? throw new ArgumentNullException(nameof(websocketHostFactory));
            _networkConfiguration = networkConfiguration ?? throw new ArgumentNullException(nameof(networkConfiguration));
            _epLogger = epLogger;
            _exchLogger = exchLogger;
        }

        public void Initialise()
        {
            var unsubFactory = new UnsubscriberFactory<ExchangeFrame>();
            var stockExchangeStream = new StockExchangeStream(unsubFactory); // from stock processor TO relay
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
            var exchange = new NetworkExchange(_websocketHostFactory, networkDuplexer, _exchLogger);
            exchange.Initialise(
                $"ws://{_networkConfiguration.RelayServiceEquityDomain}:{_networkConfiguration.RelayServiceEquityPort}");
        }
    }
}
