namespace TestHarness.Factory.Interfaces
{
    using Domain.Surveillance.Scheduling.Interfaces;

    using Infrastructure.Network.Aws.Interfaces;

    using Microsoft.Extensions.Logging;

    using TestHarness.Commands.Interfaces;
    using TestHarness.Configuration.Interfaces;
    using TestHarness.Display.Interfaces;
    using TestHarness.Engine.Heartbeat.Interfaces;
    using TestHarness.Factory.EquitiesFactory.Interfaces;
    using TestHarness.Factory.TradeCancelledFactory.Interfaces;
    using TestHarness.Factory.TradeHighProfitFactory.Interfaces;
    using TestHarness.Factory.TradeHighVolumeFactory.Interfaces;
    using TestHarness.Factory.TradeMarkingTheCloseFactory.Interfaces;
    using TestHarness.Factory.TradeWashTradeFactory.Interfaces;
    using TestHarness.Factory.TradingFactory.Interfaces;
    using TestHarness.Factory.TradingLayeringFactory.Interfaces;
    using TestHarness.Factory.TradingSpoofingFactory.Interfaces;
    using TestHarness.Factory.TradingSpoofingV2Factory.Interfaces;
    using TestHarness.Repository.Api.Interfaces;
    using TestHarness.Repository.Interfaces;
    using TestHarness.State.Interfaces;

    public interface IAppFactory
    {
        IAuroraRepository AuroraRepository { get; }

        IAwsQueueClient AwsQueueClient { get; }

        IPulsatingHeartbeat CancelTradeHeartbeat { get; }

        ICommandManager CommandManager { get; }

        ICommandManifest CommandManifest { get; }

        INetworkConfiguration Configuration { get; }

        IConsole Console { get; }

        bool DisableNuke { get; }

        IEquitiesDataGenerationProcessFactory EquitiesDataGenerationProcessFactory { get; }

        IEquitiesFileDataImportProcessFactory EquitiesFileDataImportProcessFactory { get; }

        IEquitiesFileStorageProcessFactory EquitiesFileStorageProcessFactory { get; }

        IEquitiesProcessFactory EquitiesProcessFactory { get; }

        ITradeHighProfitFactory HighProfitsFactory { get; }

        ITradingLayeringFactory LayeringProcessFactory { get; }

        ILogger Logger { get; }

        IMarketApiRepository MarketApiRepository { get; }

        ITradingMarkingTheCloseFactory MarkingTheCloseFactory { get; }

        IOrderFileStorageProcessFactory OrderFileStorageProcessFactory { get; }

        IScheduledExecutionMessageBusSerialiser ScheduledExecutionSerialiser { get; }

        ISecurityApiRepository SecurityApiRepository { get; }

        IPulsatingHeartbeat SpoofedTradeHeartbeat { get; }

        ITradingSpoofingV2Factory SpoofingV2Factory { get; }

        IProgramState State { get; }

        IStockExchangeStreamFactory StockExchangeStreamFactory { get; }

        ITradeOrderStreamFactory TradeOrderStreamFactory { get; }

        ITradingCancelled2Factory TradingCancelled2Factory { get; }

        ITradingCancelledFactory TradingCancelledOrdersFactory { get; }

        ITradingFactory TradingFactory { get; }

        ITradingFileDataImportProcessFactory TradingFileDataImportProcessFactory { get; }

        ITradingHighVolumeFactory TradingHighVolumeFactory { get; }

        ITradingSpoofingProcessFactory TradingSpoofingFactory { get; }

        ITradeWashTradeFactory WashTradeFactory { get; }
    }
}