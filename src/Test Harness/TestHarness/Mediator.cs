﻿using NLog;
using TestHarness.Commands.Interfaces;
using TestHarness.Factory;
using TestHarness.Factory.Interfaces;
using TestHarness.Interfaces;

namespace TestHarness
{
    /// <summary>
    /// Mediates program components of factory; domain; and display
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly object _lock = new object();

        private readonly IAppFactory _appFactory;
        private readonly ICommandManager _commandManager;
        private readonly ICommandManifest _commandManifest;

        public Mediator(IAppFactory appFactory)
        {
            _appFactory = appFactory ?? new AppFactory(new Configuration.Configuration());
            _commandManager = _appFactory.CommandManager;
            _commandManifest = _appFactory.CommandManifest;
        }

        /// <summary>
        /// Call factory component then initiate initial domain execution
        /// Does not start any long running processes
        /// </summary>
        public void Initiate()
        {
            lock (_lock)
            {
                _appFactory.Logger.Log(LogLevel.Info, "Mediator Initiating");

                _commandManager.InterpretIoCommand(_commandManifest.Initiate);
            }
        }

        /// <summary>
        /// Commands received during execution
        /// </summary>
        public void ActionCommand(string command)
        {
            lock (_lock)
            {
                _commandManager.InterpretIoCommand(command);
            }
        }

        /// <summary>
        /// Halt running processes; complete resource clean up and terminate
        /// </summary>
        public void Terminate()
        {
            lock (_lock)
            {
                _appFactory.Logger.Log(LogLevel.Info, "Mediator Terminating");

                _commandManager.InterpretIoCommand(_commandManifest.Quit);
            }
        }
    }
}
