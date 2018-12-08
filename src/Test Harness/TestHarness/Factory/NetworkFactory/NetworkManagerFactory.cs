using System;
using Microsoft.Extensions.Logging;
using TestHarness.Configuration.Interfaces;
using TestHarness.Display.Interfaces;
using TestHarness.Factory.NetworkFactory.Interfaces;
using TestHarness.Network_IO;
using TestHarness.Network_IO.Interfaces;
using TestHarness.Network_IO.Subscribers;
using Utilities.Network_IO.Websocket_Connections;

namespace TestHarness.Factory.NetworkFactory
{
    public class NetworkManagerFactory : INetworkManagerFactory
    {
        private readonly IConsole _console;
        private readonly ILogger _logger;
        private readonly INetworkConfiguration _networkConfiguration;

        public NetworkManagerFactory(
            IConsole console,
            ILogger logger,
            INetworkConfiguration networkConfiguration)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _networkConfiguration = networkConfiguration ?? throw new ArgumentNullException(nameof(networkConfiguration));
        }

        public INetworkManager CreateWebsockets()
        {
            var websocketFactory = new WebsocketConnectionFactory();
            var tradeOrderSubscriberFactory = new TradeOrderWebsocketSubscriberFactory(websocketFactory, _console, _logger);
            var stockMarketSubscriberFactory = new StockMarketWebsocketSubscriberFactory(websocketFactory, _console, _logger);

            var networkManager = new NetworkManager(
                tradeOrderSubscriberFactory,
                stockMarketSubscriberFactory,
                _networkConfiguration,
                _logger);

            networkManager.InitiateAllNetworkConnections();

            return networkManager;
        }

        public INetworkManager CreateStub()
        {
            return new StubNetworkManager(_logger);
        }
    }
}
