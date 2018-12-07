using System;
using System.Threading;
using DataImport.Network_IO.RelaySubscribers.Interfaces;
using Domain.Equity.Frames;
using Microsoft.Extensions.Logging;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Utilities.Network_IO.Websocket_Hosts;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace DataImport.Network_IO.RelaySubscribers
{
    public class EquityRelaySubscriber : IEquityRelaySubscriber
    {
        private volatile bool _initiated;
        private readonly object _stateLock = new object();
        private readonly INetworkSwitch _networkSwitch;
        private readonly IDuplexMessageFactory _duplexMessageFactory;
        private readonly ILogger _logger;

        public EquityRelaySubscriber(
            INetworkSwitch networkSwitch,
            IDuplexMessageFactory duplexMessageFactory,
            ILogger<EquityRelaySubscriber> logger)
        {
            _networkSwitch = networkSwitch ?? throw new ArgumentNullException(nameof(networkSwitch));
            _duplexMessageFactory = duplexMessageFactory ?? throw new ArgumentNullException(nameof(duplexMessageFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool Initiate(string domain, string port)
        {
            lock (_stateLock)
            {
                _logger.LogInformation("Equity Relay Subscriber initiating network trunk with 15 second timeout");

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
                _logger.LogInformation("Equity Relay Subscriber underlying stream completed.");

                _initiated = false;
                _networkSwitch.Terminate();
            }
        }

        public void OnError(Exception error)
        {
            _logger.LogError("Equity Relay Subscriber was passed an error from its source stream", error);

            _initiated = false;
            _networkSwitch.Terminate();
        }

        public void OnNext(ExchangeFrame value)
        {
            lock (_stateLock)
            {
                if (!_initiated)
                {
                    return;
                }

                var duplexedMessage = _duplexMessageFactory.Create(MessageType.RedderStockFormat, value);
                _networkSwitch.Send(duplexedMessage);
            }
        }
    }
}