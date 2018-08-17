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

        public Mediator()
        {
            _appFactory = new AppFactory();
        }

        /// <summary>
        /// Call factory component then initiate initial domain execution
        /// </summary>
        public void Initiate(InitiateCommand command)
        {
            lock (_lock)
            {
                _equityDataGenerator = _appFactory.Build();
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
                if (_equityDataGenerator != null)
                {
                    _equityDataGenerator.TerminateWalk();
                }
            }
        }
    }
}
