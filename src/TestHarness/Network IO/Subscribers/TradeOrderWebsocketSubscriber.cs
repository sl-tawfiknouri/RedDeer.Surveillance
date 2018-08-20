using System;
using Domain.Equity.Trading.Orders;
using Newtonsoft.Json;
using NLog;
using SuperSocket.ClientEngine;

namespace TestHarness.Network_IO.Subscribers
{
    public class TradeOrderWebsocketSubscriber : ITradeOrderWebsocketSubscriber
    {
        private object _stateLock = new object();
        private IWebsocketFactory _websocketFactory;
        private IWebsocket _activeWebsocket;
        private ILogger _logger;
        private volatile bool _initiated;

        public TradeOrderWebsocketSubscriber(
            IWebsocketFactory websocketFactory,
            ILogger logger)
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
                _logger.Log(LogLevel.Info, $"Trade Order Websocket Subscriber initiate called");

                if (_initiated)
                {
                    _logger.Log(LogLevel.Warn, $"Trade Order Websocket Subscriber initiate called before termination. Terminating active connection first.");

                    _Terminate();
                }
                
                var connectionString = $"wss://{domain}:{port}/";
                _logger.Log(LogLevel.Info, $"Opening web socket to {connectionString}");

                _activeWebsocket = _websocketFactory.Build(connectionString);
                _activeWebsocket.Opened += new EventHandler(Open_Event);
                _activeWebsocket.Error += new EventHandler<ErrorEventArgs>(Error_Event);
                _activeWebsocket.Closed += new EventHandler(Closed_Event);

                try
                {
                    _activeWebsocket.Open();
                }
                catch(Exception e)
                {
                    _logger.Log(LogLevel.Error, e);
                }
            }
        }

        private void Open_Event(object sender, EventArgs e)
        {
            _logger.Log(LogLevel.Info, $"Trade Order Websocket Subscriber Successfully Opened Connection");
        }

        private void Error_Event(object sender, ErrorEventArgs e)
        {
            _logger.Log(LogLevel.Error, "Trade Order Websocket Subscriber encountered an error");

            lock (_stateLock)
            {
                _Terminate();
            }
        }

        private void Closed_Event(object sender, EventArgs e)
        {
            _logger.Log(LogLevel.Info, "Trade Order Websocket Subscriber connection closed");
        }

        public void Terminate()
        {
            lock (_stateLock)
            {
                _logger.Log(LogLevel.Info, $"Trade Order Websocket Subscriber Termination called");

                _Terminate();
            }
        }

        private void _Terminate()
        {
            if (_activeWebsocket != null)
            {
                _logger.Log(LogLevel.Info, $"Trade Order Websocket Subscriber Closing Active Web Socket");

                try
                {
                    _activeWebsocket.Close();
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error, e);
                    throw;
                }
            }
        }

        public void OnCompleted()
        {
            _logger.Log(LogLevel.Info, $"Trade Order Websocket Subscriber underlying stream completed.");

            Terminate();
        }

        public void OnError(Exception error)
        {
            if (error != null)
            {
                _logger.Log(LogLevel.Error, error);
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