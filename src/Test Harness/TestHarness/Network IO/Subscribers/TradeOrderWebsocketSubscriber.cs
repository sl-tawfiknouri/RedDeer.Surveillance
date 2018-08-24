using System;
using System.Threading;
using Domain.Equity.Trading.Orders;
using NLog;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;
using Utilities.Network_IO.Websocket_Hosts;

namespace TestHarness.Network_IO.Subscribers
{
    public class TradeOrderWebsocketSubscriber : ITradeOrderWebsocketSubscriber
    {
        private object _stateLock = new object();
        private const int _timeoutSeconds = 10;

        private INetworkSwitch _networkSwitch;
        private IDuplexMessageFactory _duplexMessageFactory;
        private ILogger _logger;

        public TradeOrderWebsocketSubscriber(
            INetworkSwitch networkSwitch,
            IDuplexMessageFactory factory,
            ILogger logger)
        {
            _networkSwitch = networkSwitch ?? throw new ArgumentNullException(nameof(networkSwitch));
            _duplexMessageFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Returns success result
        /// </summary>
        public bool Initiate(string domain, string port)
        {
            lock (_stateLock)
            {
                _logger.Log(
                    LogLevel.Info,
                    $"Initiating trade order websocket subscriber with {_timeoutSeconds} second timeout");

                // allow a 10 second one off attempt to connect
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeoutSeconds));
                return _networkSwitch.Initiate(domain, port, cts.Token);
            }
        }

        public void Terminate()
        {
            lock (_stateLock)
            {
                _networkSwitch.Terminate();
            }
        }

        public void OnCompleted()
        {
            lock (_stateLock)
            {
                _logger.Log(LogLevel.Info, $"Trade Order Websocket Subscriber underlying stream completed.");

                _networkSwitch.Terminate();
            }
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
                var duplexedMessage = _duplexMessageFactory.Create(MessageType.ReddeerTradeFormat, value);
                _networkSwitch.Send(duplexedMessage);
            }
        }
    }
}