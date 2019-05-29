using System;
using TestHarness.Commands.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Commands.Market_Abuse_Commands
{
    /// <summary>
    /// Legacy approach of using a NASDAQ closing file to inject data
    /// </summary>
    public class SpoofingCommand : ICommand
    {
        private readonly IAppFactory _appFactory;

        public SpoofingCommand(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return string.Equals(command, "run spoofed trade", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run(string command)
        {
            _appFactory.SpoofedTradeHeartbeat?.Pulse();
        }
    }
}
