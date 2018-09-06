using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using TestHarness.Commands.Interfaces;
using TestHarness.Commands.Market_Abuse_Commands;
using TestHarness.Display;
using TestHarness.Factory.Interfaces;
using TestHarness.State.Interfaces;

namespace TestHarness.Commands
{
    public class CommandManager : ICommandManager
    {
        private readonly IReadOnlyCollection<ICommand> _commands;
        private readonly ICommand _unrecognisedCommand;
        private readonly IConsole _console;
        private readonly ILogger _logger;

        public CommandManager(
            IAppFactory appFactory,
            IProgramState programState,
            ILogger logger,
            IConsole console)
        {
            _unrecognisedCommand = new UnrecognisedCommand();
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (appFactory == null)
            {
                throw new ArgumentNullException(nameof(appFactory));
            }

            _commands = new List<ICommand>
            {
                new InitiateCommand(programState, this),
                new HelpCommand(),
                new QuitCommand(programState),
                new DemoCommand(appFactory),
                new DemoNetworkingCommand(appFactory),
                new DemoTradeFileCommand(appFactory),
                new DemoTradeFileNetworkingCommand(appFactory),
                new TradeProhibitedSecurityCommand(appFactory),
                new SpoofingCommand(appFactory),
               _unrecognisedCommand,
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
                cmd.Run(command);
            }
            
            if (!executableCommands.Any())
            {
                _unrecognisedCommand.Run(command);
            }
        }
    }
}
