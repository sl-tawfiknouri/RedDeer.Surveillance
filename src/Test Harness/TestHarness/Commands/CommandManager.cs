using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using TestHarness.Commands.Interfaces;
using TestHarness.Display;
using TestHarness.Interfaces;

namespace TestHarness.Commands
{
    public class CommandManager : ICommandManager
    {
        private IReadOnlyCollection<ICommand> _commands;
        private ICommand _unrecognisedCommand;
        private IConsole _console;
        private ILogger _logger;

        public CommandManager(
            IProgramState programState,
            ILogger logger,
            IConsole console)
        {
            _unrecognisedCommand = new UnrecognisedCommand();
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _commands = new List<ICommand>
            {
                new HelpCommand(),
                new QuitCommand(programState),
                _unrecognisedCommand,
                new InitiateCommand(programState, this)
            };
        }

        public void InterpretIOCommand(string command)
        {
            _logger.Info($"Command Manager receieved command: {command}");

            _console.ClearCommandInputLine();

            var executableCommands =
                _commands
                    .Where(cmd => cmd.Handles(command))
                    .ToList();

            foreach (var cmd in executableCommands)
            {
                cmd.Run();
            }

            if (executableCommands == null
                || !executableCommands.Any())
            {
                _unrecognisedCommand.Run();  
            }
        }
    }
}
