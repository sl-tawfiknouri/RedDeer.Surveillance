﻿using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Orders;
using NLog;
using System;
using TestHarness.Display.Subscribers;
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
            var display = new Display.Console();

            var tradeStrategy = new ProbabilisticTradeStrategy(Logger);
            var tradeOrderGenerator = new TradingMarkovProcess(Logger, tradeStrategy);
            var tradeUnsubscriberFactory = new UnsubscriberFactory<TradeOrderFrame>();
            var tradeOrderStream = new TradeOrderStream(tradeUnsubscriberFactory);
            var tradeOrderDisplaySubscriber = new TradeOrderFrameDisplaySubscriber(display);
            tradeOrderStream.Subscribe(tradeOrderDisplaySubscriber);

            var equityDataStrategy = new RandomWalkStrategy();
            var nasdaqInitialiser = new NasdaqInitialiser();
            var equityDataGenerator = new EquitiesMarkovProcess(nasdaqInitialiser, equityDataStrategy, Logger);
            var exchangeUnsubscriberFactory = new UnsubscriberFactory<ExchangeFrame>();
            var exchangeStream = new StockExchangeStream(exchangeUnsubscriberFactory);
            var exchangeStreamDisplaySubscriber = new ExchangeFrameDisplaySubscriber(display);
            exchangeStream.Subscribe(exchangeStreamDisplaySubscriber);

            tradeOrderGenerator.InitiateTrading(exchangeStream, tradeOrderStream);
            equityDataGenerator.InitiateWalk(exchangeStream, TimeSpan.FromSeconds(1));

            return equityDataGenerator;
        }

        public ILogger Logger { get;  }
    }
}
