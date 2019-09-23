namespace TestHarness.Commands.Market_Abuse_Commands
{
    using System;

    using TestHarness.Commands.Interfaces;
    using TestHarness.Factory.Interfaces;

    /// <summary>
    ///     legacy nasdaq file approach
    /// </summary>
    public class CancellationCommand : ICommand
    {
        private readonly IAppFactory _appFactory;

        public CancellationCommand(IAppFactory appFactory)
        {
            this._appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return false;

            return string.Equals(command, "run cancelled trade", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run(string command)
        {
            this._appFactory.CancelTradeHeartbeat?.Pulse();
        }
    }
}