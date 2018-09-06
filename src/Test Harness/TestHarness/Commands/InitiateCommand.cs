using System;
using TestHarness.Commands.Interfaces;
using TestHarness.State.Interfaces;

namespace TestHarness.Commands
{
    public class InitiateCommand : ICommand
    {
        private readonly IProgramState _state;
        private readonly ICommandManager _commandManager;

        public InitiateCommand(
            IProgramState state,
            ICommandManager commandManager)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _commandManager = commandManager ?? throw new ArgumentNullException(nameof(commandManager));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return string.Equals(command, "initiate", StringComparison.InvariantCultureIgnoreCase);
        }

        public void Run(string command)
        {
            _state.Executing = true;

            while (_state.Executing)
            {
                var io = Console.ReadLine();

                if (io == null)
                {
                    continue;
                }

                io = io.ToLowerInvariant();

                _commandManager.InterpretIOCommand(io);
            }
        }
    }
}
