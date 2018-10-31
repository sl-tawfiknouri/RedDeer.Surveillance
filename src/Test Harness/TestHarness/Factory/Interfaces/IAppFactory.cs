using Domain.Scheduling.Interfaces;
using NLog;
using TestHarness.Commands.Interfaces;
using TestHarness.Configuration.Interfaces;
using TestHarness.Display.Interfaces;
using TestHarness.Engine.Heartbeat.Interfaces;
using TestHarness.Factory.EquitiesFactory.Interfaces;
using TestHarness.Factory.NetworkFactory.Interfaces;
using TestHarness.Factory.TradeCancelledFactory;
using TestHarness.Factory.TradeCancelledFactory.Interfaces;
using TestHarness.Factory.TradingFactory.Interfaces;
using TestHarness.Factory.TradingSpoofingFactory.Interfaces;
using TestHarness.Repository.Api.Interfaces;
using TestHarness.Repository.Interfaces;
using TestHarness.State.Interfaces;
using Utilities.Aws_IO.Interfaces;

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

        IStockExchangeStreamFactory StockExchangeStreamFactory { get; }

        IOrderFileStorageProcessFactory OrderFileStorageProcessFactory { get; }

        INetworkManagerFactory NetworkManagerFactory { get; }

        ITradingFactory TradingFactory { get; }

        ITradeOrderStreamFactory TradeOrderStreamFactory { get; }
        ITradingSpoofingProcessFactory TradingSpoofingFactory { get; }
        ITradingFileRelayProcessFactory TradingFileRelayProcessFactory { get; }
        ITradingCancelledFactory TradingCancelledOrdersFactory { get; }
        ITradingCancelled2Factory TradingCancelled2Factory { get; }

        IAwsQueueClient AwsQueueClient { get; }
        IScheduledExecutionMessageBusSerialiser ScheduledExecutionSerialiser { get; }
        INetworkConfiguration Configuration { get; }
        IEquitiesFileRelayProcessFactory EquitiesFileRelayProcessFactory { get; }
        IEquitiesFileStorageProcessFactory EquitiesFileStorageProcessFactory { get; }
        IAuroraRepository AuroraRepository { get; }
        IMarketApiRepository MarketApiRepository { get; }
    }
}