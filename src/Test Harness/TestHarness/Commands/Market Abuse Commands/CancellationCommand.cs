using System;
using TestHarness.Commands.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Commands.Market_Abuse_Commands
{
    public class CancellationCommand : ICommand
    {
        private readonly IAppFactory _appFactory;

        public CancellationCommand(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return string.Equals(command, "run cancelled trade", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run(string command)
        {
            _appFactory.CancelTradeHeartbeat?.Throb();
        }
    }
}