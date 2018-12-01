using System;
using System.Collections.Generic;
using Domain.Equity;
using Domain.Equity.Frames;
using Domain.Finance;
using Domain.Market;
using Domain.Trades.Orders;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.RuleParameters;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.Layering;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Tests.Helpers;
using Surveillance.Universe;

namespace Surveillance.Tests.Rules.Layering
{
    [TestFixture]
    public class LayeringRuleTests
    {
        private ILogger _logger;
        private IUniverseAlertStream _alertStream;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private ISystemProcessOperationContext _operationCtx;
        private ILayeringRuleParameters _parameters;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger>();
            _alertStream = A.Fake<IUniverseAlertStream>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _operationCtx = A.Fake<ISystemProcessOperationContext>();
            _parameters = new LayeringRuleParameters(TimeSpan.FromMinutes(30), 0.2m, null, null, null, false);

            A.CallTo(() => _ruleCtx.EndEvent()).Returns(_operationCtx);
        }

        [Test]
        public void Constructor_NullParametersConsidered_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRule(null, _alertStream, _logger, _ruleCtx));
        }

        [Test]
        public void Constructor_NullLoggerConsidered_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRule(_parameters, _alertStream, null, _ruleCtx));
        }

        [Test]
        public void Constructor_NullRuleContextConsidered_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRule(_parameters, _alertStream, _logger, null));
        }

        [Test]
        public void EndOfUniverse_RecordUpdateAlertAndEndEvent()
        {
            var rule = new LayeringRule(_parameters, _alertStream, _logger, _ruleCtx);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, DateTime.UtcNow, new object());

            rule.OnNext(eschaton);

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RunRule_RaisesAlertInEschaton_WhenBidirectionalTrade()
        {
            var parameters = new LayeringRuleParameters(TimeSpan.FromMinutes(30), null, null, null, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _logger, _ruleCtx);
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

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedTwiceExactly();
        }

        [Test]
        public void RunRule_NoRaisedAlertInEschaton_WhenBidirectionalTradeAndExceedsDailyThreshold_ButNoMarketData()
        {
            
            var rule = new LayeringRule(_parameters, _alertStream, _logger, _ruleCtx);
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

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RunRule_DoesNotRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesNotExceedsDailyThreshold_ButNoMarketData()
        {
            var rule = new LayeringRule(_parameters, _alertStream, _logger, _ruleCtx);
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

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RunRule_DoesRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesExceedsDailyThreshold_AndHasMarketData()
        {
            var rule = new LayeringRule(_parameters, _alertStream, _logger, _ruleCtx);
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

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustNotHaveHappened();
        }

        [Test]
        public void RunRule_DoesNotRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesNotExceedsDailyThreshold_AndHasMarketData()
        {
            var rule = new LayeringRule(_parameters, _alertStream, _logger, _ruleCtx);
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

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustNotHaveHappened();
        }

        [Test]
        public void RunRule_DoesRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesExceedsWindowThreshold_AndHasMarketData()
        {
            var parameters = new LayeringRuleParameters(TimeSpan.FromMinutes(30), null, 0.1m, null, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _logger, _ruleCtx);
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

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustNotHaveHappened();
        }

        [Test]
        public void RunRule_DoesNotRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesNotExceedsWindowThreshold_AndHasMarketData()
        {
            var parameters = new LayeringRuleParameters(TimeSpan.FromMinutes(30), null, 0.1m, null, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _logger, _ruleCtx);
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

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustNotHaveHappened();
        }

        [Test]
        public void RunRule_DoesNotRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesNotExceedsWindowThreshold_AndNoMarketData()
        {
            var parameters = new LayeringRuleParameters(TimeSpan.FromMinutes(30), null, 0.1m, null, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _logger, _ruleCtx);
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

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RunRule_DoesNotRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesNotCausePriceMovement_AndHasMarketData()
        {
            var parameters = new LayeringRuleParameters(TimeSpan.FromMinutes(30), null, null, true, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _logger, _ruleCtx);
            var tradeBuy = ((TradeOrderFrame)null).Random();
            var tradeSell = ((TradeOrderFrame)null).Random();
            tradeBuy.Position = OrderPosition.Buy;
            tradeBuy.OrderStatus = OrderStatus.Fulfilled;
            tradeSell.Position = OrderPosition.Sell;
            tradeSell.OrderStatus = OrderStatus.Fulfilled;

            tradeBuy.FulfilledVolume = 300;
            tradeSell.FulfilledVolume = 5;

            tradeBuy.TradeSubmittedOn = new DateTime(2018, 10, 14, 10, 30, 0);
            tradeSell.TradeSubmittedOn = tradeBuy.TradeSubmittedOn.AddSeconds(30);

            var market = new StockExchange(new Market.MarketId("XLON"), "London Stock Exchange");

            var initialPrice = tradeBuy.ExecutedPrice.Value.Value;
            var marketData1 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice, tradeBuy.TradeSubmittedOn.AddSeconds(-55));
            var marketData2 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.95m, tradeBuy.TradeSubmittedOn.AddSeconds(-50));

            var marketData3 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice* 0.9m, tradeBuy.TradeSubmittedOn.AddSeconds(5));
            var marketData4 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.85m, tradeBuy.TradeSubmittedOn.AddSeconds(10));

            var marketData5 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice* 0.8m, tradeSell.TradeSubmittedOn.AddSeconds(5));
            var marketData6 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.75m, tradeSell.TradeSubmittedOn.AddSeconds(10));


            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.TradeSubmittedOn.AddMinutes(-1), new object());
            var marketDataEvent1 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData1.TimeStamp,  marketData1);
            var marketDataEvent2 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData2.TimeStamp, marketData2);

            var buyEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeBuy.TradeSubmittedOn, tradeBuy);

            var marketDataEvent3 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData3.TimeStamp, marketData3);
            var marketDataEvent4 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData4.TimeStamp, marketData4);

            var sellEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeSell.TradeSubmittedOn, tradeSell);

            var marketDataEvent5 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData5.TimeStamp, marketData5);
            var marketDataEvent6 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData6.TimeStamp, marketData6);

            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.TradeSubmittedOn.AddMinutes(1), new object());

            rule.OnNext(genesis);
            rule.OnNext(marketDataEvent1);
            rule.OnNext(marketDataEvent2);
            rule.OnNext(buyEvent);
            rule.OnNext(marketDataEvent3);
            rule.OnNext(marketDataEvent4);
            rule.OnNext(sellEvent);
            rule.OnNext(marketDataEvent5);
            rule.OnNext(marketDataEvent6);
            rule.OnNext(eschaton);

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustNotHaveHappened();
        }

        [Test]
        public void RunRule_DoesRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesCausePriceMovement_AndHasMarketData()
        {
            var parameters = new LayeringRuleParameters(TimeSpan.FromMinutes(30), null, null, true, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _logger, _ruleCtx);
            var tradeBuy = ((TradeOrderFrame)null).Random();
            var tradeSell = ((TradeOrderFrame)null).Random();
            tradeBuy.Position = OrderPosition.Buy;
            tradeBuy.OrderStatus = OrderStatus.Fulfilled;
            tradeSell.Position = OrderPosition.Sell;
            tradeSell.OrderStatus = OrderStatus.Fulfilled;

            tradeBuy.FulfilledVolume = 300;
            tradeSell.FulfilledVolume = 5;

            tradeBuy.TradeSubmittedOn = new DateTime(2018, 10, 14, 10, 30, 0);
            tradeSell.TradeSubmittedOn = tradeBuy.TradeSubmittedOn.AddSeconds(30);

            var market = new StockExchange(new Market.MarketId("XLON"), "London Stock Exchange");

            var initialPrice = tradeBuy.ExecutedPrice.Value.Value;
            var marketData1 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice, tradeBuy.TradeSubmittedOn.AddSeconds(-55));
            var marketData2 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 1.05m, tradeBuy.TradeSubmittedOn.AddSeconds(-50));

            var marketData3 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 1.1m, tradeBuy.TradeSubmittedOn.AddSeconds(5));
            var marketData4 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 1.15m, tradeBuy.TradeSubmittedOn.AddSeconds(10));

            var marketData5 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 1.2m, tradeSell.TradeSubmittedOn.AddSeconds(5));
            var marketData6 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 1.25m, tradeSell.TradeSubmittedOn.AddSeconds(10));


            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.TradeSubmittedOn.AddMinutes(-1), new object());
            var marketDataEvent1 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData1.TimeStamp, marketData1);
            var marketDataEvent2 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData2.TimeStamp, marketData2);

            var buyEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeBuy.TradeSubmittedOn, tradeBuy);

            var marketDataEvent3 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData3.TimeStamp, marketData3);
            var marketDataEvent4 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData4.TimeStamp, marketData4);

            var sellEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeSell.TradeSubmittedOn, tradeSell);

            var marketDataEvent5 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData5.TimeStamp, marketData5);
            var marketDataEvent6 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData6.TimeStamp, marketData6);

            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.TradeSubmittedOn.AddMinutes(1), new object());

            rule.OnNext(genesis);
            rule.OnNext(marketDataEvent1);
            rule.OnNext(marketDataEvent2);
            rule.OnNext(buyEvent);
            rule.OnNext(marketDataEvent3);
            rule.OnNext(marketDataEvent4);
            rule.OnNext(sellEvent);
            rule.OnNext(marketDataEvent5);
            rule.OnNext(marketDataEvent6);
            rule.OnNext(eschaton);

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustNotHaveHappened();
        }

        [Test]
        public void RunRule_DoesRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesCausePriceMovement_AndHasMarketDataWithReverseBuySellOrder()
        {
            var parameters = new LayeringRuleParameters(TimeSpan.FromMinutes(30), null, null, true, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _logger, _ruleCtx);
            var tradeBuy = ((TradeOrderFrame)null).Random();
            var tradeSell = ((TradeOrderFrame)null).Random();
            tradeBuy.Position = OrderPosition.Sell;
            tradeBuy.OrderStatus = OrderStatus.Fulfilled;
            tradeSell.Position = OrderPosition.Buy;
            tradeSell.OrderStatus = OrderStatus.Fulfilled;

            tradeBuy.FulfilledVolume = 300;
            tradeSell.FulfilledVolume = 5;

            tradeBuy.TradeSubmittedOn = new DateTime(2018, 10, 14, 10, 30, 0);
            tradeSell.TradeSubmittedOn = tradeBuy.TradeSubmittedOn.AddSeconds(30);

            var market = new StockExchange(new Market.MarketId("XLON"), "London Stock Exchange");

            var initialPrice = tradeBuy.ExecutedPrice.Value.Value;
            var marketData1 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice, tradeBuy.TradeSubmittedOn.AddSeconds(-55));
            var marketData2 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.95m, tradeBuy.TradeSubmittedOn.AddSeconds(-50));

            var marketData3 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.9m, tradeBuy.TradeSubmittedOn.AddSeconds(5));
            var marketData4 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.85m, tradeBuy.TradeSubmittedOn.AddSeconds(10));

            var marketData5 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.8m, tradeSell.TradeSubmittedOn.AddSeconds(5));
            var marketData6 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.75m, tradeSell.TradeSubmittedOn.AddSeconds(10));


            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.TradeSubmittedOn.AddMinutes(-1), new object());
            var marketDataEvent1 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData1.TimeStamp, marketData1);
            var marketDataEvent2 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData2.TimeStamp, marketData2);

            var buyEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeBuy.TradeSubmittedOn, tradeBuy);

            var marketDataEvent3 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData3.TimeStamp, marketData3);
            var marketDataEvent4 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData4.TimeStamp, marketData4);

            var sellEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeSell.TradeSubmittedOn, tradeSell);

            var marketDataEvent5 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData5.TimeStamp, marketData5);
            var marketDataEvent6 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData6.TimeStamp, marketData6);

            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.TradeSubmittedOn.AddMinutes(1), new object());

            rule.OnNext(genesis);
            rule.OnNext(marketDataEvent1);
            rule.OnNext(marketDataEvent2);
            rule.OnNext(buyEvent);
            rule.OnNext(marketDataEvent3);
            rule.OnNext(marketDataEvent4);
            rule.OnNext(sellEvent);
            rule.OnNext(marketDataEvent5);
            rule.OnNext(marketDataEvent6);
            rule.OnNext(eschaton);

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustNotHaveHappened();
        }

        [Test]
        public void RunRule_DoesNotRaiseAlertInEschaton_WhenBidirectionalTradeAndNoPriceMovementData()
        {
            var parameters = new LayeringRuleParameters(TimeSpan.FromMinutes(30), null, null, true, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _logger, _ruleCtx);
            var tradeBuy = ((TradeOrderFrame)null).Random();
            var tradeSell = ((TradeOrderFrame)null).Random();
            tradeBuy.Position = OrderPosition.Buy;
            tradeBuy.OrderStatus = OrderStatus.Fulfilled;
            tradeSell.Position = OrderPosition.Sell;
            tradeSell.OrderStatus = OrderStatus.Fulfilled;

            tradeBuy.FulfilledVolume = 300;
            tradeSell.FulfilledVolume = 5;

            tradeBuy.TradeSubmittedOn = new DateTime(2018, 10, 14, 10, 30, 0);
            tradeSell.TradeSubmittedOn = tradeBuy.TradeSubmittedOn.AddSeconds(30);

            var market = new StockExchange(new Market.MarketId("XLON"), "London Stock Exchange");
            var initialPrice = tradeBuy.ExecutedPrice.Value.Value;
            var marketData5 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 1.2m, tradeSell.TradeSubmittedOn.AddSeconds(5));
            var marketData6 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 1.25m, tradeSell.TradeSubmittedOn.AddSeconds(10));


            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.TradeSubmittedOn.AddMinutes(-1), new object());
            var buyEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeBuy.TradeSubmittedOn, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.TradeReddeerSubmitted, tradeSell.TradeSubmittedOn, tradeSell);

            var marketDataEvent5 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData5.TimeStamp, marketData5);
            var marketDataEvent6 = new UniverseEvent(UniverseStateEvent.StockTickReddeer, marketData6.TimeStamp, marketData6);

            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.TradeSubmittedOn.AddMinutes(1), new object());

            rule.OnNext(genesis);
            rule.OnNext(buyEvent);
            rule.OnNext(sellEvent);
            rule.OnNext(marketDataEvent5);
            rule.OnNext(marketDataEvent6);
            rule.OnNext(eschaton);

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _operationCtx.EndEventWithMissingDataError()).MustHaveHappenedOnceExactly();
        }

        private ExchangeFrame SetExchangeFrameToPrice(
            StockExchange market,
            TradeOrderFrame baseBuyFrame,
            TradeOrderFrame baseSellFrame,
            decimal price,
            DateTime timestamp)
        {
            return new ExchangeFrame(market, timestamp,
                new List<SecurityTick>
                {
                    new SecurityTick(baseBuyFrame.Security,
                        new Spread(baseBuyFrame.ExecutedPrice.Value, baseSellFrame.ExecutedPrice.Value,
                            new CurrencyAmount(price, baseSellFrame.OrderCurrency)), new Volume(2000), new Volume(2000),
                        timestamp, 100000,
                        new IntradayPrices(baseBuyFrame.ExecutedPrice.Value, baseBuyFrame.ExecutedPrice.Value,
                            baseBuyFrame.ExecutedPrice.Value, baseBuyFrame.ExecutedPrice.Value), 5000, market)
                });
        }

    }
}
