﻿using Fleck;
using Microsoft.Extensions.Logging;
using System;

namespace Relay.Network_IO
{
    public class NetworkManager : INetworkManager
    {
        private ILogger _logger;
        private IWebSocketServer _server;
        private volatile bool _initiated;
        private object _stateTransition = new object();

        public NetworkManager(ILogger<NetworkManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public void InitiateConnections()
        {
            lock (_stateTransition)
            {
                if (_initiated)
                {
                    _TerminateConnections();
                }

                _initiated = true;

                _server = new WebSocketServer("ws://0.0.0.0:9067");
                _server.Start((socket =>
                {
                    socket.OnOpen = () =>
                    {
                        _logger.LogInformation("Relay Network Manager initiated web socket connection");
                    };
                    socket.OnClose = () => 
                        _logger.LogInformation("Relay Network Manager terminated web socket connection");

                    socket.OnMessage = message => _logger.LogError($"Message received {message}");
                }));
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
            if (_server != null)
            {
                _server.Dispose();
            }

            _initiated = false;
        }
    }
}
