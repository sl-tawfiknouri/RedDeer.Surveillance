using System;

namespace TestHarness.Commands
{
    public class QuitCommand : ICommand
    {
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
            // ironically this works for now lol
            throw new NotImplementedException();
        }
    }
}
