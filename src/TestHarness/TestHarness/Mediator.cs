namespace TestHarness
{
    using Microsoft.Extensions.Logging;

    using TestHarness.Commands.Interfaces;
    using TestHarness.Factory;
    using TestHarness.Factory.Interfaces;
    using TestHarness.Interfaces;

    /// <summary>
    ///     Mediates program components of factory; domain; and display
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly IAppFactory _appFactory;

        private readonly ICommandManager _commandManager;

        private readonly ICommandManifest _commandManifest;

        private readonly object _lock = new object();

        public Mediator(IAppFactory appFactory)
        {
            this._appFactory = appFactory ?? new AppFactory(new Configuration.Configuration());
            this._commandManager = this._appFactory.CommandManager;
            this._commandManifest = this._appFactory.CommandManifest;
        }

        /// <summary>
        ///     Commands received during execution
        /// </summary>
        public void ActionCommand(string command)
        {
            lock (this._lock)
            {
                this._commandManager.InterpretIoCommand(command);
            }
        }

        /// <summary>
        ///     Call factory component then initiate initial domain execution
        ///     Does not start any long running processes
        /// </summary>
        public void Initiate()
        {
            lock (this._lock)
            {
                this._appFactory.Logger.LogInformation("Mediator Initiating");

                this._commandManager.InterpretIoCommand(this._commandManifest.Initiate);
            }
        }

        /// <summary>
        ///     Halt running processes; complete resource clean up and terminate
        /// </summary>
        public void Terminate()
        {
            lock (this._lock)
            {
                this._appFactory.Logger.LogInformation("Mediator Terminating");

                this._commandManager.InterpretIoCommand(this._commandManifest.Quit);
            }
        }
    }
}