using System;
using TestHarness.Commands.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Commands.Market_Abuse_Commands
{
    public class MarkingTheCloseCommand : ICommand
    {
        private readonly IAppFactory _appFactory;

        public MarkingTheCloseCommand(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return command.ToLower().Contains("run marking the close");
        }

        public void Run(string command)
        {
            var console = _appFactory.Console;

            console.WriteToUserFeedbackLine("hello world");
        }
    }
}
