using TestHarness.Commands.Interfaces;
using TestHarness.Factory.Interfaces;

namespace TestHarness.Commands
{
    public class DemoDataGenerationCommand : ICommand
    {
        private readonly IAppFactory _appFactory;
        private readonly object _lock = new object();

        public DemoDataGenerationCommand(IAppFactory appFactory)
        {
            _appFactory = appFactory;
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return false;
            }

            return command.ToLower().Contains("run data generation");
        }

        public void Run(string command)
        {
            lock (_lock)
            {
            }
        }
    }
}
