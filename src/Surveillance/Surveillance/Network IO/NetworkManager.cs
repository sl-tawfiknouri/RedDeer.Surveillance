using Domain.Streams;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using Utilities.Network_IO.Websocket_Hosts;

namespace Surveillance.Network_IO
{
    /// <summary>
    /// Manages host type network connections
    /// Where data is being received and then passed onto internal
    /// business logic
    /// </summary>
    /// <typeparam name="U">Publishing stream</typeparam>
    /// <typeparam name="V">DTO being streamed</typeparam>
    public abstract class NetworkManager<U, V>
        where U : PublishingStream<V>
        where V : class
    {
        private readonly ILogger _logger;
        private readonly string _connectionString;
        private readonly string _networkManagerName;
        private readonly IWebsocketHostFactory _websocketHostFactory;

        private readonly object _stateTransition = new object();

        private IWebsocketHost _websocketHost;
        private volatile bool _initiated;

        /// <summary>
        /// Build a network manager for hosting
        /// </summary>
        /// <param name="connectionString">Expected in format ws://0.0.0.0:9067 subsitute in your port number</param>
        public NetworkManager(
            ILogger<NetworkManager<U, V>> logger,
            IWebsocketHostFactory websocketHostFactory,
            string connectionString,
            string networkManagerName)
        {
            _logger =
                logger
                ?? throw new ArgumentNullException(nameof(logger));

            _websocketHostFactory =
                websocketHostFactory
                ?? throw new ArgumentNullException(nameof(websocketHostFactory));

            _connectionString =
                connectionString
                ?? string.Empty;

            _networkManagerName =
                networkManagerName
                ?? string.Empty;
        }

        public void InitiateConnections(U stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            lock (_stateTransition)
            {
                if (_initiated)
                {
                    _Terminate();
                }

                _initiated = true;
                _websocketHost = _websocketHostFactory.Build(_connectionString);

                _websocketHost.Start((socket =>
                {
                    socket.OnOpen = () =>
                    {
                        _logger.LogInformation($"{_networkManagerName} initiated web socket connection");
                    };
                    socket.OnClose = () =>
                        _logger.LogInformation($"{_networkManagerName} terminated web socket connection");

                    socket.OnMessage = message =>
                    {
                        var dataReceieved = JsonConvert.DeserializeObject<V>(message);
                        stream.Add(dataReceieved);
                    };
                }));

            }
        }

        public void TerminateConnections()
        {
            lock (_stateTransition)
            {
                _Terminate();
            }
        }

        protected void _Terminate()
        {
            if (_websocketHost != null)
            {
                _websocketHost.Dispose();
            }

            _initiated = false;
        }
    }
}
