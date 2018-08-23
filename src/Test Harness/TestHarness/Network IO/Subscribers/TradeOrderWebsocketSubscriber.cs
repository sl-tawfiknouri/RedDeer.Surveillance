using System;
using System.Diagnostics;
using Domain.Equity.Trading.Orders;
using Newtonsoft.Json;
using NLog;
using SuperSocket.ClientEngine;
using TestHarness.Display;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using WebSocket4Net;

namespace TestHarness.Network_IO.Subscribers
{
    public class TradeOrderWebsocketSubscriber : ITradeOrderWebsocketSubscriber
    {
        private object _stateLock = new object();
        private IWebsocketConnectionFactory _websocketFactory;
        private IConnectionWebsocket _activeWebsocket;
        private IConsole _console;
        private ILogger _logger;
        private volatile bool _initiated;

        public TradeOrderWebsocketSubscriber(
            IWebsocketConnectionFactory websocketFactory,
            IConsole console,
            ILogger logger)
        {
            _websocketFactory = websocketFactory ?? throw new ArgumentNullException(nameof(websocketFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        /// <summary>
        /// Returns success result
        /// </summary>
        public bool Initiate(string domain, string port)
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

                _initiated = true;
                
                var connectionString = $"ws://{domain}:{port}/";

                _logger.Log(LogLevel.Info, $"Opening web socket to {connectionString}");

                _activeWebsocket = _websocketFactory.Build(connectionString);
                _activeWebsocket.Opened += new EventHandler(Open_Event);
                _activeWebsocket.Error += new EventHandler<ErrorEventArgs>(Error_Event);
                _activeWebsocket.Closed += new EventHandler(Closed_Event);

                try
                {
                    _console.WriteToUserFeedbackLine($"Opening web socket to {connectionString}");

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    _activeWebsocket.Open();
                    var timeoutCheck = true;
                    while (
                        _activeWebsocket.State == WebSocketState.Connecting
                        && timeoutCheck)
                    {
                        _console.WriteToUserFeedbackLine($"Opening web socket to {connectionString} for last {stopWatch.Elapsed.TotalSeconds} seconds. Timeout at (60) seconds.");

                        if (stopWatch.Elapsed.TotalSeconds > 60)
                        {
                            timeoutCheck = false;
                        }
                    };

                    if (!timeoutCheck)
                    {
                        _logger.Log(
                            LogLevel.Error,
                            $"Trade order websocket subscriber timed out connecting to {connectionString}");

                        _console.WriteToUserFeedbackLine($"Could not open web socket to {connectionString}. Aborting process.");

                        _activeWebsocket.Close();
                    }
                    else
                    {
                        _console.WriteToUserFeedbackLine(string.Empty);
                    }

                    return timeoutCheck;

                }
                catch(Exception e)
                {
                    _logger.Log(LogLevel.Error, e);
                    return false;
                }
            }
        }

        private void Open_Event(object sender, EventArgs e)
        {
            _logger.Log(LogLevel.Info, $"Trade Order Websocket Subscriber Successfully Opened Connection");
        }

        private void Error_Event(object sender, ErrorEventArgs e)
        {
            _logger.Log(LogLevel.Error, $"Trade Order Websocket Subscriber encountered an error {e.Exception.Message}");

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