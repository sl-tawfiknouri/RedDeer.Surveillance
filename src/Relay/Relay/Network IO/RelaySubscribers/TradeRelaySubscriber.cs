using Domain.Equity.Trading.Orders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading;
using Utilities.Network_IO.Websocket_Connections.Interfaces;

namespace Relay.Network_IO.RelaySubscribers
{
    /// <summary>
    /// Receives data from its subscribed (to) trade stream and forwards it onto
    /// a listening websockets service elsewhere via its active web socket connection
    /// </summary>
    public class TradeRelaySubscriber : ITradeRelaySubscriber
    {
        private volatile bool _initiated;
        private object _stateLock = new object();
        private INetworkSwitch _networkSwitch;
        private ILogger _logger;

        public TradeRelaySubscriber(
            INetworkSwitch networkSwitch,
            ILogger<TradeRelaySubscriber> logger)
        {
            _networkSwitch = networkSwitch ?? throw new ArgumentNullException(nameof(networkSwitch));
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
            if (error != null)
            {
                _logger.LogError("Trade Relay Subscriber was passed an error from its source stream", error);

                _initiated = false;
                _networkSwitch.Terminate();
            }
        }

        public void OnNext(TradeOrderFrame value)
        {
            lock (_stateLock)
            {
                if (_initiated)
                {
                    _networkSwitch.Send(value);
                };
            }
        }
    }
}
