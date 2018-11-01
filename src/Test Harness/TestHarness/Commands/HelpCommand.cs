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
            Display.Console.WriteToLine(1, "Available commands | help | quit | run demo | stop demo | run demo csv | stop demo csv | run demo networking | stop demo networking | run spoofed trade | run cancelled trade (legacy) | run demo trade file | stop demo trade file |  run demo trade networking file | stop demo trade networking file | run demo equity market file file.csv | stop demo equity market file | run demo equity market file networking file.csv | stop demo equity market file networking | run schedule rule 01/01/2018 12/01/2018 | nuke | run data generation 20/4/2018 22/04/2018 xlon trades nomarketcsv notradecsv | run cancellation ratio trades 03/01/2018 xlon notrade B188SR5 3163836 B17BBQ5 | run high volume trades 05/01/2018 xlon notrade B188SR5 3163836 B17BBQ5");
        }
    }
}