﻿using System;
using System.Collections.Generic;
using DomainV2.Equity;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets;
using Surveillance.Markets.Interfaces;
using Surveillance.RuleParameters;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.Layering;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Tests.Helpers;
using Surveillance.Trades;
using Surveillance.Universe;
using Surveillance.Universe.Filter.Interfaces;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Tests.Rules.Layering
{
    [TestFixture]
    public class LayeringRuleTests
    {
        private ILogger _logger;
        private IUniverseAlertStream _alertStream;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private ISystemProcessOperationContext _operationCtx;
        private IUniverseMarketCacheFactory _factory;
        private ILayeringRuleParameters _parameters;
        private IUniverseOrderFilter _orderFilter;

        private IMarketTradingHoursManager _tradingHoursManager;
        private IRuleRunDataRequestRepository _ruleRunRepository;
        private IStubRuleRunDataRequestRepository _stubRuleRunRepository;
        private ILogger<UniverseMarketCacheFactory> _factoryLogger;
        private ILogger<TradingHistoryStack> _tradingLogger;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger>();
            _tradingLogger = A.Fake<ILogger<TradingHistoryStack>>();
             _alertStream = A.Fake<IUniverseAlertStream>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _operationCtx = A.Fake<ISystemProcessOperationContext>();
            _parameters = new LayeringRuleParameters("id", TimeSpan.FromMinutes(30), 0.2m, null, null, null, false);

            _orderFilter = A.Fake<IUniverseOrderFilter>();
            A.CallTo(() => _orderFilter.Filter(A<IUniverseEvent>.Ignored)).ReturnsLazily(i => (IUniverseEvent)i.Arguments[0]);

            _ruleRunRepository = A.Fake<IRuleRunDataRequestRepository>();
            _stubRuleRunRepository = A.Fake<IStubRuleRunDataRequestRepository>();
            _factoryLogger = A.Fake<ILogger<UniverseMarketCacheFactory>>();
            _factory = new UniverseMarketCacheFactory(_stubRuleRunRepository, _ruleRunRepository, _factoryLogger);
            _tradingHoursManager = A.Fake<IMarketTradingHoursManager>();

            A.CallTo(() => _ruleCtx.EndEvent()).Returns(_operationCtx);
        }

        [Test]
        public void Constructor_NullParametersConsidered_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRule(null, _alertStream, _orderFilter, _logger, _factory, _tradingHoursManager, _ruleCtx, RuleRunMode.ValidationRun, _tradingLogger));
        }

        [Test]
        public void Constructor_NullLoggerConsidered_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRule(_parameters, _alertStream, _orderFilter, null, _factory, _tradingHoursManager, _ruleCtx, RuleRunMode.ValidationRun, _tradingLogger));
        }

        [Test]
        public void Constructor_NullRuleContextConsidered_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new LayeringRule(_parameters, _alertStream, _orderFilter, _logger, _factory, _tradingHoursManager, null, RuleRunMode.ValidationRun, _tradingLogger));
        }

        [Test]
        public void EndOfUniverse_RecordUpdateAlertAndEndEvent()
        {
            var rule = new LayeringRule(_parameters, _alertStream, _orderFilter, _logger, _factory, _tradingHoursManager, _ruleCtx, RuleRunMode.ValidationRun, _tradingLogger);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, DateTime.UtcNow, new object());

            rule.OnNext(eschaton);

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RunRule_RaisesAlertInEschaton_WhenBidirectionalTrade()
        {
            var parameters = new LayeringRuleParameters("id", TimeSpan.FromMinutes(30), null, null, null, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _orderFilter, _logger, _factory, _tradingHoursManager, _ruleCtx, RuleRunMode.ValidationRun, _tradingLogger);
            var tradeBuy = ((Order)null).Random();
            var tradeSell = ((Order)null).Random();
            tradeBuy.OrderDirection = OrderDirections.BUY;
            tradeBuy.OrderFilledDate = tradeBuy.OrderPlacedDate.Value.AddMinutes(1);
            tradeSell.OrderDirection = OrderDirections.SELL;
            tradeSell.OrderFilledDate = tradeSell.OrderPlacedDate.Value.AddMinutes(1);

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.OrderPlacedDate.Value.AddMinutes(-1), new object());
            var buyEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeBuy.OrderPlacedDate.Value, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeSell.OrderPlacedDate.Value, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.OrderPlacedDate.Value.AddMinutes(-1), new object());

            rule.OnNext(genesis);
            rule.OnNext(buyEvent);
            rule.OnNext(sellEvent);
            rule.OnNext(eschaton);

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedTwiceExactly();
        }

        [Test]
        public void RunRule_NoRaisedAlertInEschaton_WhenBidirectionalTradeAndExceedsDailyThreshold_ButNoMarketData()
        {
            
            var rule = new LayeringRule(_parameters, _alertStream, _orderFilter, _logger, _factory, _tradingHoursManager, _ruleCtx, RuleRunMode.ValidationRun, _tradingLogger);
            var tradeBuy = ((Order)null).Random();
            var tradeSell = ((Order)null).Random();
            tradeBuy.OrderDirection = OrderDirections.BUY;
            tradeBuy.OrderFilledDate = tradeBuy.OrderPlacedDate.Value.AddMinutes(1);
            tradeSell.OrderDirection = OrderDirections.SELL;
            tradeSell.OrderFilledDate = tradeSell.OrderPlacedDate.Value.AddMinutes(1);

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.OrderPlacedDate.Value.AddMinutes(-1), new object());
            var buyEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeBuy.OrderPlacedDate.Value, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeSell.OrderPlacedDate.Value, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.OrderPlacedDate.Value.AddMinutes(1), new object());

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
            var rule = new LayeringRule(_parameters, _alertStream, _orderFilter, _logger, _factory, _tradingHoursManager, _ruleCtx, RuleRunMode.ValidationRun, _tradingLogger);
            var tradeBuy = ((Order)null).Random();
            var tradeSell = ((Order)null).Random();
            tradeBuy.OrderDirection = OrderDirections.BUY;
            tradeBuy.OrderFilledDate = tradeBuy.OrderPlacedDate.Value.AddMinutes(1);
            tradeSell.OrderDirection = OrderDirections.SELL;
            tradeSell.OrderFilledDate = tradeSell.OrderPlacedDate.Value.AddMinutes(1);

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.OrderPlacedDate.Value.AddMinutes(-1), new object());
            var buyEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeBuy.OrderPlacedDate.Value, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeSell.OrderPlacedDate.Value, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.OrderPlacedDate.Value.AddMinutes(1), new object());

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
            var rule = new LayeringRule(_parameters, _alertStream, _orderFilter, _logger, _factory, _tradingHoursManager, _ruleCtx, RuleRunMode.ValidationRun, _tradingLogger);
            var tradeBuy = ((Order)null).Random();
            var tradeSell = ((Order)null).Random();
            tradeBuy.OrderPlacedDate = new DateTime(2018, 01, 01, 12, 0, 0);
            tradeBuy.OrderDirection = OrderDirections.BUY;
            tradeBuy.OrderFilledDate = tradeBuy.OrderPlacedDate.Value.AddMinutes(1);
            tradeSell.OrderPlacedDate = new DateTime(2018, 01, 01, 12, 0, 0);
            tradeSell.OrderDirection = OrderDirections.SELL;
            tradeSell.OrderFilledDate = tradeSell.OrderPlacedDate.Value.AddMinutes(1);

            A.CallTo(() => _tradingHoursManager.Get(A<string>.Ignored))
                .Returns(new TradingHours
            {
                IsValid = true,
                Mic = "XLON",
                OpenOffsetInUtc = TimeSpan.FromHours(8),
                CloseOffsetInUtc = TimeSpan.FromHours(16)
            });

            tradeBuy.OrderFilledVolume = 987;
            tradeSell.OrderFilledVolume = 1019;
            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
            
            var marketData = new EquityIntraDayTimeBarCollection(market, tradeBuy.OrderPlacedDate.Value.AddSeconds(-55),
                new List<EquityInstrumentIntraDayTimeBar>
                {
                    new EquityInstrumentIntraDayTimeBar(
                        tradeBuy.Instrument,
                        new SpreadTimeBar(
                            tradeBuy.OrderAverageFillPrice.Value,
                            tradeSell.OrderAverageFillPrice.Value,
                            tradeSell.OrderAverageFillPrice.Value,
                            new Volume(2000)),
                        new DailySummaryTimeBar(
                            1000,
                            new IntradayPrices(tradeBuy.OrderAverageFillPrice.Value, tradeBuy.OrderAverageFillPrice.Value,
                                tradeBuy.OrderAverageFillPrice.Value, tradeBuy.OrderAverageFillPrice.Value),
                            1000,
                            new Volume(2000),
                            tradeBuy.OrderPlacedDate.Value.AddSeconds(-55)
                            ),
                        tradeBuy.OrderPlacedDate.Value.AddSeconds(-55),
                    market)
                });
            
            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.OrderPlacedDate.Value.AddMinutes(-1), new object());
            var marketDataEvent = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, tradeBuy.OrderPlacedDate.Value.AddSeconds(-55), marketData);
            var buyEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeBuy.OrderPlacedDate.Value, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeSell.OrderPlacedDate.Value, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.OrderPlacedDate.Value.AddMinutes(1), new object());

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
            var rule = new LayeringRule(_parameters, _alertStream, _orderFilter, _logger, _factory, _tradingHoursManager, _ruleCtx, RuleRunMode.ValidationRun, _tradingLogger);
            var tradeBuy = ((Order)null).Random();
            var tradeSell = ((Order)null).Random();
            tradeBuy.OrderDirection = OrderDirections.BUY;
            tradeBuy.OrderFilledDate = tradeBuy.OrderPlacedDate.Value.AddMinutes(1);
            tradeSell.OrderDirection = OrderDirections.SELL;
            tradeSell.OrderFilledDate = tradeSell.OrderPlacedDate.Value.AddMinutes(1);

            A.CallTo(() => _tradingHoursManager.Get(A<string>.Ignored))
                .Returns(new TradingHours
                {
                    IsValid = true,
                    Mic = "XLON",
                    OpenOffsetInUtc = TimeSpan.FromHours(8),
                    CloseOffsetInUtc = TimeSpan.FromHours(16)
                });

            tradeBuy.OrderFilledVolume = 100;
            tradeSell.OrderFilledVolume = 100;
            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);

            var marketData = new EquityIntraDayTimeBarCollection(market, tradeBuy.OrderPlacedDate.Value.AddSeconds(-55),
                new List<EquityInstrumentIntraDayTimeBar>
                {
                    new EquityInstrumentIntraDayTimeBar(
                        tradeBuy.Instrument,
                        new SpreadTimeBar(
                            tradeBuy.OrderAverageFillPrice.Value,
                            tradeSell.OrderAverageFillPrice.Value,
                            tradeSell.OrderAverageFillPrice.Value,
                            new Volume(2000)),
                        new DailySummaryTimeBar(
                            1000,
                            new IntradayPrices(tradeBuy.OrderAverageFillPrice.Value, tradeBuy.OrderAverageFillPrice.Value,
                                tradeBuy.OrderAverageFillPrice.Value, tradeBuy.OrderAverageFillPrice.Value),
                            1000,
                            new Volume(2000),
                            tradeBuy.OrderPlacedDate.Value.AddSeconds(-55)
                            ),
                        tradeBuy.OrderPlacedDate.Value.AddSeconds(-55),
                     market)
                });

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.OrderPlacedDate.Value.AddMinutes(-1), new object());
            var marketDataEvent = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, tradeBuy.OrderPlacedDate.Value.AddSeconds(-55), marketData);
            var buyEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeBuy.OrderPlacedDate.Value, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeSell.OrderPlacedDate.Value, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.OrderPlacedDate.Value.AddMinutes(1), new object());

            rule.OnNext(genesis);
            rule.OnNext(marketDataEvent);
            rule.OnNext(buyEvent);
            rule.OnNext(sellEvent);
            rule.OnNext(eschaton);

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void RunRule_DoesRaiseAlertInEschaton_WhenBidirectionalTradeAndDoesExceedsWindowThreshold_AndHasMarketData()
        {
            var parameters = new LayeringRuleParameters("id", TimeSpan.FromMinutes(30), null, 0.1m, null, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _orderFilter, _logger, _factory, _tradingHoursManager, _ruleCtx, RuleRunMode.ValidationRun, _tradingLogger);
            var tradeBuy = ((Order)null).Random();
            var tradeSell = ((Order)null).Random();
            tradeBuy.OrderDirection = OrderDirections.BUY;
            tradeBuy.OrderFilledDate = tradeBuy.OrderPlacedDate.Value.AddMinutes(1);
            tradeSell.OrderDirection = OrderDirections.SELL;
            tradeSell.OrderFilledDate = tradeSell.OrderPlacedDate.Value.AddMinutes(1);

            tradeBuy.OrderFilledVolume = 300;
            tradeSell.OrderFilledVolume = 5;
            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);

            var marketData = new EquityIntraDayTimeBarCollection(market, tradeBuy.OrderPlacedDate.Value.AddSeconds(-55),
                new List<EquityInstrumentIntraDayTimeBar>
                {
                    new EquityInstrumentIntraDayTimeBar(
                        tradeBuy.Instrument,
                        new SpreadTimeBar(
                            tradeBuy.OrderAverageFillPrice.Value,
                            tradeSell.OrderAverageFillPrice.Value,
                            tradeSell.OrderAverageFillPrice.Value,
                            new Volume(2000)),
                        new DailySummaryTimeBar(
                            1000,
                            new IntradayPrices(tradeBuy.OrderAverageFillPrice.Value, tradeBuy.OrderAverageFillPrice.Value,
                                tradeBuy.OrderAverageFillPrice.Value, tradeBuy.OrderAverageFillPrice.Value),
                            1000,
                            new Volume(2000),
                            tradeBuy.OrderPlacedDate.Value.AddSeconds(-55)
                            ),
                        tradeBuy.OrderPlacedDate.Value.AddSeconds(-55),
                    market)
                });

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.OrderPlacedDate.Value.AddMinutes(-1), new object());
            var marketDataEvent = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, tradeBuy.OrderPlacedDate.Value.AddSeconds(-55), marketData);
            var buyEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeBuy.OrderPlacedDate.Value, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeSell.OrderPlacedDate.Value, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.OrderPlacedDate.Value.AddMinutes(1), new object());

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
            var parameters = new LayeringRuleParameters("id", TimeSpan.FromMinutes(30), null, 0.1m, null, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _orderFilter, _logger, _factory, _tradingHoursManager, _ruleCtx, RuleRunMode.ValidationRun, _tradingLogger);
            var tradeBuy = ((Order)null).Random();
            var tradeSell = ((Order)null).Random();
            tradeBuy.OrderDirection = OrderDirections.BUY;
            tradeBuy.OrderFilledDate = tradeBuy.OrderPlacedDate.Value.AddMinutes(1);
            tradeSell.OrderDirection = OrderDirections.SELL;
            tradeSell.OrderFilledDate = tradeSell.OrderPlacedDate.Value.AddMinutes(1);

            tradeBuy.OrderFilledVolume = 100;
            tradeSell.OrderFilledVolume = 100;
            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);

            var marketData = new EquityIntraDayTimeBarCollection(market, tradeBuy.OrderPlacedDate.Value.AddSeconds(-55),
                new List<EquityInstrumentIntraDayTimeBar>
                {
                    new EquityInstrumentIntraDayTimeBar(
                        tradeBuy.Instrument,
                        new SpreadTimeBar(
                            tradeBuy.OrderAverageFillPrice.Value,
                            tradeSell.OrderAverageFillPrice.Value,
                            tradeSell.OrderAverageFillPrice.Value,
                            new Volume(2000)),
                        new DailySummaryTimeBar(
                            1000,
                            new IntradayPrices(tradeBuy.OrderAverageFillPrice.Value, tradeBuy.OrderAverageFillPrice.Value,
                                tradeBuy.OrderAverageFillPrice.Value, tradeBuy.OrderAverageFillPrice.Value),
                            1000,
                            new Volume(2000),
                            tradeBuy.OrderPlacedDate.Value.AddSeconds(-55)),
                        tradeBuy.OrderPlacedDate.Value.AddSeconds(-55),
                        market)
                });

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.OrderPlacedDate.Value.AddMinutes(-1), new object());
            var marketDataEvent = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, tradeBuy.OrderPlacedDate.Value.AddSeconds(-55), marketData);
            var buyEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeBuy.OrderPlacedDate.Value, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeSell.OrderPlacedDate.Value, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.OrderPlacedDate.Value.AddMinutes(1), new object());

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
            var parameters = new LayeringRuleParameters("id", TimeSpan.FromMinutes(30), null, 0.1m, null, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _orderFilter, _logger, _factory, _tradingHoursManager, _ruleCtx, RuleRunMode.ValidationRun, _tradingLogger);
            var tradeBuy = ((Order)null).Random();
            var tradeSell = ((Order)null).Random();
            tradeBuy.OrderDirection = OrderDirections.BUY;
            tradeBuy.OrderFilledDate = tradeBuy.OrderPlacedDate.Value.AddMinutes(1);
            tradeSell.OrderDirection = OrderDirections.SELL;
            tradeSell.OrderFilledDate = tradeSell.OrderPlacedDate.Value.AddMinutes(1);

            tradeBuy.OrderFilledVolume = 100;
            tradeSell.OrderFilledVolume = 100;
            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);

            var marketData = new EquityIntraDayTimeBarCollection(market, tradeBuy.OrderPlacedDate.Value.AddSeconds(-55),
                new List<EquityInstrumentIntraDayTimeBar>
                {
                    new EquityInstrumentIntraDayTimeBar(
                        tradeBuy.Instrument,
                        new SpreadTimeBar(
                            tradeBuy.OrderAverageFillPrice.Value,
                            tradeSell.OrderAverageFillPrice.Value,
                            tradeSell.OrderAverageFillPrice.Value,
                            new Volume(2000)),
                        new DailySummaryTimeBar(
                            1000,
                            new IntradayPrices(tradeBuy.OrderAverageFillPrice.Value, tradeBuy.OrderAverageFillPrice.Value,
                                tradeBuy.OrderAverageFillPrice.Value, tradeBuy.OrderAverageFillPrice.Value),
                            1000,
                            new Volume(2000),
                            tradeBuy.OrderPlacedDate.Value.AddSeconds(-55)),
                        tradeBuy.OrderPlacedDate.Value.AddSeconds(-55),
                     market)
                });

            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.OrderPlacedDate.Value.AddMinutes(-1), new object());
            var marketDataEvent = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, tradeBuy.OrderPlacedDate.Value.AddSeconds(-55), marketData);
            var buyEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeBuy.OrderPlacedDate.Value, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeSell.OrderPlacedDate.Value, tradeSell);
            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.OrderPlacedDate.Value.AddMinutes(1), new object());

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
            var parameters = new LayeringRuleParameters("id", TimeSpan.FromMinutes(30), null, null, true, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _orderFilter, _logger, _factory, _tradingHoursManager, _ruleCtx, RuleRunMode.ValidationRun, _tradingLogger);
            var tradeBuy = ((Order)null).Random();
            var tradeSell = ((Order)null).Random();
            tradeBuy.OrderDirection = OrderDirections.BUY;
            tradeBuy.OrderFilledDate = tradeBuy.OrderPlacedDate.Value.AddMinutes(1);
            tradeSell.OrderDirection = OrderDirections.SELL;
            tradeSell.OrderFilledDate = tradeSell.OrderPlacedDate.Value.AddMinutes(1);

            tradeBuy.OrderFilledVolume = 300;
            tradeSell.OrderFilledVolume = 5;

            tradeBuy.OrderPlacedDate = new DateTime(2018, 10, 14, 10, 30, 0);
            tradeSell.OrderPlacedDate = tradeBuy.OrderPlacedDate.Value.AddSeconds(30);

            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);

            var initialPrice = tradeBuy.OrderAverageFillPrice.Value.Value;
            var marketData1 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice, tradeBuy.OrderPlacedDate.Value.AddSeconds(-55));
            var marketData2 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.95m, tradeBuy.OrderPlacedDate.Value.AddSeconds(-50));

            var marketData3 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice* 0.9m, tradeBuy.OrderPlacedDate.Value.AddSeconds(5));
            var marketData4 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.85m, tradeBuy.OrderPlacedDate.Value.AddSeconds(10));

            var marketData5 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice* 0.8m, tradeSell.OrderPlacedDate.Value.AddSeconds(5));
            var marketData6 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.75m, tradeSell.OrderPlacedDate.Value.AddSeconds(10));


            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.OrderPlacedDate.Value.AddMinutes(-1), new object());
            var marketDataEvent1 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData1.Epoch,  marketData1);
            var marketDataEvent2 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData2.Epoch, marketData2);

            var buyEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeBuy.OrderPlacedDate.Value, tradeBuy);

            var marketDataEvent3 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData3.Epoch, marketData3);
            var marketDataEvent4 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData4.Epoch, marketData4);

            var sellEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeSell.OrderPlacedDate.Value, tradeSell);

            var marketDataEvent5 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData5.Epoch, marketData5);
            var marketDataEvent6 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData6.Epoch, marketData6);

            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.OrderPlacedDate.Value.AddMinutes(1), new object());

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
            var parameters = new LayeringRuleParameters("id", TimeSpan.FromMinutes(30), null, null, true, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _orderFilter, _logger, _factory, _tradingHoursManager, _ruleCtx, RuleRunMode.ValidationRun, _tradingLogger);
            var tradeBuy = ((Order)null).Random();
            var tradeSell = ((Order)null).Random();
            tradeBuy.OrderDirection = OrderDirections.BUY;
            tradeBuy.OrderFilledDate = tradeBuy.OrderPlacedDate.Value.AddMinutes(1);
            tradeSell.OrderDirection = OrderDirections.SELL;
            tradeSell.OrderFilledDate = tradeSell.OrderPlacedDate.Value.AddMinutes(1);

            tradeBuy.OrderFilledVolume = 300;
            tradeSell.OrderFilledVolume = 5;

            tradeBuy.OrderPlacedDate = new DateTime(2018, 10, 14, 10, 30, 0);
            tradeSell.OrderPlacedDate = tradeBuy.OrderPlacedDate.Value.AddSeconds(30);

            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);

            var initialPrice = tradeBuy.OrderAverageFillPrice.Value.Value;
            var marketData1 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice, tradeBuy.OrderPlacedDate.Value.AddSeconds(-55));
            var marketData2 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 1.05m, tradeBuy.OrderPlacedDate.Value.AddSeconds(-50));

            var marketData3 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 1.1m, tradeBuy.OrderPlacedDate.Value.AddSeconds(5));
            var marketData4 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 1.15m, tradeBuy.OrderPlacedDate.Value.AddSeconds(10));

            var marketData5 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 1.2m, tradeSell.OrderPlacedDate.Value.AddSeconds(5));
            var marketData6 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 1.25m, tradeSell.OrderPlacedDate.Value.AddSeconds(10));


            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.OrderPlacedDate.Value.AddMinutes(-1), new object());
            var marketDataEvent1 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData1.Epoch, marketData1);
            var marketDataEvent2 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData2.Epoch, marketData2);

            var buyEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeBuy.OrderPlacedDate.Value, tradeBuy);

            var marketDataEvent3 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData3.Epoch, marketData3);
            var marketDataEvent4 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData4.Epoch, marketData4);

            var sellEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeSell.OrderPlacedDate.Value, tradeSell);

            var marketDataEvent5 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData5.Epoch, marketData5);
            var marketDataEvent6 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData6.Epoch, marketData6);

            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.OrderPlacedDate.Value.AddMinutes(1), new object());

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
            var parameters = new LayeringRuleParameters("id", TimeSpan.FromMinutes(30), null, null, true, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _orderFilter, _logger, _factory, _tradingHoursManager, _ruleCtx, RuleRunMode.ValidationRun, _tradingLogger);
            var tradeBuy = ((Order)null).Random();
            var tradeSell = ((Order)null).Random();
            tradeBuy.OrderDirection = OrderDirections.BUY;
            tradeBuy.OrderFilledDate = tradeBuy.OrderPlacedDate.Value.AddMinutes(1);
            tradeSell.OrderDirection = OrderDirections.SELL;
            tradeSell.OrderFilledDate = tradeSell.OrderPlacedDate.Value.AddMinutes(1);

            tradeBuy.OrderFilledVolume = 300;
            tradeSell.OrderFilledVolume = 5;

            tradeBuy.OrderPlacedDate = new DateTime(2018, 10, 14, 10, 30, 0);
            tradeSell.OrderPlacedDate = tradeBuy.OrderPlacedDate.Value.AddSeconds(30);

            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);

            var initialPrice = tradeBuy.OrderAverageFillPrice.Value.Value;
            var marketData1 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice, tradeBuy.OrderPlacedDate.Value.AddSeconds(-55));
            var marketData2 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.95m, tradeBuy.OrderPlacedDate.Value.AddSeconds(-50));

            var marketData3 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.9m, tradeBuy.OrderPlacedDate.Value.AddSeconds(5));
            var marketData4 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.85m, tradeBuy.OrderPlacedDate.Value.AddSeconds(10));

            var marketData5 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.8m, tradeSell.OrderPlacedDate.Value.AddSeconds(5));
            var marketData6 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 0.75m, tradeSell.OrderPlacedDate.Value.AddSeconds(10));


            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.OrderPlacedDate.Value.AddMinutes(-1), new object());
            var marketDataEvent1 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData1.Epoch, marketData1);
            var marketDataEvent2 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData2.Epoch, marketData2);

            var buyEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeBuy.OrderPlacedDate.Value, tradeBuy);

            var marketDataEvent3 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData3.Epoch, marketData3);
            var marketDataEvent4 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData4.Epoch, marketData4);

            var sellEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeSell.OrderPlacedDate.Value, tradeSell);

            var marketDataEvent5 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData5.Epoch, marketData5);
            var marketDataEvent6 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData6.Epoch, marketData6);

            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.OrderPlacedDate.Value.AddMinutes(1), new object());

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
        public void RunRule_DoesNotRaiseAlertInEschaton_WhenBidirectionalTradeAndNoPriceMovementData()
        {
            var parameters = new LayeringRuleParameters("id", TimeSpan.FromMinutes(30), null, null, true, null, false);
            var rule = new LayeringRule(parameters, _alertStream, _orderFilter, _logger, _factory, _tradingHoursManager, _ruleCtx, RuleRunMode.ValidationRun, _tradingLogger);
            var tradeBuy = ((Order)null).Random();
            var tradeSell = ((Order)null).Random();
            tradeBuy.OrderDirection = OrderDirections.BUY;
            tradeBuy.OrderFilledDate = tradeBuy.OrderPlacedDate.Value.AddMinutes(1);
            tradeSell.OrderDirection = OrderDirections.SELL;
            tradeSell.OrderFilledDate = tradeSell.OrderPlacedDate.Value.AddMinutes(1);

            tradeBuy.OrderFilledVolume = 300;
            tradeSell.OrderFilledVolume = 5;

            tradeBuy.OrderPlacedDate = new DateTime(2018, 10, 14, 10, 30, 0);
            tradeSell.OrderPlacedDate = tradeBuy.OrderPlacedDate.Value.AddSeconds(30);

            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
            var initialPrice = tradeBuy.OrderAverageFillPrice.Value.Value;
            var marketData5 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 1.2m, tradeSell.OrderPlacedDate.Value.AddSeconds(5));
            var marketData6 = SetExchangeFrameToPrice(market, tradeBuy, tradeSell, initialPrice * 1.25m, tradeSell.OrderPlacedDate.Value.AddSeconds(10));


            var genesis = new UniverseEvent(UniverseStateEvent.Genesis, tradeBuy.OrderPlacedDate.Value.AddMinutes(-1), new object());
            var buyEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeBuy.OrderPlacedDate.Value, tradeBuy);
            var sellEvent = new UniverseEvent(UniverseStateEvent.OrderPlaced, tradeSell.OrderPlacedDate.Value, tradeSell);

            var marketDataEvent5 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData5.Epoch, marketData5);
            var marketDataEvent6 = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, marketData6.Epoch, marketData6);

            var eschaton = new UniverseEvent(UniverseStateEvent.Eschaton, tradeSell.OrderPlacedDate.Value.AddMinutes(1), new object());

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

        private EquityIntraDayTimeBarCollection SetExchangeFrameToPrice(
            Market market,
            Order baseBuyFrame,
            Order baseSellFrame,
            decimal price,
            DateTime timestamp)
        {
            return new EquityIntraDayTimeBarCollection(market, timestamp,
                new List<EquityInstrumentIntraDayTimeBar>
                {
                    new EquityInstrumentIntraDayTimeBar(
                        baseBuyFrame.Instrument,
                        new SpreadTimeBar(
                            baseBuyFrame.OrderAverageFillPrice.Value, 
                            baseSellFrame.OrderAverageFillPrice.Value,
                            new CurrencyAmount(price, baseSellFrame.OrderCurrency),
                            new Volume(2000)),
                        new DailySummaryTimeBar(
                            1000,
                            new IntradayPrices(baseBuyFrame.OrderAverageFillPrice.Value, baseBuyFrame.OrderAverageFillPrice.Value,
                                baseBuyFrame.OrderAverageFillPrice.Value, baseBuyFrame.OrderAverageFillPrice.Value),
                            1000,
                            new Volume(2000),
                            timestamp),
                        timestamp,
                        market)
                });
        }
    }
}
