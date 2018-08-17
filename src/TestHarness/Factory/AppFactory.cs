using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
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
        public IEquityDataGenerator Build()
        {
            var logger = new NLog.LogFactory().GetLogger("TestHarnessLogger");

            var tradeStrategy = new ProbabilisticTradeStrategy(logger);
            var tradeOrderGenerator = new OrderDataGenerator(logger, tradeStrategy);
            var tradeUnsubscriberFactory = new UnsubscriberFactory<TradeOrder>();
            var tradeOrderStream = new TradeOrderStream(tradeUnsubscriberFactory);

            var equityDataStrategy = new RandomWalkStrategy();
            var nasdaqInitialiser = new NasdaqInitialiser();
            var equityDataGenerator = new EquityDataGenerator(nasdaqInitialiser, equityDataStrategy, logger);
            var exchangeUnsubscriberFactory = new UnsubscriberFactory<ExchangeFrame>();
            var exchangeStream = new StockExchangeStream(exchangeUnsubscriberFactory);

            tradeOrderGenerator.InitiateTrading(exchangeStream, tradeOrderStream);
            equityDataGenerator.InitiateWalk(exchangeStream, TimeSpan.FromSeconds(5));

            return equityDataGenerator;
        }
    }
}
