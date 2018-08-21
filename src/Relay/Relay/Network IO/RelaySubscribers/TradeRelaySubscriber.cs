using Domain.Equity.Trading.Orders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using System;
using Utilities.Network_IO.Websocket_Connections;
using WebSocket4Net;

namespace Relay.Network_IO.RelaySubscribers
{
    /// <summary>
    /// Receives data from its subscribed (to) trade stream and forwards it onto
    /// a listening websockets service elsewhere via its active web socket connection
    /// </summary>
    public class TradeRelaySubscriber : ITradeRelaySubscriber
    {
        private object _stateLock = new object();
        private IWebsocketConnectionFactory _websocketFactory;
        private IConnectionWebsocket _activeWebsocket;
        private ILogger _logger;
        private volatile bool _initiated;

        public TradeRelaySubscriber(
            IWebsocketConnectionFactory websocketFactory,
            ILogger<TradeRelaySubscriber> logger)
        {
            _websocketFactory = websocketFactory ?? throw new ArgumentNullException(nameof(websocketFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initiate(string domain, string port)
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
                _logger.LogInformation($"Trade Relay Subscriber initiate called");

                if (_initiated)
                {
                    _logger.LogInformation($"Trade Relay Subscriber initiate called before termination. Terminating active connection first.");

                    _Terminate();
                }

                _initiated = true;

                var connectionString = $"ws://{domain}:{port}/";
                _logger.LogInformation($"Opening web socket to {connectionString}");

                _activeWebsocket = _websocketFactory.Build(connectionString);
                _activeWebsocket.Opened += new EventHandler(Open_Event);
                _activeWebsocket.Error += new EventHandler<ErrorEventArgs>(Error_Event);
                _activeWebsocket.Closed += new EventHandler(Closed_Event);

                try
                {
                    _activeWebsocket.Open();
                    while (_activeWebsocket.State == WebSocketState.Connecting) { };
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception was encountered in Trade Relay Subscriber initiation", e);
                    throw;
                }
            }
        }

        private void Open_Event(object sender, EventArgs e)
        {
            _logger.LogInformation($"Trade Relay Subscriber Successfully Opened Connection");
        }

        private void Error_Event(object sender, ErrorEventArgs e)
        {
            _logger.LogCritical($"Trade Relay Subscriber encountered an error {e.Exception.Message}");

            lock (_stateLock)
            {
                _Terminate();
            }
        }

        private void Closed_Event(object sender, EventArgs e)
        {
            _logger.LogInformation("Trade Relay Subscriber connection closed");
        }

        public void Terminate()
        {
            lock (_stateLock)
            {
                _logger.LogInformation($"Trade Relay Subscriber Termination called");

                _Terminate();
            }
        }

        private void _Terminate()
        {
            if (_activeWebsocket != null)
            {
                _logger.LogInformation($"Trade Relay Subscriber Closing Active Web Socket");

                try
                {
                    _activeWebsocket.Close();
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception was encountered whilst terminating the trade relay subscriber web socket connections", e);
                    throw;
                }
            }
        }

        public void OnCompleted()
        {
            _logger.LogInformation($"Trade Relay Subscriber underlying stream completed.");

            Terminate();
        }

        public void OnError(Exception error)
        {
            if (error != null)
            {
                _logger.LogError("Trade Relay Subscriber was passed an error from its source stream", error);
            }
        }

        public void OnNext(TradeOrderFrame value)
        {
            lock (_stateLock)
            {
                if (_initiated
                    && _activeWebsocket != null)
                {
                    var jsonFrame = JsonConvert.SerializeObject(value);
                    _activeWebsocket.Send(jsonFrame);
                }
            }
        }
    }
}
