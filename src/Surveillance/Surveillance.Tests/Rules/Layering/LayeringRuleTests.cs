using System;
using System.Collections.Generic;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Market;
using Domain.Trades.Orders;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Rules.Layering;
using Surveillance.Rule_Parameters;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Tests.Helpers;
using Surveillance.Universe;

namespace Surveillance.Tests.Rules.Layering
{
    [TestFixture]
    public class LayeringRuleTests
    {
        private ILogger _logger;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private ISystemProcessOperationContext _operationCtx;
        private ILayeringRuleParameters _parameters;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _operationCtx = A.Fake<ISystemProcessOperationContext>();
            _parameters = new LayeringRuleParameters(TimeSpan.FromMinutes(30), 0.2m, null);

            A.CallTo(() => _ruleCtx.EndEvent()).Returns(_operationCtx);
        }

        [Test]
        public void Constructor_NullParametersConsidered_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRule(null, _logger, _ruleCtx));
        }

        [Test]
        public void Constructor_NullLoggerConsidered_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRule(_parameters, null, _ruleCtx));
        }

        [Test]
        public void Constructor_NullRuleContextConsidered_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRule(_parameters, _logger, null));
        }

        [Test]
        public void EndOfUniverse_RecordUpdateAlertAndEndEvent()
        {
            var rule = new LayeringRule(_parameters, _logger, _ruleCtx);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, DateTime.UtcNow, new object());

            rule.OnNext(eschaton);

            A.CallTo(() => _ruleCtx.UpdateAlertEvent(A<int>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RunRule_RaisesAlertInEschaton_WhenBidirectionalTrade()
        {
            var parameters = new LayeringRuleParameters(TimeSpan.FromMinutes(30), null, null);
            var rule = new LayeringRule(parameters, _logger, _ruleCtx);
            var tradeBuy = ((TradeOrderFrame)null).Random();
            var tradeSell = ((TradeOrderFrame)null).Random();
            tradeBuy.Position = OrderPosition.Buy;
            tradeBuy.OrderStatus = OrderStatus.Fulfilled;
            tradeSell.Position = OrderPosition.Sell;
            tradeSell.OrderStatus = OrderStatus.Fulfilled;

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.TradeSubmittedOn.AddMinutes(-1), new object());
            var buyEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeBuy.TradeSubmittedOn, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeSell.TradeSubmittedOn, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.TradeSubmittedOn.AddMinutes(1), new object());

            rule.OnNext(genesis);
            rule.OnNext(buyEvent);
            rule.OnNext(sellEvent);
            rule.OnNext(eschaton);

            A.CallTo(() => _ruleCtx.UpdateAlertEvent(1)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RunRule_NoRaisedAlertInEschaton_WhenBidirectionalTradeAndExceedsDailyThreshold_ButNoMarketData()
        {
            
            var rule = new LayeringRule(_parameters, _logger, _ruleCtx);
            var tradeBuy = ((TradeOrderFrame)null).Random();
            var tradeSell = ((TradeOrderFrame)null).Random();
            tradeBuy.Position = OrderPosition.Buy;
            tradeBuy.OrderStatus = OrderStatus.Fulfilled;
            tradeSell.Position = OrderPosition.Sell;
            tradeSell.OrderStatus = OrderStatus.Fulfilled;

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.TradeSubmittedOn.AddMinutes(-1), new object());
            var buyEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeBuy.TradeSubmittedOn, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeSell.TradeSubmittedOn, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.TradeSubmittedOn.AddMinutes(1), new object());

            rule.OnNext(genesis);
            rule.OnNext(buyEvent);
            rule.OnNext(sellEvent);
            rule.OnNext(eschaton);

            A.CallTo(() => _ruleCtx.UpdateAlertEvent(0)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RunRule_DoesNotRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesNotExceedsDailyThreshold_ButNoMarketData()
        {
            var rule = new LayeringRule(_parameters, _logger, _ruleCtx);
            var tradeBuy = ((TradeOrderFrame)null).Random();
            var tradeSell = ((TradeOrderFrame)null).Random();
            tradeBuy.Position = OrderPosition.Buy;
            tradeBuy.OrderStatus = OrderStatus.Fulfilled;
            tradeSell.Position = OrderPosition.Sell;
            tradeSell.OrderStatus = OrderStatus.Fulfilled;

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.TradeSubmittedOn.AddMinutes(-1), new object());
            var buyEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeBuy.TradeSubmittedOn, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeSell.TradeSubmittedOn, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.TradeSubmittedOn.AddMinutes(1), new object());

            rule.OnNext(genesis);
            rule.OnNext(buyEvent);
            rule.OnNext(sellEvent);
            rule.OnNext(eschaton);

            A.CallTo(() => _ruleCtx.UpdateAlertEvent(0)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RunRule_DoesRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesExceedsDailyThreshold_AndHasMarketData()
        {
            var rule = new LayeringRule(_parameters, _logger, _ruleCtx);
            var tradeBuy = ((TradeOrderFrame)null).Random();
            var tradeSell = ((TradeOrderFrame)null).Random();
            tradeBuy.Position = OrderPosition.Buy;
            tradeBuy.OrderStatus = OrderStatus.Fulfilled;
            tradeSell.Position = OrderPosition.Sell;
            tradeSell.OrderStatus = OrderStatus.Fulfilled;

            tradeBuy.FulfilledVolume = 987;
            tradeSell.FulfilledVolume = 1019;
            var market = new StockExchange(new Market.MarketId("XLON"), "London Stock Exchange");
            
            var marketData = new ExchangeFrame(market, tradeBuy.TradeSubmittedOn.AddSeconds(-55),
                new List<SecurityTick>
                {
                    new SecurityTick(tradeBuy.Security,
                        new Spread(tradeBuy.ExecutedPrice.Value, tradeSell.ExecutedPrice.Value,
                            tradeSell.ExecutedPrice.Value), new Volume(2000), new Volume(2000),
                        tradeBuy.TradeSubmittedOn.AddSeconds(-55), 100000,
                        new IntradayPrices(tradeBuy.ExecutedPrice.Value, tradeBuy.ExecutedPrice.Value,
                            tradeBuy.ExecutedPrice.Value, tradeBuy.ExecutedPrice.Value), 5000, market)
                });
            
            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.TradeSubmittedOn.AddMinutes(-1), new object());
            var marketDataEvent = new UniverseEvent(UniverseStateEvent.StockTickReddeer, tradeBuy.TradeSubmittedOn.AddSeconds(-55), marketData);
            var buyEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeBuy.TradeSubmittedOn, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeSell.TradeSubmittedOn, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.TradeSubmittedOn.AddMinutes(1), new object());

            rule.OnNext(genesis);
            rule.OnNext(marketDataEvent);
            rule.OnNext(buyEvent);
            rule.OnNext(sellEvent);
            rule.OnNext(eschaton);

            A.CallTo(() => _ruleCtx.UpdateAlertEvent(1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustNotHaveHappened();
        }

        [Test]
        public void RunRule_DoesNotRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesNotExceedsDailyThreshold_AndHasMarketData()
        {
            var rule = new LayeringRule(_parameters, _logger, _ruleCtx);
            var tradeBuy = ((TradeOrderFrame)null).Random();
            var tradeSell = ((TradeOrderFrame)null).Random();
            tradeBuy.Position = OrderPosition.Buy;
            tradeBuy.OrderStatus = OrderStatus.Fulfilled;
            tradeSell.Position = OrderPosition.Sell;
            tradeSell.OrderStatus = OrderStatus.Fulfilled;

            tradeBuy.FulfilledVolume = 100;
            tradeSell.FulfilledVolume = 100;
            var market = new StockExchange(new Market.MarketId("XLON"), "London Stock Exchange");

            var marketData = new ExchangeFrame(market, tradeBuy.TradeSubmittedOn.AddSeconds(-55),
                new List<SecurityTick>
                {
                    new SecurityTick(tradeBuy.Security,
                        new Spread(tradeBuy.ExecutedPrice.Value, tradeSell.ExecutedPrice.Value,
                            tradeSell.ExecutedPrice.Value), new Volume(2000), new Volume(2000),
                        tradeBuy.TradeSubmittedOn.AddSeconds(-55), 100000,
                        new IntradayPrices(tradeBuy.ExecutedPrice.Value, tradeBuy.ExecutedPrice.Value,
                            tradeBuy.ExecutedPrice.Value, tradeBuy.ExecutedPrice.Value), 5000, market)
                });

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.TradeSubmittedOn.AddMinutes(-1), new object());
            var marketDataEvent = new UniverseEvent(UniverseStateEvent.StockTickReddeer, tradeBuy.TradeSubmittedOn.AddSeconds(-55), marketData);
            var buyEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeBuy.TradeSubmittedOn, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeSell.TradeSubmittedOn, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.TradeSubmittedOn.AddMinutes(1), new object());

            rule.OnNext(genesis);
            rule.OnNext(marketDataEvent);
            rule.OnNext(buyEvent);
            rule.OnNext(sellEvent);
            rule.OnNext(eschaton);

            A.CallTo(() => _ruleCtx.UpdateAlertEvent(0)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustNotHaveHappened();
        }

        [Test]
        public void RunRule_DoesRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesExceedsWindowThreshold_AndHasMarketData()
        {
            var parameters = new LayeringRuleParameters(TimeSpan.FromMinutes(30), null, 0.1m);
            var rule = new LayeringRule(parameters, _logger, _ruleCtx);
            var tradeBuy = ((TradeOrderFrame)null).Random();
            var tradeSell = ((TradeOrderFrame)null).Random();
            tradeBuy.Position = OrderPosition.Buy;
            tradeBuy.OrderStatus = OrderStatus.Fulfilled;
            tradeSell.Position = OrderPosition.Sell;
            tradeSell.OrderStatus = OrderStatus.Fulfilled;

            tradeBuy.FulfilledVolume = 300;
            tradeSell.FulfilledVolume = 5;
            var market = new StockExchange(new Market.MarketId("XLON"), "London Stock Exchange");

            var marketData = new ExchangeFrame(market, tradeBuy.TradeSubmittedOn.AddSeconds(-55),
                new List<SecurityTick>
                {
                    new SecurityTick(tradeBuy.Security,
                        new Spread(tradeBuy.ExecutedPrice.Value, tradeSell.ExecutedPrice.Value,
                            tradeSell.ExecutedPrice.Value), new Volume(2000), new Volume(2000),
                        tradeBuy.TradeSubmittedOn.AddSeconds(-55), 100000,
                        new IntradayPrices(tradeBuy.ExecutedPrice.Value, tradeBuy.ExecutedPrice.Value,
                            tradeBuy.ExecutedPrice.Value, tradeBuy.ExecutedPrice.Value), 5000, market)
                });

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.TradeSubmittedOn.AddMinutes(-1), new object());
            var marketDataEvent = new UniverseEvent(UniverseStateEvent.StockTickReddeer, tradeBuy.TradeSubmittedOn.AddSeconds(-55), marketData);
            var buyEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeBuy.TradeSubmittedOn, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeSell.TradeSubmittedOn, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.TradeSubmittedOn.AddMinutes(1), new object());

            rule.OnNext(genesis);
            rule.OnNext(marketDataEvent);
            rule.OnNext(buyEvent);
            rule.OnNext(sellEvent);
            rule.OnNext(eschaton);

            A.CallTo(() => _ruleCtx.UpdateAlertEvent(1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustNotHaveHappened();
        }

        [Test]
        public void RunRule_DoesNotRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesNotExceedsWindowThreshold_AndHasMarketData()
        {
            var parameters = new LayeringRuleParameters(TimeSpan.FromMinutes(30), null, 0.1m);
            var rule = new LayeringRule(parameters, _logger, _ruleCtx);
            var tradeBuy = ((TradeOrderFrame)null).Random();
            var tradeSell = ((TradeOrderFrame)null).Random();
            tradeBuy.Position = OrderPosition.Buy;
            tradeBuy.OrderStatus = OrderStatus.Fulfilled;
            tradeSell.Position = OrderPosition.Sell;
            tradeSell.OrderStatus = OrderStatus.Fulfilled;

            tradeBuy.FulfilledVolume = 100;
            tradeSell.FulfilledVolume = 100;
            var market = new StockExchange(new Market.MarketId("XLON"), "London Stock Exchange");

            var marketData = new ExchangeFrame(market, tradeBuy.TradeSubmittedOn.AddSeconds(-55),
                new List<SecurityTick>
                {
                    new SecurityTick(tradeBuy.Security,
                        new Spread(tradeBuy.ExecutedPrice.Value, tradeSell.ExecutedPrice.Value,
                            tradeSell.ExecutedPrice.Value), new Volume(2000), new Volume(2000),
                        tradeBuy.TradeSubmittedOn.AddSeconds(-55), 100000,
                        new IntradayPrices(tradeBuy.ExecutedPrice.Value, tradeBuy.ExecutedPrice.Value,
                            tradeBuy.ExecutedPrice.Value, tradeBuy.ExecutedPrice.Value), 5000, market)
                });

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.TradeSubmittedOn.AddMinutes(-1), new object());
            var marketDataEvent = new UniverseEvent(UniverseStateEvent.StockTickReddeer, tradeBuy.TradeSubmittedOn.AddSeconds(-55), marketData);
            var buyEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeBuy.TradeSubmittedOn, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeSell.TradeSubmittedOn, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.TradeSubmittedOn.AddMinutes(1), new object());

            rule.OnNext(genesis);
            rule.OnNext(marketDataEvent);
            rule.OnNext(buyEvent);
            rule.OnNext(sellEvent);
            rule.OnNext(eschaton);

            A.CallTo(() => _ruleCtx.UpdateAlertEvent(0)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustNotHaveHappened();
        }

        [Test]
        public void RunRule_DoesNotRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesNotExceedsWindowThreshold_AndNoMarketData()
        {
            var parameters = new LayeringRuleParameters(TimeSpan.FromMinutes(30), null, 0.1m);
            var rule = new LayeringRule(parameters, _logger, _ruleCtx);
            var tradeBuy = ((TradeOrderFrame)null).Random();
            var tradeSell = ((TradeOrderFrame)null).Random();
            tradeBuy.Position = OrderPosition.Buy;
            tradeBuy.OrderStatus = OrderStatus.Fulfilled;
            tradeSell.Position = OrderPosition.Sell;
            tradeSell.OrderStatus = OrderStatus.Fulfilled;

            tradeBuy.FulfilledVolume = 100;
            tradeSell.FulfilledVolume = 100;
            var market = new StockExchange(new Market.MarketId("XLON"), "London Stock Exchange");

            var marketData = new ExchangeFrame(market, tradeBuy.TradeSubmittedOn.AddSeconds(-55),
                new List<SecurityTick>
                {
                    new SecurityTick(tradeBuy.Security,
                        new Spread(tradeBuy.ExecutedPrice.Value, tradeSell.ExecutedPrice.Value,
                            tradeSell.ExecutedPrice.Value), new Volume(2000), new Volume(2000),
                        tradeBuy.TradeSubmittedOn.AddSeconds(-55), 100000,
                        new IntradayPrices(tradeBuy.ExecutedPrice.Value, tradeBuy.ExecutedPrice.Value,
                            tradeBuy.ExecutedPrice.Value, tradeBuy.ExecutedPrice.Value), 5000, market)
                });

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.TradeSubmittedOn.AddMinutes(-1), new object());
            var marketDataEvent = new UniverseEvent(UniverseStateEvent.StockTickReddeer, tradeBuy.TradeSubmittedOn.AddSeconds(-55), marketData);
            var buyEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeBuy.TradeSubmittedOn, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeSell.TradeSubmittedOn, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.TradeSubmittedOn.AddMinutes(1), new object());

            rule.OnNext(genesis);
            rule.OnNext(buyEvent);
            rule.OnNext(sellEvent);
            rule.OnNext(marketDataEvent);
            rule.OnNext(eschaton);

            A.CallTo(() => _ruleCtx.UpdateAlertEvent(0)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustHaveHappenedOnceExactly();
        }
    }
}
