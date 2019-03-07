using Domain.Surveillance.Scheduling.Interfaces;
using Infrastructure.Network.Aws_IO.Interfaces;
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

namespace TestHarness.Factory.Interfaces
{
    public interface IAppFactory
    {
        ILogger Logger { get; }

        ISecurityApiRepository SecurityApiRepository { get; }

        ICommandManager CommandManager { get; }

        ICommandManifest CommandManifest { get; }

        IProgramState State { get; }

        IConsole Console { get; }

        IPulsatingHeartbeat SpoofedTradeHeartbeat { get; }

        IPulsatingHeartbeat CancelTradeHeartbeat { get; }

        IEquitiesProcessFactory EquitiesProcessFactory { get; }

        IEquitiesDataGenerationProcessFactory EquitiesDataGenerationProcessFactory { get; }

        ITradeHighProfitFactory HighProfitsFactory { get; }

        ITradingLayeringFactory LayeringProcessFactory { get;}
        ITradingMarkingTheCloseFactory MarkingTheCloseFactory { get; }
        ITradingSpoofingV2Factory SpoofingV2Factory { get; }
        ITradeWashTradeFactory WashTradeFactory { get; }

        IStockExchangeStreamFactory StockExchangeStreamFactory { get; }

        IOrderFileStorageProcessFactory OrderFileStorageProcessFactory { get; }

        ITradingFactory TradingFactory { get; }

        ITradeOrderStreamFactory TradeOrderStreamFactory { get; }
        ITradingSpoofingProcessFactory TradingSpoofingFactory { get; }
        ITradingFileDataImportProcessFactory TradingFileDataImportProcessFactory { get; }
        ITradingCancelledFactory TradingCancelledOrdersFactory { get; }
        ITradingCancelled2Factory TradingCancelled2Factory { get; }
        ITradingHighVolumeFactory TradingHighVolumeFactory { get; }

        IAwsQueueClient AwsQueueClient { get; }
        IScheduledExecutionMessageBusSerialiser ScheduledExecutionSerialiser { get; }
        INetworkConfiguration Configuration { get; }
        IEquitiesFileDataImportProcessFactory EquitiesFileDataImportProcessFactory { get; }
        IEquitiesFileStorageProcessFactory EquitiesFileStorageProcessFactory { get; }
        IAuroraRepository AuroraRepository { get; }
        IMarketApiRepository MarketApiRepository { get; }
        bool DisableNuke { get; }

    }
}