using SuperSocket.ClientEngine;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using WebSocket4Net;
using System;
using System.Threading;
using Newtonsoft.Json;

namespace Utilities.Network_IO.Websocket_Connections
{
    public class NetworkTrunk : INetworkTrunk
    {
        private object _stateLock = new object();
        private IWebsocketConnectionFactory _websocketFactory;
        private IConnectionWebsocket _activeWebsocket;
        private volatile bool _initiated;

        public NetworkTrunk(IWebsocketConnectionFactory websocketFactory)
        {
            _websocketFactory = websocketFactory ?? throw new ArgumentNullException(nameof(websocketFactory));
        }

        /// <summary>
        /// Returns success result
        /// </summary>
        public bool Initiate(string domain, string port, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                throw new ArgumentNullException(nameof(domain));
            }

            if (string.IsNullOrWhiteSpace(port))
            {
                throw new ArgumentNullException(nameof(port));
            }

            lock (_stateLock)
            {
                if (_initiated)
                {
                    _Terminate();
                }

                _initiated = true;

                var connectionString = $"ws://{domain}:{port}/";
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
            // add logging
        }

        private void Error_Event(object sender, ErrorEventArgs e)
        {
            lock (_stateLock)
            {
                // add logging

                _Terminate();
            }
        }

        private void Closed_Event(object sender, EventArgs e)
        {
            // add logging
        }

        private bool ConnectToActiveSocket(CancellationToken token)
        {
            try
            {
                _activeWebsocket.Open();

                while (_activeWebsocket.State == WebSocketState.Connecting)
                {
                    if (token != null && token.IsCancellationRequested)
                    {
                        break;
                    }
                };

                if (token != null && token.IsCancellationRequested)
                {
                    _activeWebsocket.Close();
                }

                return token == null || !token.IsCancellationRequested;
            }
            catch
            {
                return false;
            }
        }

        public void Terminate()
        {
            lock (_stateLock)
            {
                _Terminate();
            }
        }

        private void _Terminate()
        {
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
                catch
                {
                    // add logging
                    throw;
                }
            }
        }

        public void Send<T>(T value)
        {
            if (value == null)
            {
                return;
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
                }
            }
        }
    }
}