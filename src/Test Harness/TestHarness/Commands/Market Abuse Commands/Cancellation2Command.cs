using System;
using TestHarness.Commands.Interfaces;
using TestHarness.Factory.Interfaces;
using TestHarness.Network_IO.Interfaces;

namespace TestHarness.Commands.Market_Abuse_Commands
{
    /// <summary>
    /// Edition 2 of the cancellation command
    /// Prior edition injected cancelled orders into the
    /// on going data generation process this one injects into
    /// historic data only and calls run schedule rule after the injection
    /// </summary>
    public class Cancellation2Command : ICommand
    {
        private readonly IAppFactory _appFactory;
        private INetworkManager _networkManager;

        private readonly object _lock = new object();

        public Cancellation2Command(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return command.ToLower().Contains("run cancellation2 trade");
        }

        public void Run(string command)
        {
            lock (_lock)
            {
                var console = _appFactory.Console;

                _networkManager =
                    _appFactory
                        .NetworkManagerFactory
                        .CreateWebsockets();

                var tradeStream =
                    _appFactory
                        .TradeOrderStreamFactory
                        .CreateDisplayable(console);

                // start networking processes
                var connectionEstablished = _networkManager.InitiateAllNetworkConnections();

                if (!connectionEstablished)
                {
                    console.WriteToUserFeedbackLine("Failed to establish network connections. Aborting run data generation.");
                    return;
                }

                connectionEstablished = _networkManager.AttachTradeOrderSubscriberToStream(tradeStream);
                if (!connectionEstablished)
                {
                    console.WriteToUserFeedbackLine("Failed to establish trade network connections. Aborting run data generation.");
                    return;
                }

                
            }
        }
    }
}
