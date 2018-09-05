using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Utilities.Network_IO.Websocket_Hosts
{
    public class NetworkExchange : INetworkExchange
    {
        readonly IWebsocketHostFactory _websocketHostFactory;
        readonly INetworkDuplexer _networkDuplexer;
        IWebsocketHost _activeHost;
        readonly ILogger _logger;

        private volatile bool _initiated;
        private readonly object _stateTransition = new object();

        public NetworkExchange(
            IWebsocketHostFactory websocketHostFactory,
            INetworkDuplexer networkDuplexer,
            ILogger<NetworkExchange> logger)
        {
            _websocketHostFactory =
                websocketHostFactory
                ?? throw new ArgumentNullException(nameof(websocketHostFactory));

            _networkDuplexer = networkDuplexer ?? throw new ArgumentNullException(nameof(networkDuplexer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// "ws://0.0.0.0:9067" RELAY SERVICE
        /// "ws://0.0.0.0:9069" SURVEILLANCE SERVICE
        /// </summary>
        public void Initialise(string hostUrl)
        {
            if (string.IsNullOrWhiteSpace(hostUrl))
            {
                _logger.LogError("NetworkExchange received null or empty host url");
                return;
            }

            lock (_stateTransition)
            {
                if (_initiated)
                {
                    _TerminateConnections();
                }

                _initiated = true;

                var newHost = _websocketHostFactory.Build(hostUrl);
                newHost.Start((socket =>
                {
                    socket.OnOpen = () =>
                    {
                        _logger.LogInformation($"Network Exchange initiated web socket hosting at {hostUrl}");
                    };
                    socket.OnClose = () =>
                        _logger.LogInformation($"Network Exchange terminated web socket hosting at {hostUrl}");

                    socket.OnError = (e) =>
                        _logger.LogCritical($"Critical error in Network Exchange Host for {hostUrl}", e);

                    socket.OnMessage = message =>
                    {
                        var duplexMessage = JsonConvert.DeserializeObject<DuplexedMessage>(message);
                        _networkDuplexer.Transmit(duplexMessage);
                  };
                }));

                _activeHost = newHost;
            }
        }

        public void TerminateConnections()
        {
            lock (_stateTransition)
            {
                _TerminateConnections();
            }
        }

        private void _TerminateConnections()
        {
            _activeHost?.Dispose();

            _initiated = false;
        }
    }
}
