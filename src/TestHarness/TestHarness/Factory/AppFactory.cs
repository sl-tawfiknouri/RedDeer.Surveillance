namespace TestHarness.Factory
{
    using Domain.Surveillance.Scheduling;
    using Domain.Surveillance.Scheduling.Interfaces;

    using Infrastructure.Network.Aws;
    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using SharedKernel.Files.Orders;

    using TestHarness.Commands;
    using TestHarness.Commands.Interfaces;
    using TestHarness.Configuration.Interfaces;
    using TestHarness.Display;
    using TestHarness.Display.Interfaces;
    using TestHarness.Engine.Heartbeat;
    using TestHarness.Engine.Heartbeat.Interfaces;
    using TestHarness.Factory.EquitiesFactory;
    using TestHarness.Factory.EquitiesFactory.Interfaces;
    using TestHarness.Factory.Interfaces;
    using TestHarness.Factory.TradeCancelledFactory;
    using TestHarness.Factory.TradeCancelledFactory.Interfaces;
    using TestHarness.Factory.TradeHighProfitFactory.Interfaces;
    using TestHarness.Factory.TradeHighVolumeFactory;
    using TestHarness.Factory.TradeHighVolumeFactory.Interfaces;
    using TestHarness.Factory.TradeMarkingTheCloseFactory;
    using TestHarness.Factory.TradeMarkingTheCloseFactory.Interfaces;
    using TestHarness.Factory.TradeWashTradeFactory.Interfaces;
    using TestHarness.Factory.TradingFactory;
    using TestHarness.Factory.TradingFactory.Interfaces;
    using TestHarness.Factory.TradingLayeringFactory.Interfaces;
    using TestHarness.Factory.TradingSpoofingFactory;
    using TestHarness.Factory.TradingSpoofingFactory.Interfaces;
    using TestHarness.Factory.TradingSpoofingV2Factory.Interfaces;
    using TestHarness.Repository;
    using TestHarness.Repository.Api;
    using TestHarness.Repository.Api.Interfaces;
    using TestHarness.Repository.Aurora;
    using TestHarness.Repository.Interfaces;
    using TestHarness.State;
    using TestHarness.State.Interfaces;

    /// <summary>
    ///     Replace with a DI approach later on | this is a bit messy
    /// </summary>
    public class AppFactory : IAppFactory
    {
        public AppFactory(INetworkConfiguration networkConfiguration)
        {
            this.DisableNuke = AwsTags.IsLiveEc2Instance();

            this.Logger = new LoggerFactory().CreateLogger("TestHarnessLogger");

            this.State = new ProgramState();
            this.Console = new Console();
            this.CommandManifest = new CommandManifest();
            this.ProhibitedSecurityHeartbeat = new PulsatingHeartbeat(); // singleton
            this.SpoofedTradeHeartbeat = new PulsatingHeartbeat(); // singleton
            this.CancelTradeHeartbeat = new PulsatingHeartbeat(); // singleton

            this.EquitiesProcessFactory = new EquitiesProcessFactory(this.Logger);
            this.StockExchangeStreamFactory = new StockExchangeStreamFactory();
            this.EquitiesDataGenerationProcessFactory = new EquitiesDataGenerationProcessFactory(this.Logger);
            this.TradingFactory = new TradingFactory.TradingFactory(this.StockExchangeStreamFactory, this.Logger);
            this.TradeOrderStreamFactory = new TradeOrderStreamFactory();
            this.TradingSpoofingFactory = new TradingSpoofingProcessFactory(this);
            this.TradingFileDataImportProcessFactory = new TradingFileDataImportProcessFactory(this);
            this.TradingCancelledOrdersFactory = new TradingCancelledFactory(this);
            this.EquitiesFileDataImportProcessFactory = new EquitiesFileDataImportProcessFactory(this.Logger);
            this.EquitiesFileStorageProcessFactory = new EquitiesFileStorageProcessFactory(this.Logger);
            this.OrderFileStorageProcessFactory = new OrderFileStorageProcessFactory(
                this.Console,
                new OrderFileToOrderSerialiser(),
                this.Logger);
            this.LayeringProcessFactory = new TradingLayeringFactory.TradingLayeringFactory(this.Logger);
            this.TradingCancelled2Factory = new TradingCancelled2Factory(this.Logger);
            this.TradingHighVolumeFactory = new TradingHighVolumeFactory(this.Logger);
            this.MarkingTheCloseFactory = new TradingMarkingTheCloseFactory(this.Logger);
            this.SpoofingV2Factory = new TradingSpoofingV2Factory.TradingSpoofingV2Factory(this.Logger);
            this.HighProfitsFactory = new TradeHighProfitFactory.TradeHighProfitFactory(this.Logger);
            this.WashTradeFactory = new TradeWashTradeFactory.TradeWashTradeFactory(this.Logger);

            this.AwsQueueClient = new AwsQueueClient(null);
            this.ScheduledExecutionSerialiser =
                new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            this.Configuration = networkConfiguration;
            this.AuroraRepository = new AuroraRepository(networkConfiguration, this.Console);

            this.SecurityApiRepository = new SecurityApiRepository(networkConfiguration);
            this.MarketApiRepository = new MarketApiRepository(networkConfiguration);

            this.CommandManager = new CommandManager(this, this.State, this.Logger, this.Console);
        }

        public IAuroraRepository AuroraRepository { get; }

        public IAwsQueueClient AwsQueueClient { get; }

        public IPulsatingHeartbeat CancelTradeHeartbeat { get; }

        /// <summary>
        ///     Ctor is used to construct this
        /// </summary>
        public ICommandManager CommandManager { get; }

        /// <summary>
        ///     Ctor is used to construct this
        /// </summary>
        public ICommandManifest CommandManifest { get; }

        public INetworkConfiguration Configuration { get; }

        public IConsole Console { get; }

        public bool DisableNuke { get; }

        public IEquitiesDataGenerationProcessFactory EquitiesDataGenerationProcessFactory { get; }

        public IEquitiesFileDataImportProcessFactory EquitiesFileDataImportProcessFactory { get; }

        public IEquitiesFileStorageProcessFactory EquitiesFileStorageProcessFactory { get; }

        // new factories
        public IEquitiesProcessFactory EquitiesProcessFactory { get; }

        public ITradeHighProfitFactory HighProfitsFactory { get; }

        public ITradingLayeringFactory LayeringProcessFactory { get; }

        /// <summary>
        ///     Ctor is used to construct this
        /// </summary>
        public ILogger Logger { get; }

        public IMarketApiRepository MarketApiRepository { get; }

        public ITradingMarkingTheCloseFactory MarkingTheCloseFactory { get; }

        public IOrderFileStorageProcessFactory OrderFileStorageProcessFactory { get; }

        public IPulsatingHeartbeat ProhibitedSecurityHeartbeat { get; }

        public IScheduledExecutionMessageBusSerialiser ScheduledExecutionSerialiser { get; }

        public ISecurityApiRepository SecurityApiRepository { get; }

        public IPulsatingHeartbeat SpoofedTradeHeartbeat { get; }

        public ITradingSpoofingV2Factory SpoofingV2Factory { get; }

        /// <summary>
        ///     Ctor is used to construct this
        /// </summary>
        public IProgramState State { get; }

        public IStockExchangeStreamFactory StockExchangeStreamFactory { get; }

        public ITradeOrderStreamFactory TradeOrderStreamFactory { get; }

        public ITradingCancelled2Factory TradingCancelled2Factory { get; }

        public ITradingCancelledFactory TradingCancelledOrdersFactory { get; }

        public ITradingFactory TradingFactory { get; }

        public ITradingFileDataImportProcessFactory TradingFileDataImportProcessFactory { get; }

        public ITradingHighVolumeFactory TradingHighVolumeFactory { get; }

        public ITradingSpoofingProcessFactory TradingSpoofingFactory { get; }

        public ITradeWashTradeFactory WashTradeFactory { get; }
    }
}