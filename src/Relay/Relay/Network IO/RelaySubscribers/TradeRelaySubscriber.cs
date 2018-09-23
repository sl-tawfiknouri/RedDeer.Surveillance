using Microsoft.Extensions.Logging;
using Relay.Network_IO.RelaySubscribers.Interfaces;
using System;
using System.Threading;
using Domain.Trades.Orders;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace Relay.Network_IO.RelaySubscribers
{
    /// <summary>
    /// Receives data from its subscribed (to) trade stream and forwards it onto
    /// a listening websockets service elsewhere via its active web socket connection
    /// </summary>
    public class TradeRelaySubscriber : ITradeRelaySubscriber
    {
        private volatile bool _initiated;
        private readonly object _stateLock = new object();
        private readonly INetworkSwitch _networkSwitch;
        private readonly IDuplexMessageFactory _duplexMessageFactory;
        private readonly ILogger _logger;

        public TradeRelaySubscriber(
            INetworkSwitch networkSwitch,
            IDuplexMessageFactory duplexMessageFactory,
            ILogger<TradeRelaySubscriber> logger)
        {
            _networkSwitch = networkSwitch ?? throw new ArgumentNullException(nameof(networkSwitch));
            _duplexMessageFactory = duplexMessageFactory ?? throw new ArgumentNullException(nameof(duplexMessageFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool Initiate(string domain, string port)
        {
            lock (_stateLock)
            {
                _logger.LogInformation("Trade Relay Subscriber initiating network trunk with 15 second timeout");

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                var success = _networkSwitch.Initiate(domain, port, cts.Token);

               _initiated = true;
                return success;
            }
        }

        public void Terminate()
        {
            lock (_stateLock)
            {
                _networkSwitch.Terminate();
                _initiated = false;
            }
        }

        public void OnCompleted()
        {
            lock (_stateLock)
            {
                _logger.LogInformation($"Trade Relay Subscriber underlying stream completed.");

                _initiated = false;
                _networkSwitch.Terminate();
            }
        }

        public void OnError(Exception error)
        {
            _logger.LogError("Trade Relay Subscriber was passed an error from its source stream", error);

            _initiated = false;
            _networkSwitch.Terminate();
        }

        public void OnNext(TradeOrderFrame value)
        {
            lock (_stateLock)
            {
                if (!_initiated)
                {
                    return;
                }

                var duplexedMessage = _duplexMessageFactory.Create(MessageType.ReddeerTradeFormat, value);
                _networkSwitch.Send(duplexedMessage);
            }
        }
    }
}
