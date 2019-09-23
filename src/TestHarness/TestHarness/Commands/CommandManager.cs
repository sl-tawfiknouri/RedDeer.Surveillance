namespace TestHarness.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Extensions.Logging;

    using TestHarness.Commands.Interfaces;
    using TestHarness.Commands.Market_Abuse_Commands;
    using TestHarness.Display.Interfaces;
    using TestHarness.Factory.Interfaces;
    using TestHarness.State.Interfaces;

    public class CommandManager : ICommandManager
    {
        private readonly IReadOnlyCollection<ICommand> _commands;

        private readonly IConsole _console;

        private readonly ILogger _logger;

        private readonly ICommand _unRecognisedCommand;

        public CommandManager(IAppFactory appFactory, IProgramState programState, ILogger logger, IConsole console)
        {
            this._unRecognisedCommand = new UnRecognisedCommand();
            this._console = console ?? throw new ArgumentNullException(nameof(console));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (appFactory == null) throw new ArgumentNullException(nameof(appFactory));

            this._commands = new List<ICommand>
                                 {
                                     new InitiateCommand(programState, this),
                                     new HelpCommand(),
                                     new QuitCommand(programState),
                                     new DemoCommand(appFactory),
                                     new DemoCsvCommand(appFactory),
                                     new DemoNetworkingCommand(appFactory),
                                     new DemoTradeFileCommand(appFactory),
                                     new DemoTradeFileNetworkingCommand(appFactory),
                                     new SpoofingCommand(appFactory),
                                     new ScheduleRuleCommand(appFactory),
                                     new DemoMarketEquityFileCommand(appFactory),
                                     new DemoMarketEquityFileNetworkingCommand(appFactory),
                                     new CancellationCommand(appFactory),
                                     new NukeCommand(appFactory),
                                     new DemoDataGenerationCommand(appFactory),
                                     new Cancellation2Command(appFactory),
                                     new HighVolumeCommand(appFactory),
                                     new MarkingTheCloseCommand(appFactory),
                                     new Spoofing2Command(appFactory),
                                     new LayeringCommand(appFactory),
                                     new HighProfitCommand(appFactory),
                                     new WashTradeCommand(appFactory),
                                     this._unRecognisedCommand
                                 };
        }

        public void InterpretIoCommand(string command)
        {
            this._logger.LogInformation($"Command Manager received command: {command}");

            this._console.ClearCommandInputLine();

            var executableCommands = this._commands.Where(cmd => cmd.Handles(command)).ToList();

            foreach (var cmd in executableCommands) cmd.Run(command);

            if (!executableCommands.Any()) this._unRecognisedCommand.Run(command);
        }
    }
}