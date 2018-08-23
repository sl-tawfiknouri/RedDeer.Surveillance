﻿using NLog;
using System;
using TestHarness.Display;
using TestHarness.Factory.NetworkFactory.Interfaces;
using TestHarness.Network_IO;
using TestHarness.Network_IO.Subscribers;
using Utilities.Network_IO.Websocket_Connections;

namespace TestHarness.Factory.NetworkFactory
{
    public class NetworkManagerFactory : INetworkManagerFactory
    {
        private IConsole _console;
        private ILogger _logger;

        public NetworkManagerFactory(IConsole console, ILogger logger)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public INetworkManager CreateWebsockets()
        {
            var websocketFactory = new WebsocketConnectionFactory();
            var configuration = new Configuration.Configuration();
            var tradeOrderSubscriberFactory = new TradeOrderWebsocketSubscriberFactory(websocketFactory, _console, _logger);

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