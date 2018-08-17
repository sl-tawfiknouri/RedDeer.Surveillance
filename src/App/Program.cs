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

            Thread.Sleep(2000);
        }

        private static void InitialSetup()
        {
            Console.Title = "Red Deer Surveillance | Test Harness";
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private static void InitialWelcomeMessage()
        {
            Console.SetCursorPosition(2, 2);
            Console.WriteLine("RED DEER SURVEILLANCE");

            Console.SetCursorPosition(2, 4);
            Console.WriteLine("TEST HARNESS");
            Console.WriteLine();
        }

        private static Mediator Initiate()
        {
            var mediator = new Mediator(null);
            mediator.Initiate(null);

            return mediator;
        }

        private static void Commands()
        {
            Console.WriteLine();
            Console.WriteLine("Available commands");
            Console.WriteLine();
            Console.WriteLine("--help");
            Console.WriteLine("--quit");
            Console.WriteLine();
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
                        Commands();
                        break;

                    case "quit":
                        Console.WriteLine("Exiting application...");

                        programLoop = false;
                        mediator.Terminate();
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("Enter 'help' to see available commands");
                        Console.WriteLine();
                        break;
                }
            }
        }
    }
}