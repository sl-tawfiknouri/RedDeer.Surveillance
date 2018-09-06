using NLog;
using TestHarness.Commands.Interfaces;
using TestHarness.Display;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.Heartbeat.Interfaces;
using TestHarness.Factory.EquitiesFactory.Interfaces;
using TestHarness.Factory.NetworkFactory.Interfaces;
using TestHarness.Factory.TradingFactory.Interfaces;
using TestHarness.Factory.TradingProhibitedSecurityFactory.Interfaces;
using TestHarness.Factory.TradingSpoofingFactory.Interfaces;
using TestHarness.Network_IO.Interfaces;
using TestHarness.State.Interfaces;

namespace TestHarness.Factory.Interfaces
{
    public interface IAppFactory
    {
        ILogger Logger { get; }

        INetworkManager NetworkManager { get; }

        ICommandManager CommandManager { get; }

        ICommandManifest CommandManifest { get; }

        IProgramState State { get; }

        IEquityDataGenerator EquityDataGenerator { get; }

        IConsole Console { get; }

        IPulsatingHeartbeat ProhibitedSecurityHeartbeat { get; }

        IPulsatingHeartbeat SpoofedTradeHeartbeat { get; }

        IEquitiesProcessFactory EquitiesProcessFactory { get; }

        IStockExchangeStreamFactory StockExchangeStreamFactory { get; }

        INetworkManagerFactory NetworkManagerFactory { get; }

        ITradingFactory TradingFactory { get; }

        ITradeOrderStreamFactory TradeOrderStreamFactory { get; }

        ITradingProhibitedSecurityProcessFactory TradingProhibitedSecurityFactory { get; }
        ITradingSpoofingProcessFactory TradingSpoofingFactory { get; }
        ITradingFileRelayProcessFactory TradingFileRelayProcessFactory { get; }
    }
}