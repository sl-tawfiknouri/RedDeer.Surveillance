using Microsoft.Extensions.Logging;
using System;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Surveillance.Network_IO
{
    public class SurveillanceNetworkExchangeFactory : ISurveillanceNetworkExchangeFactory
    {
        private IWebsocketHostFactory _websocketHostFactory;
        private ILogger<NetworkExchange> _logger;

        public SurveillanceNetworkExchangeFactory(
            IWebsocketHostFactory websocketHostFactory,
            ILogger<NetworkExchange> logger)
        {
            _websocketHostFactory = websocketHostFactory ?? throw new ArgumentNullException(nameof(websocketHostFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public NetworkExchange Create(INetworkDuplexer networkDuplexer)
        {
            return new NetworkExchange(
                _websocketHostFactory,
                networkDuplexer,
                _logger);
        }
    }
}
