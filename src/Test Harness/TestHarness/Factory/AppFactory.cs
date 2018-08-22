using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using NLog;
using System;
using TestHarness.Commands;
using TestHarness.Commands.Interfaces;
using TestHarness.Display;
using TestHarness.Display.Subscribers;
using TestHarness.Engine.EquitiesGenerator;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Strategies;
using TestHarness.Engine.Heartbeat;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Strategies;
using TestHarness.Factory.EquitiesFactory;
using TestHarness.Factory.EquitiesFactory.Interfaces;
using TestHarness.Factory.Interfaces;
using TestHarness.Factory.NetworkFactory;
using TestHarness.Factory.NetworkFactory.Interfaces;
using TestHarness.Factory.TradingFactory;
using TestHarness.Factory.TradingFactory.Interfaces;
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
            Console = new Display.Console();
            CommandManifest = new CommandManifest();
            CommandManager = new CommandManager(State, Logger, Console);

            EquitiesProcessFactory = new EquitiesProcessFactory(Logger);
            StockExchangeStreamFactory = new StockExchangeStreamFactory();
            NetworkManagerFactory = new NetworkManagerFactory(Logger);
            TradingFactory = new TradingFactory.TradingFactory(Logger);
            TradeOrderStreamFactory = new TradeOrderStreamFactory();

            var tradingStrat = 
                TradingFactory
                .Create()
                .Heartbeat()
                .Regular(TimeSpan.FromMilliseconds(100))
                .TradingFixedVolume(10)
                .Finish();
        }

        public void Build()
        {
            // if normal distri
            // var tradeVolumeStrategy = new TradeVolumeNormalDistributionStrategy(8);

            // if fixed
            var tradeVolumeStrategy = new TradeVolumeFixedStrategy(1);

            // if heartbeat
            var irregularHeartbeat = new IrregularHeartbeat(TimeSpan.FromMilliseconds(300), 10);

            var tradeStrategy = new MarkovTradeStrategy(Logger, tradeVolumeStrategy);
            var tradeOrderGenerator = new TradingHeatbeatDrivenProcess(Logger, tradeStrategy, irregularHeartbeat);
            var tradeUnsubscriberFactory = new UnsubscriberFactory<TradeOrderFrame>();
            var tradeOrderStream = new TradeOrderStream(tradeUnsubscriberFactory);
            var tradeOrderDisplaySubscriber = new TradeOrderFrameDisplaySubscriber(Console);
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
            var exchangeStreamDisplaySubscriber = new ExchangeFrameDisplaySubscriber(Console);
            exchangeStream.Subscribe(exchangeStreamDisplaySubscriber);



            // there is our problem with it initially walking
            tradeOrderGenerator.InitiateTrading(exchangeStream, tradeOrderStream);
            var heartBeat = new Heartbeat(TimeSpan.FromMilliseconds(1500));
            equityDataGenerator.InitiateWalk(exchangeStream, heartBeat);
            irregularHeartbeat.Start();
            heartBeat.Start();
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
        public ICommandManifest CommandManifest { get; private set; }

        /// <summary>
        /// Ctor is used to construct this
        /// </summary>
        public IProgramState State { get; private set; }

        public IConsole Console { get; private set; }



        // new factories
        public IEquitiesProcessFactory EquitiesProcessFactory { get; private set; } 

        public IStockExchangeStreamFactory StockExchangeStreamFactory { get; private set; }

        public INetworkManagerFactory NetworkManagerFactory { get; private set; } 

        public ITradingFactory TradingFactory { get; private set; }

        public ITradeOrderStreamFactory TradeOrderStreamFactory { get; private set; }
    }
}
