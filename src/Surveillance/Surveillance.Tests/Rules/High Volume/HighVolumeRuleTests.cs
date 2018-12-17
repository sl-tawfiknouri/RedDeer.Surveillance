using System;
using System.Collections.Generic;
using DomainV2.Equity;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using DomainV2.Scheduling;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules.HighVolume;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Tests.Helpers;
using Surveillance.Universe;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Tests.Rules.High_Volume
{
    [TestFixture]
    public class HighVolumeRuleTests
    {
        private IUniverseAlertStream _alertStream;
        private IHighVolumeRuleParameters _parameters;
        private ISystemProcessOperationRunRuleContext _ruleCtx;
        private ISystemProcessOperationContext _opCtx;
        private IUniverseMarketCacheFactory _factory;
        private IMarketTradingHoursManager _tradingHoursManager;
        private ILogger<IHighVolumeRule> _logger;

        [SetUp]
        public void Setup()
        {
            _alertStream = A.Fake<IUniverseAlertStream>();
            _parameters = A.Fake<IHighVolumeRuleParameters>();
            _ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            _opCtx = A.Fake<ISystemProcessOperationContext>();
            _factory = A.Fake<IUniverseMarketCacheFactory>();
            _tradingHoursManager = A.Fake<IMarketTradingHoursManager>();
            _logger = A.Fake<ILogger<IHighVolumeRule>>();

            A.CallTo(() => _ruleCtx.EndEvent()).Returns(_opCtx);
        }

        [Test]
        public void Constructor_ConsidersNullParameters_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new HighVolumeRule(null, _ruleCtx, _alertStream, _factory, _tradingHoursManager, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullOpCtx_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new HighVolumeRule(_parameters, null, _alertStream, _factory, _tradingHoursManager, _logger));
        }

        [Test]
        public void Constructor_ConsidersNullLogger_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new HighVolumeRule(_parameters, _ruleCtx, _alertStream, _factory, _tradingHoursManager, null));
        }

        [Test]
        public void Eschaton_UpdatesAlertCountAndEndsEvent_ForCtx()
        {
            var highVolumeRule = new HighVolumeRule(_parameters, _ruleCtx, _alertStream, new UniverseMarketCacheFactory(), _tradingHoursManager, _logger);

            highVolumeRule.OnNext(Eschaton());

            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Eschaton_SetsMissingData_WhenExchangeDataMissing()
        {
            A.CallTo(() => _parameters.HighVolumePercentageDaily).Returns(0.1m);
            var highVolumeRule = new HighVolumeRule(_parameters, _ruleCtx, _alertStream, new UniverseMarketCacheFactory(), _tradingHoursManager, _logger);

            highVolumeRule.OnNext(Trade());
            highVolumeRule.OnNext(Eschaton());

            A.CallTo(() => _opCtx.EndEventWithMissingDataError()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void DailyParameter_NoThresholdBreach_DoesNotRaiseAlert()
        {
            A.CallTo(() => _parameters.HighVolumePercentageDaily).Returns(0.1m);
            A.CallTo(() => _parameters.WindowSize).Returns(TimeSpan.FromHours(1));
            var highVolumeRule = new HighVolumeRule(_parameters, _ruleCtx, _alertStream, new UniverseMarketCacheFactory(), _tradingHoursManager, _logger);

            var trade = Trade();
            var underlyingTrade = (Order)trade.UnderlyingEvent;
            underlyingTrade.OrderFilledDate = DateTime.UtcNow;
            underlyingTrade.OrderFilledVolume = 10;
            underlyingTrade.OrderFilledDate = DateTime.UtcNow;
            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
            var marketData = new ExchangeFrame(market, underlyingTrade.OrderPlacedDate.Value.AddSeconds(-55),
                new List<SecurityTick>
                {
                    new SecurityTick(underlyingTrade.Instrument,
                        new Spread(underlyingTrade.OrderAveragePrice.Value, underlyingTrade.OrderAveragePrice.Value,
                            underlyingTrade.OrderAveragePrice.Value), new Volume(2000), new Volume(2000),
                        underlyingTrade.OrderPlacedDate.Value.AddSeconds(-55), 100000,
                        new IntradayPrices(underlyingTrade.OrderAveragePrice.Value, underlyingTrade.OrderAveragePrice.Value,
                            underlyingTrade.OrderAveragePrice.Value, underlyingTrade.OrderAveragePrice.Value), 5000, market)
                });

            var marketEvent =
                new UniverseEvent(
                    UniverseStateEvent.StockTickReddeer,
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
            A.CallTo(() => _parameters.HighVolumePercentageDaily).Returns(0.1m);
            A.CallTo(() => _parameters.WindowSize).Returns(TimeSpan.FromHours(1));
            var highVolumeRule = new HighVolumeRule(_parameters, _ruleCtx, _alertStream, new UniverseMarketCacheFactory(), _tradingHoursManager, _logger);

            var trade = Trade();
            var underlyingTrade = (Order)trade.UnderlyingEvent;
            underlyingTrade.OrderFilledVolume = 300;
            underlyingTrade.OrderPlacedDate = DateTime.UtcNow;
            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
            var marketData = new ExchangeFrame(market, underlyingTrade.OrderPlacedDate.Value.AddSeconds(-55),
                new List<SecurityTick>
                {
                    new SecurityTick(underlyingTrade.Instrument,
                        new Spread(underlyingTrade.OrderAveragePrice.Value, underlyingTrade.OrderAveragePrice.Value,
                            underlyingTrade.OrderAveragePrice.Value), new Volume(2000), new Volume(2000),
                        underlyingTrade.OrderPlacedDate.Value.AddSeconds(-55), 100000,
                        new IntradayPrices(underlyingTrade.OrderAveragePrice.Value, underlyingTrade.OrderAveragePrice.Value,
                            underlyingTrade.OrderAveragePrice.Value, underlyingTrade.OrderAveragePrice.Value), 5000, market)
                });

            var marketEvent =
                new UniverseEvent(
                    UniverseStateEvent.StockTickReddeer,
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
            A.CallTo(() => _parameters.HighVolumePercentageWindow).Returns(0.1m);
            A.CallTo(() => _parameters.WindowSize).Returns(TimeSpan.FromHours(1));
            var highVolumeRule = new HighVolumeRule(_parameters, _ruleCtx, _alertStream, new UniverseMarketCacheFactory(), _tradingHoursManager, _logger);

            var trade = Trade();
            var underlyingTrade = (Order)trade.UnderlyingEvent;
            underlyingTrade.OrderFilledDate = DateTime.Now;
            underlyingTrade.OrderFilledVolume = 300;
            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
            var marketData = new ExchangeFrame(market, underlyingTrade.OrderPlacedDate.Value.AddSeconds(-55),
                new List<SecurityTick>
                {
                    new SecurityTick(underlyingTrade.Instrument,
                        new Spread(underlyingTrade.OrderAveragePrice.Value, underlyingTrade.OrderAveragePrice.Value,
                            underlyingTrade.OrderAveragePrice.Value), new Volume(2000), new Volume(2000),
                        underlyingTrade.OrderPlacedDate.Value.AddSeconds(-55), 100000,
                        new IntradayPrices(underlyingTrade.OrderAveragePrice.Value, underlyingTrade.OrderAveragePrice.Value,
                            underlyingTrade.OrderAveragePrice.Value, underlyingTrade.OrderAveragePrice.Value), 5000, market)
                });

            var marketEvent =
                new UniverseEvent(
                    UniverseStateEvent.StockTickReddeer,
                    DateTime.UtcNow.AddMinutes(-1),
                    marketData);

            highVolumeRule.OnNext(marketEvent);
            highVolumeRule.OnNext(trade);
            highVolumeRule.OnNext(Eschaton());

            A.CallTo(() => _alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappened();
        }

        private IUniverseEvent Trade()
        {
            var trade = ((Order)null).Random();
            return new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.UtcNow, trade);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private IUniverseEvent Eschaton()
        {
            var underlyingEvent = new ScheduledExecution();
            return new UniverseEvent(UniverseStateEvent.Eschaton, DateTime.UtcNow, underlyingEvent);
        }
    }
}
