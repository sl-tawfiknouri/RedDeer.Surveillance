using System.Collections.Generic;
using System.Linq;

namespace TestHarness.Commands
{
    public class CommandManager : ICommandManager
    {
        private IReadOnlyCollection<ICommand> _commands;
        private ICommand _unrecognisedCommand;

        public CommandManager()
        {
            _commands = new List<ICommand>
            {
                new HelpCommand(),
                new QuitCommand(),
                new UnrecognisedCommand()
            };
        }

        public void InterpretIOCommand(string command)
        {
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
