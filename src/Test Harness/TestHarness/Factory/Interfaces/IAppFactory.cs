using NLog;
using TestHarness.Commands.Interfaces;
using TestHarness.Display;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Factory.EquitiesFactory.Interfaces;
using TestHarness.Factory.NetworkFactory.Interfaces;
using TestHarness.Factory.TradingFactory.Interfaces;
using TestHarness.Interfaces;
using TestHarness.Network_IO;

namespace TestHarness.Factory.Interfaces
{
    public interface IAppFactory
    {
        void Build();

        ILogger Logger { get; }

        INetworkManager NetworkManager { get; }

        ICommandManager CommandManager { get; }

        ICommandManifest CommandManifest { get; }

        IProgramState State { get; }

        IEquityDataGenerator EquityDataGenerator { get; }

        IConsole Console { get; }

        IEquitiesProcessFactory EquitiesProcessFactory { get; }

        IStockExchangeStreamFactory StockExchangeStreamFactory { get; }

        INetworkManagerFactory NetworkManagerFactory { get; }

        ITradingFactory TradingFactory { get; }

        ITradeOrderStreamFactory TradeOrderStreamFactory { get; }
    }
}