using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using NLog;
using System;
using TestHarness.Engine.EquitiesGenerator;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Strategies;
using TestHarness.Engine.OrderGenerator;
using TestHarness.Engine.OrderGenerator.Strategies;

namespace TestHarness.Factory
{
    /// <summary>
    /// Replace with a DI approach later on
    /// </summary>
    public class AppFactory : IAppFactory
    {
        public AppFactory()
        {
            Logger = new LogFactory().GetLogger("TestHarnessLogger");
        }

        public IEquityDataGenerator Build()
        {
            var tradeStrategy = new ProbabilisticTradeStrategy(Logger);
            var tradeOrderGenerator = new OrderDataGenerator(Logger, tradeStrategy);
            var tradeUnsubscriberFactory = new UnsubscriberFactory<TradeOrder>();
            var tradeOrderStream = new TradeOrderStream(tradeUnsubscriberFactory);

            var equityDataStrategy = new RandomWalkStrategy();
            var nasdaqInitialiser = new NasdaqInitialiser();
            var equityDataGenerator = new EquityDataGenerator(nasdaqInitialiser, equityDataStrategy, Logger);
            var exchangeUnsubscriberFactory = new UnsubscriberFactory<ExchangeFrame>();
            var exchangeStream = new StockExchangeStream(exchangeUnsubscriberFactory);

            tradeOrderGenerator.InitiateTrading(exchangeStream, tradeOrderStream);
            equityDataGenerator.InitiateWalk(exchangeStream, TimeSpan.FromSeconds(5));

            return equityDataGenerator;
        }

        public ILogger Logger { get;  }
    }
}
