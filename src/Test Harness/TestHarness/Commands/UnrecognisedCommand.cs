using TestHarness.Commands.Interfaces;

namespace TestHarness.Commands
{
    public class UnrecognisedCommand : ICommand
    {
        public bool Handles(string command)
        {
            return string.IsNullOrWhiteSpace(command);
        }

        public void Run()
        {
            Display.Console.WriteToLine(1, "Enter 'help' to see available commands");
        }
    }
}
