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
        private INetworkTrunk _networkTrunk;
        private ILogger _logger;

        public TradeRelaySubscriber(
            INetworkTrunk networkTrunk,
            ILogger<TradeRelaySubscriber> logger)
        {
            _networkTrunk = networkTrunk ?? throw new ArgumentNullException(nameof(networkTrunk));   
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool Initiate(string domain, string port)
        {
            lock (_stateLock)
            {
                _logger.LogInformation("Trade Relay Subscriber initiating network trunk with 15 second timeout");

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                var success = _networkTrunk.Initiate(domain, port, cts.Token);

               _initiated = success;
                return success;
            }
        }

        public void Terminate()
        {
            lock (_stateLock)
            {
                _networkTrunk.Terminate();
                _initiated = false;
            }
        }

        public void OnCompleted()
        {
            lock (_stateLock)
            {
            _logger.LogInformation($"Trade Relay Subscriber underlying stream completed.");

            _initiated = false;
            _networkTrunk.Terminate();
            }
        }

        public void OnError(Exception error)
        {
            if (error != null)
            {
                _logger.LogError("Trade Relay Subscriber was passed an error from its source stream", error);

                _initiated = false;
                _networkTrunk.Terminate();
            }
        }

        public void OnNext(TradeOrderFrame value)
        {
            lock (_stateLock)
            {
                if (_initiated)
                {
                    _networkTrunk.Send(value);
                };
            }
        }
    }
}
