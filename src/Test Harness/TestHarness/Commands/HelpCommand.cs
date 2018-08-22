using TestHarness.Commands.Interfaces;

namespace TestHarness.Commands
{
    public class HelpCommand : ICommand
    {
        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return string.Equals(command, "help", System.StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run()
        {
            Display.Console.WriteToLine(0, "Available commands | help | quit");
        }
    }
}
