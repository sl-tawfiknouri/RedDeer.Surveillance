namespace TestHarness.Commands
{
    using TestHarness.Commands.Interfaces;
    using TestHarness.Display;

    public class UnRecognisedCommand : ICommand
    {
        public bool Handles(string command)
        {
            return string.IsNullOrWhiteSpace(command);
        }

        public void Run(string command)
        {
            Console.WriteToLine(1, "Enter 'help' to see available commands");
        }
    }
}