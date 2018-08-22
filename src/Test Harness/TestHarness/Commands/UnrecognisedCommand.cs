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
            Display.Console.WriteToLine(0, "Enter 'help' to see available commands");
        }
    }
}
