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
using Surveillance.Engine.Rules.Universe.Filter;
using System;

namespace Surveillance.Engine.Rules.Tests.Universe.Filter
{
    using System.Collections.Generic;

    using Domain.Core.Financial.Money;

    using Surveillance.Data.Universe;
    using Surveillance.Engine.Rules.Currency.Interfaces;

    using TestHelpers;

    public class HighMarketCapFilterTests
    {
        private ICurrencyConverterService currencyConverterService;
        private IUniverseMarketCacheFactory _universeMarketCacheFactory;
        private IUniverseEquityInterDayCache _universeEquityInterDayCache;
        private IMarketTradingHoursService _tradingHoursService;
        private ISystemProcessOperationRunRuleContext _operationRunRuleContext;
        private IUniverseDataRequestsSubscriber _universeDataRequestsSubscriber;
        private ILogger<HighMarketCapFilter> _logger;

        [SetUp]
        public void SetUp()
        {
            this.currencyConverterService = A.Fake<ICurrencyConverterService>();
            _universeMarketCacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            _universeEquityInterDayCache = A.Fake<IUniverseEquityInterDayCache>();
            _universeDataRequestsSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();

            A.CallTo(() => _universeMarketCacheFactory.BuildInterday(Engine.Rules.Rules.RuleRunMode.ValidationRun))
                .Returns(_universeEquityInterDayCache);

            _tradingHoursService = A.Fake<IMarketTradingHoursService>();
            _operationRunRuleContext = A.Fake<ISystemProcessOperationRunRuleContext>();
            _logger = A.Fake<ILogger<HighMarketCapFilter>>();
        }

        [Test]
        public void Filter_WhenUniverseEventNull_MustNotBeFiltered()
        {
            var marketCapRangeRuleFilter = DecimalRangeRuleFilter.None();

            var highMarketCapFilter = new HighMarketCapFilter(
                _universeMarketCacheFactory,
                Engine.Rules.Rules.RuleRunMode.ValidationRun,
                marketCapRangeRuleFilter,
                _tradingHoursService,
                _operationRunRuleContext,
                _universeDataRequestsSubscriber,
                this.currencyConverterService,
                "test",
                _logger);

            var result = highMarketCapFilter.Filter(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Filter_WhenUniverseEventUnderlyingEventIsNotOrder_MustNotBeFiltered()
        {
            var marketCapRangeRuleFilter = DecimalRangeRuleFilter.None();

            var highMarketCapFilter = new HighMarketCapFilter(
                _universeMarketCacheFactory,
                Engine.Rules.Rules.RuleRunMode.ValidationRun,
                marketCapRangeRuleFilter,
                _tradingHoursService,
                _operationRunRuleContext,
                _universeDataRequestsSubscriber,
                this.currencyConverterService,
                "test",
                _logger);

            
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, new { });

            var result = highMarketCapFilter.Filter(eventOne);

            Assert.IsFalse(result);
        }

        [Test]
        public void Filter_WhenUniverseEventNoneMarketCapFilter_MustNotBeFiltered()
        {
            var marketCapRangeRuleFilter = DecimalRangeRuleFilter.None();

            var highMarketCapFilter = new HighMarketCapFilter(
                _universeMarketCacheFactory,
                Engine.Rules.Rules.RuleRunMode.ValidationRun,
                marketCapRangeRuleFilter,
                _tradingHoursService,
                _operationRunRuleContext,
                _universeDataRequestsSubscriber,
                this.currencyConverterService,
                "test",
                _logger);


            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, new { });

            var result = highMarketCapFilter.Filter(eventOne);

            Assert.IsFalse(result);
        }

        [Test]
        public void Filter_WhenUniverseEventExcludeMarketCapFilter_MustNotBeFiltered()
        {
            var marketCapRangeRuleFilter = new DecimalRangeRuleFilter
            {
                Type = RuleFilterType.Exclude
            };

            var highMarketCapFilter = new HighMarketCapFilter(
                _universeMarketCacheFactory,
                Engine.Rules.Rules.RuleRunMode.ValidationRun,
                marketCapRangeRuleFilter,
                _tradingHoursService,
                _operationRunRuleContext,
                _universeDataRequestsSubscriber,
                this.currencyConverterService,
                "test",
                _logger);


            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, new { });

            var result = highMarketCapFilter.Filter(eventOne);

            Assert.IsFalse(result);
        }

        [Test]
        public void Filter_WhenUniverseEventAndTradingHoursIsNotValid_MustBeFiltered()
        {
            var fundOne = ((Order)null).Random();
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            A.CallTo(() => _tradingHoursService.GetTradingHoursForMic(fundOne.Market.MarketIdentifierCode))
                .Returns(new TradingHours() { IsValid = false });

            var marketCapRangeRuleFilter = new DecimalRangeRuleFilter
            {
                Type = RuleFilterType.Include
            };

            var highMarketCapFilter = new HighMarketCapFilter(
                _universeMarketCacheFactory,
                Engine.Rules.Rules.RuleRunMode.ValidationRun,
                marketCapRangeRuleFilter,
                _tradingHoursService,
                _operationRunRuleContext,
                _universeDataRequestsSubscriber,
                this.currencyConverterService,
                "test",
                _logger);

            var result = highMarketCapFilter.Filter(eventOne);

            Assert.IsTrue(result);
        }

        [Test]
        public void Filter_WhenUniverseEventAndHadMissingDataInUniverseEquityInterdayCache_MustBeFiltered()
        {
            var fundOne = ((Order)null).Random();
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            A.CallTo(() => _tradingHoursService.GetTradingHoursForMic(fundOne.Market.MarketIdentifierCode))
                .Returns(new TradingHours() { IsValid = true });

            A.CallTo(() => _universeEquityInterDayCache.Get(
                    A<MarketDataRequest>.That.Matches(
                        m => m.MarketIdentifierCode == fundOne.Market.MarketIdentifierCode &&
                            m.Cfi == fundOne.Instrument.Cfi
                    )))
                .Returns(MarketDataResponse<EquityInstrumentInterDayTimeBar>.MissingData());

            var marketCapRangeRuleFilter = new DecimalRangeRuleFilter
            {
                Type = RuleFilterType.Include
            };

            var highMarketCapFilter = new HighMarketCapFilter(
                _universeMarketCacheFactory,
                Engine.Rules.Rules.RuleRunMode.ValidationRun,
                marketCapRangeRuleFilter,
                _tradingHoursService,
                _operationRunRuleContext,
                _universeDataRequestsSubscriber,
                this.currencyConverterService,
                "test",
                _logger);

            var result = highMarketCapFilter.Filter(eventOne);

            Assert.IsTrue(result);
        }

        [TestCase(150, null, null, false)]
        [TestCase(150, 100, 200, false)]
        [TestCase(50, 100, 200, true)]
        [TestCase(250, 100, 200, true)]
        public void Filter_WhenUniverseEventAndMarketCapFilter_MustFilterCorrectly(
            decimal? marketCap,
            decimal? min,
            decimal? max,
            bool mustBeFiltered)
        {
            A.CallTo(
                () => this.currencyConverterService.Convert(
                    A<IReadOnlyCollection<Money>>.Ignored,
                    A<Currency>.Ignored,
                    A<DateTime>.Ignored,
                    A<ISystemProcessOperationRunRuleContext>.Ignored)).Returns(new Money(marketCap.Value, "GBP"));

            var fundOne = ((Order)null).Random();
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            A.CallTo(() => _tradingHoursService.GetTradingHoursForMic(fundOne.Market.MarketIdentifierCode))
                .Returns(new TradingHours() { IsValid = true });

            //decimal? marketCap = 150;
            var marketDataResponse = new MarketDataResponse<EquityInstrumentInterDayTimeBar>(
                new EquityInstrumentInterDayTimeBar(null, new DailySummaryTimeBar(marketCap, "GBP", null, null, new Volume(), DateTime.UtcNow), DateTime.UtcNow, null), false, false);

            A.CallTo(() => _universeEquityInterDayCache.Get(
                    A<MarketDataRequest>.That.Matches(
                        m => m.MarketIdentifierCode == fundOne.Market.MarketIdentifierCode &&
                            m.Cfi == fundOne.Instrument.Cfi
                    )))
                .Returns(marketDataResponse);

            var marketCapRangeRuleFilter = new DecimalRangeRuleFilter
            {
                Type = RuleFilterType.Include,
                Min = min,
                Max = max
            };

            var highMarketCapFilter = new HighMarketCapFilter(
                _universeMarketCacheFactory,
                Engine.Rules.Rules.RuleRunMode.ValidationRun,
                marketCapRangeRuleFilter,
                _tradingHoursService,
                _operationRunRuleContext,
                _universeDataRequestsSubscriber,
                this.currencyConverterService,
                "test",
                _logger);

            var result = highMarketCapFilter.Filter(eventOne);

            Assert.AreEqual(result, mustBeFiltered);
        }
    }
}
