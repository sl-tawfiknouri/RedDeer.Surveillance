using NLog;
using TestHarness.Commands;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Factory;
using TestHarness.Interfaces;

namespace TestHarness
{
    /// <summary>
    /// Mediates program components of factory; domain; and display
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly object _lock = new object();

        private IAppFactory _appFactory;
        private IEquityDataGenerator _equityDataGenerator;

        public Mediator(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? new AppFactory();
        }

        /// <summary>
        /// Call factory component then initiate initial domain execution
        /// </summary>
        public void Initiate(InitiateCommand command)
        {
            lock (_lock)
            {
                _appFactory.Logger.Log(LogLevel.Info, "Mediator Initiating");

                if (command == null)
                {
                    _appFactory.Logger.Log(LogLevel.Warn, "Mediator receieved a null initiation command");
                }

                if (_equityDataGenerator != null)
                {
                    _equityDataGenerator.TerminateWalk();
                }

                _equityDataGenerator = _appFactory.Build();

                InitiateProgram();
            }
        }

        private void InitiateProgram()
        {
            var commandManager = _appFactory.CommandManager;
            var programLoop = true;

            while (programLoop)
            {
                var io = System.Console.ReadLine();
                io = io.ToLowerInvariant();

                commandManager.InterpretIOCommand(io);
            }
        }

        /// <summary>
        /// Commands received during execution
        /// </summary>
        public void ActionCommand()
        {

        }

        /// <summary>
        /// Halt running processes; complete resource clean up and terminate
        /// </summary>
        public void Terminate()
        {
            lock (_lock)
            {
                _appFactory.Logger.Log(LogLevel.Info, "Mediator Terminating");

                if (_equityDataGenerator != null)
                {
                    _equityDataGenerator.TerminateWalk();
                }
            }
        }
    }
}
