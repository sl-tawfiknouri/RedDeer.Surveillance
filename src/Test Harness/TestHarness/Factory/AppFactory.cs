﻿using Domain.Scheduling;
using Domain.Scheduling.Interfaces;
using NLog;
using TestHarness.Commands;
using TestHarness.Commands.Interfaces;
using TestHarness.Display;
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
using TestHarness.Factory.TradingSpoofingFactory;
using TestHarness.Factory.TradingSpoofingFactory.Interfaces;
using TestHarness.Configuration.Interfaces;
using TestHarness.Display.Interfaces;
using TestHarness.Factory.TradeCancelledFactory;
using TestHarness.Factory.TradeCancelledFactory.Interfaces;
using TestHarness.Repository;
using TestHarness.Repository.Api;
using TestHarness.Repository.Api.Interfaces;
using TestHarness.Repository.Interfaces;
using Utilities.Aws_IO;
using Utilities.Aws_IO.Interfaces;

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
            CancelTradeHeartbeat = new PulsatingHeartbeat(); // singleton

            EquitiesProcessFactory = new EquitiesProcessFactory(Logger);
            StockExchangeStreamFactory = new StockExchangeStreamFactory();
            EquitiesDataGenerationProcessFactory = new EquitiesDataGenerationProcessFactory(Logger);
            NetworkManagerFactory = new NetworkManagerFactory(Console, Logger, networkConfiguration);
            TradingFactory = new TradingFactory.TradingFactory(Logger);
            TradeOrderStreamFactory = new TradeOrderStreamFactory();
            TradingSpoofingFactory = new TradingSpoofingProcessFactory(this);
            TradingFileRelayProcessFactory = new TradingFileRelayProcessFactory(this);
            TradingCancelledOrdersFactory = new TradingCancelledFactory(this);
            EquitiesFileRelayProcessFactory = new EquitiesFileRelayProcessFactory(Logger);
            EquitiesFileStorageProcessFactory = new EquitiesFileStorageProcessFactory(Logger);
            OrderFileStorageProcessFactory = new OrderFileStorageProcessFactory(Console, Logger);

            AwsQueueClient = new AwsQueueClient(networkConfiguration, null);
            ScheduledExecutionSerialiser = new ScheduledExecutionMessageBusSerialiser();
            Configuration = networkConfiguration;
            AuroraRepository = new AuroraRepository(networkConfiguration, Console);

            SecurityApiRepository = new SecurityApiRepository(networkConfiguration);
            MarketApiRepository = new MarketApiRepository(networkConfiguration);

            CommandManager = new CommandManager(this, State, Logger, Console);
        }

        /// <summary>
        /// Ctor is used to construct this
        /// </summary>
        public ILogger Logger { get; }

        public ISecurityApiRepository SecurityApiRepository { get; }

        /// <summary>
        /// Ctor is used to construct this
        /// </summary>
        public ICommandManager CommandManager { get; }

        /// <summary>
        /// Ctor is used to construct this
        /// </summary>
        public ICommandManifest CommandManifest { get; }

        /// <summary>
        /// Ctor is used to construct this
        /// </summary>
        public IProgramState State { get; }

        public IConsole Console { get; }
        
        public IPulsatingHeartbeat ProhibitedSecurityHeartbeat { get; }

        public IPulsatingHeartbeat SpoofedTradeHeartbeat { get; }

        public IPulsatingHeartbeat CancelTradeHeartbeat { get; }

        // new factories
        public IEquitiesProcessFactory EquitiesProcessFactory { get; }

        public IEquitiesDataGenerationProcessFactory EquitiesDataGenerationProcessFactory { get; }

        public IStockExchangeStreamFactory StockExchangeStreamFactory { get; }

        public IOrderFileStorageProcessFactory OrderFileStorageProcessFactory { get; }

        public INetworkManagerFactory NetworkManagerFactory { get; } 

        public ITradingFactory TradingFactory { get; }

        public ITradeOrderStreamFactory TradeOrderStreamFactory { get; }

        public ITradingSpoofingProcessFactory TradingSpoofingFactory { get; }

        public ITradingFileRelayProcessFactory TradingFileRelayProcessFactory { get; }

        public ITradingCancelledFactory TradingCancelledOrdersFactory { get; }

        public IAwsQueueClient AwsQueueClient { get; }

        public IScheduledExecutionMessageBusSerialiser ScheduledExecutionSerialiser { get; }

        public INetworkConfiguration Configuration { get; }

        public IEquitiesFileRelayProcessFactory EquitiesFileRelayProcessFactory { get; }

        public IEquitiesFileStorageProcessFactory EquitiesFileStorageProcessFactory { get; }
        public IAuroraRepository AuroraRepository { get; }

        public IMarketApiRepository MarketApiRepository { get; }
    }
}
