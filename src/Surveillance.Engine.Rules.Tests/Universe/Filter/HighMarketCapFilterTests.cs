namespace Surveillance.Engine.Rules.Tests.Universe.Filter
{
    using System;

    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading.Orders;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Tests.Helpers;
    using Surveillance.Engine.Rules.Universe;
    using Surveillance.Engine.Rules.Universe.Filter;

    public class HighMarketCapFilterTests
    {
        private ILogger<HighMarketCapFilter> _logger;

        private ISystemProcessOperationRunRuleContext _operationRunRuleContext;

        private IMarketTradingHoursService _tradingHoursService;

        private IUniverseDataRequestsSubscriber _universeDataRequestsSubscriber;

        private IUniverseEquityInterDayCache _universeEquityInterDayCache;

        private IUniverseMarketCacheFactory _universeMarketCacheFactory;

        [Test]
        public void Filter_WhenUniverseEventAndHadMissingDataInUniverseEquityInterdayCache_MustBeFiltered()
        {
            var fundOne = ((Order)null).Random();
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            A.CallTo(() => this._tradingHoursService.GetTradingHoursForMic(fundOne.Market.MarketIdentifierCode))
                .Returns(new TradingHours { IsValid = true });

            A.CallTo(
                () => this._universeEquityInterDayCache.Get(
                    A<MarketDataRequest>.That.Matches(
                        m => m.MarketIdentifierCode == fundOne.Market.MarketIdentifierCode
                             && m.Cfi == fundOne.Instrument.Cfi))).Returns(
                MarketDataResponse<EquityInstrumentInterDayTimeBar>.MissingData());

            var marketCapRangeRuleFilter = new DecimalRangeRuleFilter { Type = RuleFilterType.Include };

            var highMarketCapFilter = new HighMarketCapFilter(
                this._universeMarketCacheFactory,
                RuleRunMode.ValidationRun,
                marketCapRangeRuleFilter,
                this._tradingHoursService,
                this._operationRunRuleContext,
                this._universeDataRequestsSubscriber,
                "test",
                this._logger);

            var result = highMarketCapFilter.Filter(eventOne);

            Assert.IsTrue(result);
        }

        [TestCase(150, null, null, false)]
        [TestCase(150, 100, 200, true)]
        [TestCase(50, 100, 200, true)]
        [TestCase(250, 100, 200, true)]
        public void Filter_WhenUniverseEventAndMarketCapFilter_MustFilterCorrectly(
            decimal? marketCap,
            decimal? min,
            decimal? max,
            bool mustBeFiltered)
        {
            var fundOne = ((Order)null).Random();
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            A.CallTo(() => this._tradingHoursService.GetTradingHoursForMic(fundOne.Market.MarketIdentifierCode))
                .Returns(new TradingHours { IsValid = true });

            // decimal? marketCap = 150;
            var marketDataResponse = new MarketDataResponse<EquityInstrumentInterDayTimeBar>(
                new EquityInstrumentInterDayTimeBar(
                    null,
                    new DailySummaryTimeBar(marketCap, null, null, new Volume(), DateTime.UtcNow),
                    DateTime.UtcNow,
                    null),
                false,
                false);

            A.CallTo(
                () => this._universeEquityInterDayCache.Get(
                    A<MarketDataRequest>.That.Matches(
                        m => m.MarketIdentifierCode == fundOne.Market.MarketIdentifierCode
                             && m.Cfi == fundOne.Instrument.Cfi))).Returns(marketDataResponse);

            var marketCapRangeRuleFilter = new DecimalRangeRuleFilter
                                               {
                                                   Type = RuleFilterType.Include, Min = min, Max = max
                                               };

            var highMarketCapFilter = new HighMarketCapFilter(
                this._universeMarketCacheFactory,
                RuleRunMode.ValidationRun,
                marketCapRangeRuleFilter,
                this._tradingHoursService,
                this._operationRunRuleContext,
                this._universeDataRequestsSubscriber,
                "test",
                this._logger);

            var result = highMarketCapFilter.Filter(eventOne);

            Assert.AreEqual(result, mustBeFiltered);
        }

        [Test]
        public void Filter_WhenUniverseEventAndTradingHoursIsNotValid_MustBeFiltered()
        {
            var fundOne = ((Order)null).Random();
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            A.CallTo(() => this._tradingHoursService.GetTradingHoursForMic(fundOne.Market.MarketIdentifierCode))
                .Returns(new TradingHours { IsValid = false });

            var marketCapRangeRuleFilter = new DecimalRangeRuleFilter { Type = RuleFilterType.Include };

            var highMarketCapFilter = new HighMarketCapFilter(
                this._universeMarketCacheFactory,
                RuleRunMode.ValidationRun,
                marketCapRangeRuleFilter,
                this._tradingHoursService,
                this._operationRunRuleContext,
                this._universeDataRequestsSubscriber,
                "test",
                this._logger);

            var result = highMarketCapFilter.Filter(eventOne);

            Assert.IsTrue(result);
        }

        [Test]
        public void Filter_WhenUniverseEventExcludeMarketCapFilter_MustNotBeFiltered()
        {
            var marketCapRangeRuleFilter = new DecimalRangeRuleFilter { Type = RuleFilterType.Exclude };

            var highMarketCapFilter = new HighMarketCapFilter(
                this._universeMarketCacheFactory,
                RuleRunMode.ValidationRun,
                marketCapRangeRuleFilter,
                this._tradingHoursService,
                this._operationRunRuleContext,
                this._universeDataRequestsSubscriber,
                "test",
                this._logger);

            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, new { });

            var result = highMarketCapFilter.Filter(eventOne);

            Assert.IsFalse(result);
        }

        [Test]
        public void Filter_WhenUniverseEventNoneMarketCapFilter_MustNotBeFiltered()
        {
            var marketCapRangeRuleFilter = DecimalRangeRuleFilter.None();

            var highMarketCapFilter = new HighMarketCapFilter(
                this._universeMarketCacheFactory,
                RuleRunMode.ValidationRun,
                marketCapRangeRuleFilter,
                this._tradingHoursService,
                this._operationRunRuleContext,
                this._universeDataRequestsSubscriber,
                "test",
                this._logger);

            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, new { });

            var result = highMarketCapFilter.Filter(eventOne);

            Assert.IsFalse(result);
        }

        [Test]
        public void Filter_WhenUniverseEventNull_MustNotBeFiltered()
        {
            var marketCapRangeRuleFilter = DecimalRangeRuleFilter.None();

            var highMarketCapFilter = new HighMarketCapFilter(
                this._universeMarketCacheFactory,
                RuleRunMode.ValidationRun,
                marketCapRangeRuleFilter,
                this._tradingHoursService,
                this._operationRunRuleContext,
                this._universeDataRequestsSubscriber,
                "test",
                this._logger);

            var result = highMarketCapFilter.Filter(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Filter_WhenUniverseEventUnderlyingEventIsNotOrder_MustNotBeFiltered()
        {
            var marketCapRangeRuleFilter = DecimalRangeRuleFilter.None();

            var highMarketCapFilter = new HighMarketCapFilter(
                this._universeMarketCacheFactory,
                RuleRunMode.ValidationRun,
                marketCapRangeRuleFilter,
                this._tradingHoursService,
                this._operationRunRuleContext,
                this._universeDataRequestsSubscriber,
                "test",
                this._logger);

            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, new { });

            var result = highMarketCapFilter.Filter(eventOne);

            Assert.IsFalse(result);
        }

        [SetUp]
        public void SetUp()
        {
            this._universeMarketCacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            this._universeEquityInterDayCache = A.Fake<IUniverseEquityInterDayCache>();
            this._universeDataRequestsSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();

            A.CallTo(() => this._universeMarketCacheFactory.BuildInterday(RuleRunMode.ValidationRun))
                .Returns(this._universeEquityInterDayCache);

            this._tradingHoursService = A.Fake<IMarketTradingHoursService>();
            this._operationRunRuleContext = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._logger = A.Fake<ILogger<HighMarketCapFilter>>();
        }
    }
}