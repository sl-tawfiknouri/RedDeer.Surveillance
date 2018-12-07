﻿using System;
using Microsoft.Extensions.Logging;
using Surveillance.NetworkIO.Interfaces;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Surveillance.NetworkIO
{
    public class SurveillanceNetworkExchangeFactory : ISurveillanceNetworkExchangeFactory
    {
        private readonly IWebsocketHostFactory _websocketHostFactory;
        private readonly ILogger<NetworkExchange> _logger;

        public SurveillanceNetworkExchangeFactory(
            IWebsocketHostFactory websocketHostFactory,
            ILogger<NetworkExchange> logger)
        {
            _websocketHostFactory = websocketHostFactory ?? throw new ArgumentNullException(nameof(websocketHostFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public NetworkExchange Create(ISurveillanceNetworkDuplexer networkDuplexer)
        {
            return new NetworkExchange(
                _websocketHostFactory,
                networkDuplexer,
                _logger);
        }
    }
}