using DomainV2.Files;
using DomainV2.Scheduling;
using DomainV2.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;
using TestHarness.Commands;
using TestHarness.Commands.Interfaces;
using TestHarness.Engine.Heartbeat;
using TestHarness.Engine.Heartbeat.Interfaces;
using TestHarness.Factory.EquitiesFactory;
using TestHarness.Factory.EquitiesFactory.Interfaces;
using TestHarness.Factory.Interfaces;
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
using TestHarness.Factory.TradeHighProfitFactory.Interfaces;
using TestHarness.Factory.TradeHighVolumeFactory;
using TestHarness.Factory.TradeHighVolumeFactory.Interfaces;
using TestHarness.Factory.TradeMarkingTheCloseFactory;
using TestHarness.Factory.TradeMarkingTheCloseFactory.Interfaces;
using TestHarness.Factory.TradeWashTradeFactory.Interfaces;
using TestHarness.Factory.TradingLayeringFactory.Interfaces;
using TestHarness.Factory.TradingSpoofingV2Factory.Interfaces;
using TestHarness.Repository;
using TestHarness.Repository.Api;
using TestHarness.Repository.Api.Interfaces;
using TestHarness.Repository.Aurora;
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
            DisableNuke = AwsTags.IsLiveEc2Instance();

            Logger = new LoggerFactory().CreateLogger("TestHarnessLogger");

            State = new ProgramState();
            Console = new Display.Console();
            CommandManifest = new CommandManifest();
            ProhibitedSecurityHeartbeat = new PulsatingHeartbeat(); // singleton
            SpoofedTradeHeartbeat = new PulsatingHeartbeat(); // singleton
            CancelTradeHeartbeat = new PulsatingHeartbeat(); // singleton

            EquitiesProcessFactory = new EquitiesProcessFactory(Logger);
            StockExchangeStreamFactory = new StockExchangeStreamFactory();
            EquitiesDataGenerationProcessFactory = new EquitiesDataGenerationProcessFactory(Logger);
            TradingFactory = new TradingFactory.TradingFactory(Logger);
            TradeOrderStreamFactory = new TradeOrderStreamFactory();
            TradingSpoofingFactory = new TradingSpoofingProcessFactory(this);
            TradingFileDataImportProcessFactory = new TradingFileDataImportProcessFactory(this);
            TradingCancelledOrdersFactory = new TradingCancelledFactory(this);
            EquitiesFileDataImportProcessFactory = new EquitiesFileDataImportProcessFactory(Logger);
            EquitiesFileStorageProcessFactory = new EquitiesFileStorageProcessFactory(Logger);
            OrderFileStorageProcessFactory = new OrderFileStorageProcessFactory(Console, new TradeFileCsvToOrderMapper(), Logger);
            LayeringProcessFactory = new TradingLayeringFactory.TradingLayeringFactory(Logger);
            TradingCancelled2Factory = new TradingCancelled2Factory(Logger);
            TradingHighVolumeFactory = new TradingHighVolumeFactory(Logger);
            MarkingTheCloseFactory = new TradingMarkingTheCloseFactory(Logger);
            SpoofingV2Factory = new TradingSpoofingV2Factory.TradingSpoofingV2Factory(Logger);
            HighProfitsFactory = new TradeHighProfitFactory.TradeHighProfitFactory(Logger);
            WashTradeFactory = new TradeWashTradeFactory.TradeWashTradeFactory(Logger);

            AwsQueueClient = new AwsQueueClient(null);
            ScheduledExecutionSerialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
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
        public ITradeHighProfitFactory HighProfitsFactory { get; }

        public ITradingLayeringFactory LayeringProcessFactory { get; }

        public ITradingMarkingTheCloseFactory MarkingTheCloseFactory { get; }
        public ITradingSpoofingV2Factory SpoofingV2Factory { get; }
        public ITradeWashTradeFactory WashTradeFactory { get; }

        public IStockExchangeStreamFactory StockExchangeStreamFactory { get; }

        public IOrderFileStorageProcessFactory OrderFileStorageProcessFactory { get; }

        public ITradingFactory TradingFactory { get; }

        public ITradeOrderStreamFactory TradeOrderStreamFactory { get; }

        public ITradingSpoofingProcessFactory TradingSpoofingFactory { get; }

        public ITradingFileDataImportProcessFactory TradingFileDataImportProcessFactory { get; }

        public ITradingCancelledFactory TradingCancelledOrdersFactory { get; }

        public ITradingCancelled2Factory TradingCancelled2Factory { get; }

        public ITradingHighVolumeFactory TradingHighVolumeFactory { get; }

        public IAwsQueueClient AwsQueueClient { get; }

        public IScheduledExecutionMessageBusSerialiser ScheduledExecutionSerialiser { get; }

        public INetworkConfiguration Configuration { get; }

        public IEquitiesFileDataImportProcessFactory EquitiesFileDataImportProcessFactory { get; }

        public IEquitiesFileStorageProcessFactory EquitiesFileStorageProcessFactory { get; }
        public IAuroraRepository AuroraRepository { get; }

        public IMarketApiRepository MarketApiRepository { get; }

        public bool DisableNuke { get; }
    }
}
