using System;
using System.Threading;
using TestHarness;

namespace App
{
    public class Program
    {
        static void Main(string[] args)
        {
            InitialSetup();
            InitialWelcomeMessage();

            var mediator = Initiate();
            ProgramLoop(mediator);

            Thread.Sleep(1000);
        }

        private static void InitialSetup()
        {
            Console.Title = "Red Deer Surveillance | Test Harness";
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private static void InitialWelcomeMessage()
        {
            WriteToLine(2, "RED DEER SURVEILLANCE");
            WriteToLine(4, "TEST HARNESS");
        }

        private static Mediator Initiate()
        {
            var mediator = new Mediator(null);
            mediator.Initiate(null);

            return mediator;
        }

        private static void ProgramLoop(Mediator mediator)
        {
            var programLoop = true;
            while (programLoop)
            {
                var io = Console.ReadLine();

                io = io.ToLowerInvariant();

                switch (io)
                {
                    case "help":
                        WriteToLine(0, "Available commands | help | quit");
                        break;

                    case "quit":
                        WriteToLine(0, "Exiting application...");


                        programLoop = false;
                        mediator.Terminate();
                        break;

                    default:
                        WriteToLine(0, "Enter 'help' to see available commands");
                        break;
                }
            }
        }

        private static void WriteToLine(int targetLine, string message)
        {
            System.Console.SetCursorPosition(0, targetLine);
            System.Console.Write(new string(' ', System.Console.WindowWidth));
            System.Console.SetCursorPosition(5, targetLine);
            System.Console.Write(message);
        }
    }
}