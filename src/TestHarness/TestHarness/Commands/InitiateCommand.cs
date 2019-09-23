namespace TestHarness.Commands
{
    using System;
    using System.IO;

    using TestHarness.Commands.Interfaces;
    using TestHarness.State.Interfaces;

    public class InitiateCommand : ICommand
    {
        private readonly ICommandManager _commandManager;

        private readonly IProgramState _state;

        public InitiateCommand(IProgramState state, ICommandManager commandManager)
        {
            this._state = state ?? throw new ArgumentNullException(nameof(state));
            this._commandManager = commandManager ?? throw new ArgumentNullException(nameof(commandManager));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return false;

            return string.Equals(command, "initiate", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run(string command)
        {
            this._state.Executing = true;
            SetPlayFilesDirectory();

            while (this._state.Executing)
            {
                var io = Console.ReadLine();

                if (io == null) continue;

                io = io.ToLowerInvariant();

                this._commandManager.InterpretIoCommand(io);
            }
        }

        private static void SetPlayFilesDirectory()
        {
            var executingDirectory = Directory.GetCurrentDirectory();
            var subFolder = Path.Combine(executingDirectory, DemoTradeFileCommand.FileDirectory);
            if (!Directory.Exists(subFolder)) Directory.CreateDirectory(subFolder);
        }
    }
}