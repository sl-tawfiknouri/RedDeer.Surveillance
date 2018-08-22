using System;
using TestHarness.Commands.Interfaces;
using TestHarness.Interfaces;

namespace TestHarness.Commands
{
    public class QuitCommand : ICommand
    {
        private readonly IProgramState _programState;

        public QuitCommand(IProgramState programState)
        {
            _programState = programState ?? throw new ArgumentNullException(nameof(programState));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return string.Equals(command, "quit", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run()
        {
            _programState.Executing = false;
        }
    }
}
