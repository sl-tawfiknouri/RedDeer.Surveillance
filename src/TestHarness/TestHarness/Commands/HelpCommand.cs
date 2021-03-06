﻿namespace TestHarness.Commands
{
    using System;

    using TestHarness.Commands.Interfaces;

    using Console = TestHarness.Display.Console;

    public class HelpCommand : ICommand
    {
        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return false;

            return string.Equals(command, "help", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run(string command)
        {
            Console.WriteToLine(
                1,
                "Available commands | help | quit | run demo | stop demo | run demo csv | stop demo csv | run demo networking | stop demo networking | run spoofed trade (legacy) | run cancelled trade (legacy) | run demo trade file | stop demo trade file |  run demo trade networking file | stop demo trade networking file | run demo equity market file file.csv | stop demo equity market file | run demo equity market file networking file.csv | stop demo equity market file networking | run schedule rule 01/01/2018 12/01/2018 | nuke | run data generation 20/4/2018 22/04/2018 xlon trades nomarketcsv notradecsv filter-ftse-100 | run cancellation ratio trades 03/01/2018 xlon notrade nomarketcsv notradecsv B188SR5 3163836 B17BBQ5 | run high volume trades 05/01/2018 xlon notrade nomarketcsv notradecsv B188SR5 3163836 B17BBQ5 | run marking the close 08/01/2018 xlon notrade nomarketcsv notradecsv B188SR5 3163836 B17BBQ5 | run spoofing trades 09/01/2018 xlon notrade nomarketcsv notradecsv B188SR5 3163836 B17BBQ5 | run layering trades 10/01/2018 xlon notrade nomarketcsv notradecsv B188SR5 3163836 B17BBQ5 | run high profits 11/01/2018 xlon notrade nomarketcsv notradecsv B188SR5 3163836 B17BBQ5 | run wash trades 12/01/2018 xlon notrade nomarketcsv notradecsv B188SR5 3163836 B17BBQ5");
        }
    }
}