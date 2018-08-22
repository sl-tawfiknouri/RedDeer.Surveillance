using NLog;
using System;
using TestHarness.Factory.NetworkFactory.Interfaces;
using TestHarness.Network_IO;
using TestHarness.Network_IO.Subscribers;
using Utilities.Network_IO.Websocket_Connections;

namespace TestHarness.Factory.NetworkFactory
{
    public class NetworkManagerFactory : INetworkManagerFactory
    {
        private ILogger _logger;

        public NetworkManagerFactory(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public INetworkManager CreateWebsockets()
        {
            var websocketFactory = new WebsocketConnectionFactory();
            var configuration = new Configuration.Configuration();
            var tradeOrderSubscriberFactory = new TradeOrderWebsocketSubscriberFactory(websocketFactory, _logger);

            var networkManager = new NetworkManager(tradeOrderSubscriberFactory, configuration, _logger);
            networkManager.InitiateNetworkConnections();

            return networkManager;
        }

        public INetworkManager CreateStub()
        {
            return new StubNetworkManager(_logger);
        }
    }
}
