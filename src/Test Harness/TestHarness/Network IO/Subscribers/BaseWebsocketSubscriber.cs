using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Utilities.Network_IO.Websocket_Connections.Interfaces;
using Utilities.Network_IO.Websocket_Hosts.Interfaces;

namespace TestHarness.Network_IO.Subscribers
{
    public abstract class BaseWebsocketSubscriber<T>
    {
        protected object StateLock = new object();
        private const int TimeoutSeconds = 15;

        protected INetworkSwitch NetworkSwitch;
        protected IDuplexMessageFactory DuplexMessageFactory;
        private readonly ILogger _logger;

        protected BaseWebsocketSubscriber(
            INetworkSwitch networkSwitch,
            IDuplexMessageFactory factory,
            ILogger logger)
        {
            NetworkSwitch = networkSwitch ?? throw new ArgumentNullException(nameof(networkSwitch));
            DuplexMessageFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Returns success result
        /// </summary>
        public bool Initiate(string domain, string port)
        {
            lock (StateLock)
            {
                _logger.LogInformation($"Initiating trade order websocket subscriber with {TimeoutSeconds} second timeout");

                // allow a 10 second one off attempt to connect
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutSeconds));
                return NetworkSwitch.Initiate(domain, port, cts.Token);
            }
        }

        public void Terminate()
        {
            lock (StateLock)
            {
                NetworkSwitch.Terminate();
            }
        }

        public void OnCompleted()
        {
            lock (StateLock)
            {
                _logger.LogInformation("Trade Order Websocket Subscriber underlying stream completed.");

                NetworkSwitch.Terminate();
            }
        }

        public void OnError(Exception error)
        {
            if (error != null)
            {
                _logger.LogInformation(error.Message);
            }
        }

        // ReSharper disable once UnusedMember.Global
        public abstract void OnNext(T value);
    }
}
