namespace Surveillance.Engine.Rules.Tests.Rules.Equities.High_Volume
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Markets;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading.Orders;

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
    using Surveillance.Engine.Rules.RuleParameters;
    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.OrganisationalFactors;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
    using Surveillance.Engine.Rules.Tests.Helpers;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;

    [TestFixture]
    public class HighVolumeRuleTests
    {
        private IUniverseAlertStream _alertStream;

        private IRuleRunDataRequestRepository _dataRequestRepository;

        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private IHighVolumeRuleEquitiesParameters _equitiesParameters;

        private IUniverseMarketCacheFactory _factory;

        private ILogger<UniverseMarketCacheFactory> _factoryCache;

        private ILogger<IHighVolumeRule> _logger;

        private ISystemProcessOperationContext _opCtx;

        private IUniverseOrderFilter _orderFilter;

        private ISystemProcessOperationRunRuleContext _ruleCtx;

        private IStubRuleRunDataRequestRepository _stubDataRequestRepository;

        private IMarketTradingHoursService _tradingHoursService;

        private ILogger<TradingHistoryStack> _tradingLogger;

        [Test]
        public void Clone_Copies_FactorValue_To_New_Clone()
        {
            var rule = this.BuildRule();
            var factor = new FactorValue(ClientOrganisationalFactors.Fund, "abcd");

            var clone = rule.Clone(factor);

            Assert.AreEqual(rule.OrganisationFactorValue.OrganisationalFactors, ClientOrganisationalFactors.None);
            Assert.AreEqual(rule.OrganisationFactorValue.Value, string.Empty);

            Assert.AreEqual(clone.OrganisationFactorValue.OrganisationalFactors, ClientOrganisationalFactors.Fund);
            Assert.AreEqual(clone.OrganisationFactorValue.Value, "abcd");
        }

        [Test]
        public void Constructor_ConsidersNullLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new HighVolumeRule(
                    this._equitiesParameters,
                    this._ruleCtx,
                    this._alertStream,
                    this._orderFilter,
                    this._factory,
                    this._tradingHoursService,
                    this._dataRequestSubscriber,
                    RuleRunMode.ValidationRun,
                    null,
                    this._tradingLogger));
        }

        [Test]
        public void Constructor_ConsidersNullOpCtx_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new HighVolumeRule(
                    this._equitiesParameters,
                    null,
                    this._alertStream,
                    this._orderFilter,
                    this._factory,
                    this._tradingHoursService,
                    this._dataRequestSubscriber,
                    RuleRunMode.ValidationRun,
                    this._logger,
                    this._tradingLogger));
        }

        [Test]
        public void Constructor_ConsidersNullParameters_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new HighVolumeRule(
                    null,
                    this._ruleCtx,
                    this._alertStream,
                    this._orderFilter,
                    this._factory,
                    this._tradingHoursService,
                    this._dataRequestSubscriber,
                    RuleRunMode.ValidationRun,
                    this._logger,
                    this._tradingLogger));
        }

        [Test]
        public void DailyParameter_NoThresholdBreach_DoesNotRaiseAlert()
        {
            var windows = new TimeWindows("id", TimeSpan.FromHours(1));
            A.CallTo(() => this._equitiesParameters.HighVolumePercentageDaily).Returns(0.1m);
            A.CallTo(() => this._equitiesParameters.Windows).Returns(windows);
            var highVolumeRule = this.BuildRule();

            var trade = this.Trade();
            var underlyingTrade = (Order)trade.UnderlyingEvent;
            underlyingTrade.FilledDate = DateTime.UtcNow;
            underlyingTrade.OrderFilledVolume = 10;
            underlyingTrade.FilledDate = DateTime.UtcNow;
            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
            var marketData = new EquityIntraDayTimeBarCollection(
                market,
                underlyingTrade.PlacedDate.Value.AddSeconds(-55),
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

            var marketEvent = new UniverseEvent(
                UniverseStateEvent.EquityIntradayTick,
                DateTime.UtcNow.AddMinutes(-1),
                marketData);

            highVolumeRule.OnNext(marketEvent);
            highVolumeRule.OnNext(trade);
            highVolumeRule.OnNext(this.Eschaton());

            A.CallTo(() => this._alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void DailyParameter_ThresholdBreach_RaisesAlert()
        {
            var windows = new TimeWindows("id", TimeSpan.FromHours(1));
            A.CallTo(() => this._equitiesParameters.HighVolumePercentageDaily).Returns(0.1m);
            A.CallTo(() => this._equitiesParameters.Windows).Returns(windows);
            var highVolumeRule = this.BuildRule();

            var trade = this.Trade();
            var underlyingTrade = (Order)trade.UnderlyingEvent;
            underlyingTrade.OrderFilledVolume = 300;
            underlyingTrade.PlacedDate = DateTime.UtcNow;
            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
            var marketData = new EquityIntraDayTimeBarCollection(
                market,
                underlyingTrade.PlacedDate.Value.AddSeconds(-55),
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
                                100000,
                                new IntradayPrices(
                                    underlyingTrade.OrderAverageFillPrice.Value,
                                    underlyingTrade.OrderAverageFillPrice.Value,
                                    underlyingTrade.OrderAverageFillPrice.Value,
                                    underlyingTrade.OrderAverageFillPrice.Value),
                                1000,
                                new Volume(1000),
                                underlyingTrade.PlacedDate.Value.AddSeconds(-55)),
                            underlyingTrade.PlacedDate.Value.AddSeconds(-55),
                            market)
                    });

            var marketEvent = new UniverseEvent(
                UniverseStateEvent.EquityIntradayTick,
                DateTime.UtcNow.AddMinutes(-1),
                marketData);

            highVolumeRule.OnNext(marketEvent);
            highVolumeRule.OnNext(trade);
            highVolumeRule.OnNext(this.Eschaton());

            A.CallTo(() => this._alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void Eschaton_SetsMissingData_WhenExchangeDataMissing()
        {
            A.CallTo(() => this._equitiesParameters.HighVolumePercentageDaily).Returns(0.1m);
            var highVolumeRule = this.BuildRule();

            highVolumeRule.OnNext(this.Trade());
            highVolumeRule.OnNext(this.Eschaton());

            A.CallTo(() => this._dataRequestSubscriber.SubmitRequest()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Eschaton_UpdatesAlertCountAndEndsEvent_ForCtx()
        {
            var highVolumeRule = this.BuildRule();

            highVolumeRule.OnNext(this.Eschaton());

            A.CallTo(() => this._ruleCtx.EndEvent()).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._alertStream = A.Fake<IUniverseAlertStream>();
            this._equitiesParameters = A.Fake<IHighVolumeRuleEquitiesParameters>();
            this._ruleCtx = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._opCtx = A.Fake<ISystemProcessOperationContext>();
            this._dataRequestRepository = A.Fake<IRuleRunDataRequestRepository>();
            this._stubDataRequestRepository = A.Fake<IStubRuleRunDataRequestRepository>();

            this._factoryCache = A.Fake<ILogger<UniverseMarketCacheFactory>>();
            this._factory = new UniverseMarketCacheFactory(
                this._stubDataRequestRepository,
                this._dataRequestRepository,
                this._factoryCache);
            this._tradingHoursService = A.Fake<IMarketTradingHoursService>();
            this._dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            this._logger = A.Fake<ILogger<IHighVolumeRule>>();
            this._tradingLogger = A.Fake<ILogger<TradingHistoryStack>>();

            this._orderFilter = A.Fake<IUniverseOrderFilter>();
            A.CallTo(() => this._orderFilter.Filter(A<IUniverseEvent>.Ignored))
                .ReturnsLazily(i => (IUniverseEvent)i.Arguments[0]);

            A.CallTo(() => this._ruleCtx.EndEvent()).Returns(this._opCtx);
        }

        [Test]
        public void WindowParameter_ThresholdBreach_RaisesAlert()
        {
            var windows = new TimeWindows("id", TimeSpan.FromHours(1));
            A.CallTo(() => this._equitiesParameters.HighVolumePercentageWindow).Returns(0.1m);
            A.CallTo(() => this._equitiesParameters.Windows).Returns(windows);
            var highVolumeRule = this.BuildRule();

            var trade = this.Trade();
            var underlyingTrade = (Order)trade.UnderlyingEvent;
            underlyingTrade.FilledDate = DateTime.UtcNow;
            underlyingTrade.OrderFilledVolume = 300;
            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
            var marketData = new EquityIntraDayTimeBarCollection(
                market,
                underlyingTrade.PlacedDate.Value.AddSeconds(-55),
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

            var marketEvent = new UniverseEvent(
                UniverseStateEvent.EquityIntradayTick,
                DateTime.UtcNow.AddMinutes(-1),
                marketData);

            highVolumeRule.OnNext(marketEvent);
            highVolumeRule.OnNext(trade);
            highVolumeRule.OnNext(this.Eschaton());

            A.CallTo(() => this._alertStream.Add(A<IUniverseAlertEvent>.Ignored)).MustHaveHappened();
        }

        private HighVolumeRule BuildRule()
        {
            return new HighVolumeRule(
                this._equitiesParameters,
                this._ruleCtx,
                this._alertStream,
                this._orderFilter,
                this._factory,
                this._tradingHoursService,
                this._dataRequestSubscriber,
                RuleRunMode.ValidationRun,
                this._logger,
                this._tradingLogger);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private IUniverseEvent Eschaton()
        {
            var underlyingEvent = new ScheduledExecution();
            return new UniverseEvent(UniverseStateEvent.Eschaton, DateTime.UtcNow, underlyingEvent);
        }

        private IUniverseEvent Trade()
        {
            var trade = ((Order)null).Random();
            return new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, trade);
        }
    }
}