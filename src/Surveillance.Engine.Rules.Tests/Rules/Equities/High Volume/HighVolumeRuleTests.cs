﻿using System;
using System.Collections.Generic;
using Domain.Equity.TimeBars;
using Domain.Financial;
using Domain.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Rules;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
using Surveillance.Engine.Rules.Tests.Helpers;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Rules.Equities.High_Volume
{
    [TestFixture]
    public class HighVolumeRuleTests
    {
        private IUniverseAlertStream _alertStream;
        private IHighVolumeRuleEquitiesParameters _equitiesParameters;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private ISystemProcessOperationContext _opCtx;
        private IUniverseOrderFilter _orderFilter;
        private IUniverseMarketCacheFactory _factory;
        private IMarketTradingHoursManager _tradingHoursManager;
        private IRuleRunDataRequestRepository _dataRequestRepository;
        private IStubRuleRunDataRequestRepository _stubDataRequestRepository;
        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private ILogger<IHighVolumeRule> _logger;
        private ILogger<UniverseMarketCacheFactory> _factoryCache;
        private ILogger<TradingHistoryStack> _tradingLogger;

        [SetUp]
        public void Setup()
        {
            _alertStream = A.Fake<IUniverseAlertStream>();
            _equitiesParameters = A.Fake<IHighVolumeRuleEquitiesParameters>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _opCtx = A.Fake<ISystemProcessOperationContext>();
            _dataRequestRepository = A.Fake<IRuleRunDataRequestRepository>();
            _stubDataRequestRepository = A.Fake<IStubRuleRunDataRequestRepository>();

            _factoryCache = A.Fake<ILogger<UniverseMarketCacheFactory>>();
            _factory = new UniverseMarketCacheFactory(_stubDataRequestRepository, _dataRequestRepository, _factoryCache);
            _tradingHoursManager = A.Fake<IMarketTradingHoursManager>();
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            _logger = A.Fake<ILogger<IHighVolumeRule>>();
            _tradingLogger = A.Fake<ILogger<TradingHistoryStack>>();

            _orderFilter = A.Fake<IUniverseOrderFilter>();
            A.CallTo(() => _orderFilter.Filter(A<IUniverseEvent>.Ignored)).ReturnsLazily(i => (IUniverseEvent)i.Arguments[0]);

            A.CallTo(() => _ruleCtx.EndEvent()).Returns(_opCtx);
        }

        [Test]
        public void Constructor_ConsidersNullParameters_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement

            Assert.Throws<ArgumentNullException>(() => new HighVolumeRule(null, _ruleCtx, _alertStream, _orderFilter, _factory, _tradingHoursManager, _dataRequestSubscriber, RuleRunMode.ValidationRun, _logger, _tradingLogger));
        }

        [Test]
        public void Constructor_ConsidersNullOpCtx_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement

            Assert.Throws<ArgumentNullException>(() => new HighVolumeRule(_equitiesParameters, null, _alertStream, _orderFilter, _factory, _tradingHoursManager, _dataRequestSubscriber, RuleRunMode.ValidationRun, _logger, _tradingLogger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement

            Assert.Throws<ArgumentNullException>(() => new HighVolumeRule(_equitiesParameters, _ruleCtx, _alertStream, _orderFilter, _factory, _tradingHoursManager, _dataRequestSubscriber, RuleRunMode.ValidationRun, null, _tradingLogger));
        }

        [Test]
        public void Eschaton_UpdatesAlertCountAndEndsEvent_ForCtx()
        {
            var highVolumeRule = BuildRule();

            highVolumeRule.OnNext(Eschaton());

            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Eschaton_SetsMissingData_WhenExchangeDataMissing()
        {
            A.CallTo(() => _equitiesParameters.HighVolumePercentageDaily).Returns(0.1m);
            var highVolumeRule = BuildRule();

            highVolumeRule.OnNext(Trade());
            highVolumeRule.OnNext(Eschaton());

            A.CallTo(() => _dataRequestSubscriber.SubmitRequest()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void DailyParameter_NoThresholdBreach_DoesNotRaiseAlert()
        {
            A.CallTo(() => _equitiesParameters.HighVolumePercentageDaily).Returns(0.1m);
            A.CallTo(() => _equitiesParameters.WindowSize).Returns(TimeSpan.FromHours(1));
            var highVolumeRule = BuildRule();

            var trade = Trade();
            var underlyingTrade = (Order)trade.UnderlyingEvent;
            underlyingTrade.FilledDate = DateTime.UtcNow;
            underlyingTrade.OrderFilledVolume = 10;
            underlyingTrade.FilledDate = DateTime.UtcNow;
            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
            var marketData = new EquityIntraDayTimeBarCollection(market, underlyingTrade.PlacedDate.Value.AddSeconds(-55),
                new List<EquityInstrumentIntraDayTimeBar>
                {
                    new EquityInstrumentIntraDayTimeBar(
                        underlyingTrade.Instrument,
                        new SpreadTimeBar(
                            underlyingTrade.OrderAverageFillPrice.Value,
                            underlyingTrade.OrderAverageFillPrice.Value,
                            underlyingTrade.OrderAverageFillPrice.Value,
                            new Volume(2000)),
                        new DailySummaryTimeBar(
                            1000m,
                            new IntradayPrices(
                                underlyingTrade.OrderAverageFillPrice.Value, 
                                underlyingTrade.OrderAverageFillPrice.Value,
                                underlyingTrade.OrderAverageFillPrice.Value,
                                underlyingTrade.OrderAverageFillPrice.Value),
                            10000,
                            new Volume(10000),
                            underlyingTrade.PlacedDate.Value.AddSeconds(-55)),
                        underlyingTrade.PlacedDate.Value.AddSeconds(-55),
                        market)
                });

            var marketEvent =
                new UniverseEvent(
                    UniverseStateEvent.EquityIntradayTick,
                    DateTime.UtcNow.AddMinutes(-1),
                    marketData);

            highVolumeRule.OnNext(marketEvent);
            highVolumeRule.OnNext(trade);
            highVolumeRule.OnNext(Eschaton());

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void DailyParameter_ThresholdBreach_RaisesAlert()
        {
            A.CallTo(() => _equitiesParameters.HighVolumePercentageDaily).Returns(0.1m);
            A.CallTo(() => _equitiesParameters.WindowSize).Returns(TimeSpan.FromHours(1));
            var highVolumeRule = BuildRule();

            var trade = Trade();
            var underlyingTrade = (Order)trade.UnderlyingEvent;
            underlyingTrade.OrderFilledVolume = 300;
            underlyingTrade.PlacedDate = DateTime.UtcNow;
            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
            var marketData = new EquityIntraDayTimeBarCollection(market, underlyingTrade.PlacedDate.Value.AddSeconds(-55),
                new List<EquityInstrumentIntraDayTimeBar>
                {
                    new EquityInstrumentIntraDayTimeBar
                    (underlyingTrade.Instrument,
                        new SpreadTimeBar(
                            underlyingTrade.OrderAverageFillPrice.Value, 
                            underlyingTrade.OrderAverageFillPrice.Value,
                            underlyingTrade.OrderAverageFillPrice.Value,
                            new Volume(2000)),
                        new DailySummaryTimeBar(
                            100000,
                            new IntradayPrices(underlyingTrade.OrderAverageFillPrice.Value, underlyingTrade.OrderAverageFillPrice.Value,
                                underlyingTrade.OrderAverageFillPrice.Value, underlyingTrade.OrderAverageFillPrice.Value),
                            1000,
                            new Volume(1000),
                            underlyingTrade.PlacedDate.Value.AddSeconds(-55)
                            ),
                        underlyingTrade.PlacedDate.Value.AddSeconds(-55),
                        market)
                });

            var marketEvent =
                new UniverseEvent(
                    UniverseStateEvent.EquityIntradayTick,
                    DateTime.UtcNow.AddMinutes(-1),
                    marketData);

            highVolumeRule.OnNext(marketEvent);
            highVolumeRule.OnNext(trade);
            highVolumeRule.OnNext(Eschaton());

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void WindowParameter_ThresholdBreach_RaisesAlert()
        {
            A.CallTo(() => _equitiesParameters.HighVolumePercentageWindow).Returns(0.1m);
            A.CallTo(() => _equitiesParameters.WindowSize).Returns(TimeSpan.FromHours(1));
            var highVolumeRule = BuildRule();

            var trade = Trade();
            var underlyingTrade = (Order)trade.UnderlyingEvent;
            underlyingTrade.FilledDate = DateTime.UtcNow;
            underlyingTrade.OrderFilledVolume = 300;
            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
            var marketData = new EquityIntraDayTimeBarCollection(market, underlyingTrade.PlacedDate.Value.AddSeconds(-55),
                new List<EquityInstrumentIntraDayTimeBar>
                {
                    new EquityInstrumentIntraDayTimeBar(
                        underlyingTrade.Instrument,
                        new SpreadTimeBar(
                            underlyingTrade.OrderAverageFillPrice.Value,
                            underlyingTrade.OrderAverageFillPrice.Value,
                            underlyingTrade.OrderAverageFillPrice.Value,
                            new Volume(2000)),
                        new DailySummaryTimeBar(
                            1000,
                            new IntradayPrices(
                                underlyingTrade.OrderAverageFillPrice.Value,
                                underlyingTrade.OrderAverageFillPrice.Value,
                                underlyingTrade.OrderAverageFillPrice.Value, 
                                underlyingTrade.OrderAverageFillPrice.Value),
                            1000,
                            new Volume(2000),
                            underlyingTrade.PlacedDate.Value.AddSeconds(-55)),
                        underlyingTrade.PlacedDate.Value.AddSeconds(-55),
                        market)
                });

            var marketEvent =
                new UniverseEvent(
                    UniverseStateEvent.EquityIntradayTick,
                    DateTime.UtcNow.AddMinutes(-1),
                    marketData);

            highVolumeRule.OnNext(marketEvent);
            highVolumeRule.OnNext(trade);
            highVolumeRule.OnNext(Eschaton());

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappened();
        }

        private HighVolumeRule BuildRule()
        {
            return new HighVolumeRule(
                _equitiesParameters,
                _ruleCtx,
                _alertStream,
                _orderFilter,
                _factory,
                _tradingHoursManager,
                _dataRequestSubscriber,
                RuleRunMode.ValidationRun,
                _logger,
                _tradingLogger);
        }

        private IUniverseEvent Trade()
        {
            var trade = ((Order)null).Random();
            return new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, trade);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private IUniverseEvent Eschaton()
        {
            var underlyingEvent = new ScheduledExecution();
            return new UniverseEvent(UniverseStateEvent.Eschaton, DateTime.UtcNow, underlyingEvent);
        }
    }
}
