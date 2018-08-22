using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using NLog;
using System;
using TestHarness.Commands;
using TestHarness.Commands.Interfaces;
using TestHarness.Display.Subscribers;
using TestHarness.Engine.EquitiesGenerator;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Strategies;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Strategies;
using TestHarness.Interfaces;
using TestHarness.Network_IO;
using TestHarness.Network_IO.Subscribers;
using Utilities.Network_IO.Websocket_Connections;

namespace TestHarness.Factory
{
    /// <summary>
    /// Replace with a DI approach later on | this is a bit messy
    /// </summary>
    public class AppFactory : IAppFactory
    {
        public AppFactory()
        {
            Logger = new LogFactory().GetLogger("TestHarnessLogger");
            State = new ProgramState();
            CommandManager = new CommandManager(State, Logger);
        }

        public void Build()
        {
            var display = new Display.Console();

            var tradeStrategy = new MarkovTradeStrategy(Logger);
            var tradeOrderGenerator = new TradingMarkovProcess(Logger, tradeStrategy);
            var tradeUnsubscriberFactory = new UnsubscriberFactory<TradeOrderFrame>();
            var tradeOrderStream = new TradeOrderStream(tradeUnsubscriberFactory);
            var tradeOrderDisplaySubscriber = new TradeOrderFrameDisplaySubscriber(display);
            tradeOrderStream.Subscribe(tradeOrderDisplaySubscriber);

            var websocketFactory = new WebsocketConnectionFactory();
            var configuration = new Configuration.Configuration("localhost", "9067");
            var tradeOrderSubscriberFactory = new TradeOrderWebsocketSubscriberFactory(websocketFactory, Logger);

            // if networking
            //NetworkManager = new NetworkManager(tradeOrderSubscriberFactory, configuration, Logger);
            //NetworkManager.InitiateNetworkConnections();
            //NetworkManager.AttachTradeOrderSubscriberToStream(tradeOrderStream);

            // if stubbing out networking (default mode)
            NetworkManager = new StubNetworkManager(Logger);

            var equityDataStrategy = new MarkovEquityStrategy();
            var nasdaqInitialiser = new NasdaqInitialiser();
            var equityDataGenerator = new EquitiesMarkovProcess(nasdaqInitialiser, equityDataStrategy, Logger);
            var exchangeUnsubscriberFactory = new UnsubscriberFactory<ExchangeFrame>();
            var exchangeStream = new StockExchangeStream(exchangeUnsubscriberFactory);
            var exchangeStreamDisplaySubscriber = new ExchangeFrameDisplaySubscriber(display);
            exchangeStream.Subscribe(exchangeStreamDisplaySubscriber);

            tradeOrderGenerator.InitiateTrading(exchangeStream, tradeOrderStream);
            equityDataGenerator.InitiateWalk(exchangeStream, TimeSpan.FromSeconds(2));

            EquityDataGenerator = equityDataGenerator;
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
        public IProgramState State { get; private set; }

    }
}
