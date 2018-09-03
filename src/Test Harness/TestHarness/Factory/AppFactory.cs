using NLog;
using TestHarness.Commands;
using TestHarness.Commands.Interfaces;
using TestHarness.Display;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.Heartbeat;
using TestHarness.Engine.Heartbeat.Interfaces;
using TestHarness.Factory.EquitiesFactory;
using TestHarness.Factory.EquitiesFactory.Interfaces;
using TestHarness.Factory.Interfaces;
using TestHarness.Factory.NetworkFactory;
using TestHarness.Factory.NetworkFactory.Interfaces;
using TestHarness.Factory.TradingFactory;
using TestHarness.Factory.TradingFactory.Interfaces;
using TestHarness.State.Interfaces;
using TestHarness.State;
using TestHarness.Factory.TradingProhibitedSecurityFactory;
using TestHarness.Factory.TradingSpoofingFactory;
using TestHarness.Network_IO.Interfaces;
using TestHarness.Factory.TradingProhibitedSecurityFactory.Interfaces;
using TestHarness.Factory.TradingSpoofingFactory.Interfaces;
using TestHarness.Configuration.Interfaces;

namespace TestHarness.Factory
{
    /// <summary>
    /// Replace with a DI approach later on | this is a bit messy
    /// </summary>
    public class AppFactory : IAppFactory
    {
        public AppFactory(INetworkConfiguration networkConfiguration)
        {
            Logger = new LogFactory().GetLogger("TestHarnessLogger");

            State = new ProgramState();
            Console = new Console();
            CommandManifest = new CommandManifest();
            ProhibitedSecurityHeartbeat = new PulsatingHeartbeat(); // singleton
            SpoofedTradeHeartbeat = new PulsatingHeartbeat(); // singleton

            EquitiesProcessFactory = new EquitiesProcessFactory(Logger);
            StockExchangeStreamFactory = new StockExchangeStreamFactory();
            NetworkManagerFactory = new NetworkManagerFactory(Console, Logger, networkConfiguration);
            TradingFactory = new TradingFactory.TradingFactory(Logger);
            TradeOrderStreamFactory = new TradeOrderStreamFactory();
            TradingProhibitedSecurityFactory = new TradingProhibitedSecurityProcessFactory(this);
            TradingSpoofingFactory = new TradingSpoofingProcessFactory(this);

            CommandManager = new CommandManager(this, State, Logger, Console);
        }

        /// <summary>
        /// Ctor is used to construct this
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Build is used to construct this
        /// </summary>
        public INetworkManager NetworkManager { get; private set; }

        /// <summary>
        /// Build is used to construct this
        /// </summary>
        public IEquityDataGenerator EquityDataGenerator { get; private set; }

        /// <summary>
        /// Ctor is used to construct this
        /// </summary>
        public ICommandManager CommandManager { get; private set; }

        /// <summary>
        /// Ctor is used to construct this
        /// </summary>
        public ICommandManifest CommandManifest { get; private set; }

        /// <summary>
        /// Ctor is used to construct this
        /// </summary>
        public IProgramState State { get; private set; }

        public IConsole Console { get; private set; }
        
        public IPulsatingHeartbeat ProhibitedSecurityHeartbeat { get; private set; }

        public IPulsatingHeartbeat SpoofedTradeHeartbeat { get; private set; }

        // new factories
        public IEquitiesProcessFactory EquitiesProcessFactory { get; private set; } 

        public IStockExchangeStreamFactory StockExchangeStreamFactory { get; private set; }

        public INetworkManagerFactory NetworkManagerFactory { get; private set; } 

        public ITradingFactory TradingFactory { get; private set; }

        public ITradeOrderStreamFactory TradeOrderStreamFactory { get; private set; }

        public ITradingProhibitedSecurityProcessFactory TradingProhibitedSecurityFactory { get; private set; }

        public ITradingSpoofingProcessFactory TradingSpoofingFactory { get; private set; }
    }
}
