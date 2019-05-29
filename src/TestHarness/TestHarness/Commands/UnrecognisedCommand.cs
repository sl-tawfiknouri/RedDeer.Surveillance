using TestHarness.Commands.Interfaces;

namespace TestHarness.Commands
{
    public class UnRecognisedCommand : ICommand
    {
        public bool Handles(string command)
        {
            return string.IsNullOrWhiteSpace(command);
        }

        public void Run(string command)
        {
            Display.Console.WriteToLine(1, "Enter 'help' to see available commands");
        }
    }
}
