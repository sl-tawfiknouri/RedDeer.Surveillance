using SuperSocket.ClientEngine;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using WebSocket4Net;
using System;
using System.Threading;
using Newtonsoft.Json;
using Utilities.Network_IO.Interfaces;

namespace Utilities.Network_IO.Websocket_Connections
{
    public class NetworkTrunk : INetworkTrunk
    {
        private readonly object _stateLock = new object();

        private readonly IWebsocketConnectionFactory _websocketFactory;
        private IConnectionWebsocket _activeWebsocket;
        private readonly IMessageWriter _messageWriter;
        private volatile bool _initiated;
        
        public NetworkTrunk(
            IWebsocketConnectionFactory websocketFactory,
            IMessageWriter messageWriter)
        {
            _websocketFactory = websocketFactory ?? throw new ArgumentNullException(nameof(websocketFactory));
            _messageWriter = messageWriter ?? throw new ArgumentNullException(nameof(messageWriter));
        }

        public bool Active { get; private set; }

        /// <summary>
        /// Returns success result
        /// </summary>
        public bool Initiate(string domain, string port, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                _messageWriter.Write("Network trunk passed null domain");
                throw new ArgumentNullException(nameof(domain));
            }

            if (string.IsNullOrWhiteSpace(port))
            {
                _messageWriter.Write("Network trunk passed null port");
                throw new ArgumentNullException(nameof(port));
            }

            lock (_stateLock)
            {
                if (_initiated)
                {
                    _messageWriter.Write("Network trunk initiating with already initiated instance. Terminating old process first.");
                    _Terminate(false);
                }

                _initiated = true;

                var connectionString = $"ws://{domain}:{port}/";

                _messageWriter.Write($"Network trunk initiating against connection {connectionString}");
                BuildActiveSocket(connectionString);

                return ConnectToActiveSocket(token);
            }
        }

        private void BuildActiveSocket(string connectionString)
        {
            _activeWebsocket = _websocketFactory.Build(connectionString);

            _activeWebsocket.Opened += new EventHandler(Open_Event);
            _activeWebsocket.Error += new EventHandler<ErrorEventArgs>(Error_Event);
            _activeWebsocket.Closed += new EventHandler(Closed_Event);
        }

        private void Open_Event(object sender, EventArgs e)
        {
        }

        private void Closed_Event(object sender, EventArgs e)
        {
        }

        private void Error_Event(object sender, ErrorEventArgs e)
        {
            lock (_stateLock)
            {
                _Terminate(false);
            }
        }

        private bool ConnectToActiveSocket(CancellationToken token)
        {
            try
            {
                _activeWebsocket.Open();

                _messageWriter.Write($"Network trunk connecting to web socket...");
                while (_activeWebsocket.State == WebSocketState.Connecting)
                {
                    if (token != null && token.IsCancellationRequested)
                    {
                        break;
                    }
                };

                if (token != null && token.IsCancellationRequested)
                {
                    _messageWriter.Write($"Network trunk connection attempt timed out. Closing web socket...");

                    _activeWebsocket.Close();
                }

                var connectionEstablished = token == null || !token.IsCancellationRequested;

                Active = connectionEstablished;

                if (connectionEstablished)
                {
                    _messageWriter.Write($"Network trunk connection established");
                }
                else
                {
                    _messageWriter.Write($"Network trunk connection failed to establish");
                }

                return Active;
            }
            catch
            {
                _messageWriter.Write("Network trunk encountered an exception on connection. Closing socket.");
                return false;
            }
        }

        public void Terminate()
        {
            lock (_stateLock)
            {
                _messageWriter.Write("Network trunk terminating web socket");
                _Terminate(true);
            }
        }

        private void _Terminate(bool userTriggered)
        {
            if (userTriggered)
            {
                _initiated = false;
            }

            Active = false;

            if (_activeWebsocket != null)
            {
                try
                {
                    if (_activeWebsocket.State != WebSocketState.Closed
                        && _activeWebsocket.State != WebSocketState.Closing
                        && _activeWebsocket.State != WebSocketState.None)
                    {
                        _activeWebsocket.Close();

                        while (_activeWebsocket.State != WebSocketState.Closed)
                        { }
                    }
                }
                catch (Exception e)
                {
                    _messageWriter.Write($"Network trunk encountered error terminating web socket {e.Message}");
                    throw;
                }
            }
        }

        /// <summary>
        /// If using a duplexed connection
        /// make sure that the value is already duplexed
        /// </summary>
        public bool Send<T>(T value)
        {
            if (value == null)
            {
                return false;
            }

            lock (_stateLock)
            {
                if (_initiated
                    && _activeWebsocket != null
                    && _activeWebsocket.State == WebSocketState.Open
                    && _activeWebsocket.State != WebSocketState.Connecting
                    && _activeWebsocket.State != WebSocketState.Closing)
                {
                    var jsonFrame = JsonConvert.SerializeObject(value);
                    _activeWebsocket.Send(jsonFrame);
                    return true;
                }
                else
                {
                    Active = false;
                    return false;
                }
            }
        }
    }
}