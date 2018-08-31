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

        public void Run(string command)
        {
            Display.Console.WriteToLine(1, "Available commands | help | quit | run demo | stop demo | run demo networking | stop demo networking | run prohibited trade | run spoofed trade");
        }
    }
}
