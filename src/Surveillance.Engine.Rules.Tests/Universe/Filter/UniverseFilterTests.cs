namespace Surveillance.Engine.Rules.Tests.Universe.Filter
{
    using System;

    using Domain.Core.Markets;
    using Domain.Core.Markets.Collections;
    using Domain.Core.Markets.Timebars;
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Streams.Interfaces;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Data.Universe;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Universe.Filter;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

    using TestHelpers;

    [TestFixture]
    public class UniverseFilterTests
    {
        private IHighMarketCapFilter _highMarketCapFilter;

        private ILogger<UniverseFilterService> _logger;

        private IUniverseCloneableRule _observer;

        private IUnsubscriberFactory<IUniverseEvent> _unsubscriber;

        [Test]
        public void Constructor_ConsidersNullUnsubscriber_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new UniverseFilterService(
                    null,
                    this._highMarketCapFilter,
                    new RuleFilter(),
                    new RuleFilter(),
                    new RuleFilter(),
                    new RuleFilter(),
                    new RuleFilter(),
                    new RuleFilter(),
                    new RuleFilter(),
                    new RuleFilter(),
                    new RuleFilter(),
                    this._logger));
        }

        [Test]
        public void OnCompleted_CallsOnCompleted_ForSubscribers()
        {
            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                this._logger);
            filter.Subscribe(this._observer);

            filter.OnCompleted();

            A.CallTo(() => this._observer.OnCompleted()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnError_CallsOnCompleted_ForSubscribers()
        {
            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                this._logger);
            filter.Subscribe(this._observer);

            filter.OnError(new ArgumentNullException());

            A.CallTo(() => this._observer.OnError(A<Exception>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForBlackListTrueSubscribers_Accounts()
        {
            var account = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Exclude };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                account,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var accOne = ((Order)null).Random();
            accOne.OrderClientAccountAttributionId = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accOne);

            var accTwo = ((Order)null).Random();
            accTwo.OrderClientAccountAttributionId = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustNotHaveHappened();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForBlackListTrueSubscribers_Countries()
        {
            var country = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Exclude };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                country,
                this._logger);

            filter.Subscribe(this._observer);

            var strategyOne = ((Order)null).Random();
            strategyOne.Instrument.CountryCode = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyOne);

            var strategyTwo = ((Order)null).Random();
            strategyTwo.Instrument.CountryCode = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustNotHaveHappened();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForBlackListTrueSubscribers_Funds()
        {
            var fund = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Exclude };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                fund,
                null,
                null,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var fundOne = ((Order)null).Random();
            fundOne.OrderFund = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            var fundTwo = ((Order)null).Random();
            fundTwo.OrderFund = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustNotHaveHappened();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForBlackListTrueSubscribers_HighMarketCapFilter()
        {
            var fundOne = ((Order)null).Random();
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            A.CallTo(() => this._highMarketCapFilter.Filter(eventOne)).Returns(true);

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            filter.OnNext(eventOne);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForBlackListTrueSubscribers_Industries()
        {
            var industry = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Exclude };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                null,
                industry,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var strategyOne = ((Order)null).Random();
            strategyOne.Instrument.IndustryCode = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyOne);

            var strategyTwo = ((Order)null).Random();
            strategyTwo.Instrument.IndustryCode = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustNotHaveHappened();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForBlackListTrueSubscribers_Markets()
        {
            var markets = new RuleFilter { Ids = new[] { "abc", "ghi" }, Type = RuleFilterType.Exclude };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                markets,
                null,
                null,
                null,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var accOne = ((Order)null).Random();
            accOne.Market = new Market("1", "abc", "abc", MarketTypes.STOCKEXCHANGE);
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accOne);

            var accTwo = ((Order)null).Random();
            accTwo.Market = new Market("1", "def", "def", MarketTypes.STOCKEXCHANGE);
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accTwo);

            var exchangeOne = new EquityIntraDayTimeBarCollection(
                new Market("1", "ghi", "ghi", MarketTypes.STOCKEXCHANGE),
                DateTime.UtcNow,
                new EquityInstrumentIntraDayTimeBar[0]);
            var eventThree = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, DateTime.UtcNow, exchangeOne);

            var exchangeTwo = new EquityIntraDayTimeBarCollection(
                new Market("1", "jkl", "jkl", MarketTypes.STOCKEXCHANGE),
                DateTime.UtcNow,
                new EquityInstrumentIntraDayTimeBar[0]);
            var eventFour = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, DateTime.UtcNow, exchangeTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);
            filter.OnNext(eventThree);
            filter.OnNext(eventFour);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustNotHaveHappened();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventThree)).MustNotHaveHappened();
            A.CallTo(() => this._observer.OnNext(eventFour)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForBlackListTrueSubscribers_Regions()
        {
            var region = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Exclude };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                region,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var strategyOne = ((Order)null).Random();
            strategyOne.Instrument.RegionCode = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyOne);

            var strategyTwo = ((Order)null).Random();
            strategyTwo.Instrument.RegionCode = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustNotHaveHappened();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForBlackListTrueSubscribers_Sectors()
        {
            var sector = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Exclude };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                sector,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var strategyOne = ((Order)null).Random();
            strategyOne.Instrument.SectorCode = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyOne);

            var strategyTwo = ((Order)null).Random();
            strategyTwo.Instrument.SectorCode = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustNotHaveHappened();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForBlackListTrueSubscribers_Strategies()
        {
            var strategy = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Exclude };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                strategy,
                null,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var strategyOne = ((Order)null).Random();
            strategyOne.OrderStrategy = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyOne);

            var strategyTwo = ((Order)null).Random();
            strategyTwo.OrderStrategy = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustNotHaveHappened();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForBlackListTrueSubscribers_Traders()
        {
            var traders = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Exclude };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                traders,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var accOne = ((Order)null).Random();
            accOne.OrderTraderId = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accOne);

            var accTwo = ((Order)null).Random();
            accTwo.OrderTraderId = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustNotHaveHappened();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForNoneTrueSubscribers_Accounts()
        {
            var account = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.None };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                account,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var accOne = ((Order)null).Random();
            accOne.OrderClientAccountAttributionId = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accOne);

            var accTwo = ((Order)null).Random();
            accTwo.OrderClientAccountAttributionId = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForNoneTrueSubscribers_Countries()
        {
            var country = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.None };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                country,
                this._logger);

            filter.Subscribe(this._observer);

            var fundOne = ((Order)null).Random();
            fundOne.Instrument.CountryCode = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            var fundTwo = ((Order)null).Random();
            fundTwo.Instrument.CountryCode = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForNoneTrueSubscribers_Funds()
        {
            var fund = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.None };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                fund,
                null,
                null,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var fundOne = ((Order)null).Random();
            fundOne.OrderFund = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            var fundTwo = ((Order)null).Random();
            fundTwo.OrderFund = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForNoneTrueSubscribers_Industries()
        {
            var industry = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.None };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                null,
                industry,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var fundOne = ((Order)null).Random();
            fundOne.Instrument.IndustryCode = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            var fundTwo = ((Order)null).Random();
            fundTwo.Instrument.IndustryCode = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForNoneTrueSubscribers_Regions()
        {
            var region = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.None };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                region,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var fundOne = ((Order)null).Random();
            fundOne.Instrument.RegionCode = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            var fundTwo = ((Order)null).Random();
            fundTwo.Instrument.RegionCode = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForNoneTrueSubscribers_Sectors()
        {
            var sector = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.None };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                sector,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var fundOne = ((Order)null).Random();
            fundOne.Instrument.SectorCode = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            var fundTwo = ((Order)null).Random();
            fundTwo.Instrument.SectorCode = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForNoneTrueSubscribers_Strategies()
        {
            var strategy = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.None };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                strategy,
                null,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var fundOne = ((Order)null).Random();
            fundOne.OrderStrategy = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            var fundTwo = ((Order)null).Random();
            fundTwo.OrderStrategy = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForSubscribers()
        {
            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                this._logger);
            filter.Subscribe(this._observer);

            filter.OnNext(new UniverseEvent(UniverseStateEvent.Genesis, DateTime.UtcNow, new object()));

            A.CallTo(() => this._observer.OnNext(A<IUniverseEvent>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForWhiteListTrueSubscribers_Accounts()
        {
            var account = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Include };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                account,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var accOne = ((Order)null).Random();
            accOne.OrderClientAccountAttributionId = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accOne);

            var accTwo = ((Order)null).Random();
            accTwo.OrderClientAccountAttributionId = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForWhiteListTrueSubscribers_Countries()
        {
            var country = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Include };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                country,
                this._logger);

            filter.Subscribe(this._observer);

            var strategyOne = ((Order)null).Random();
            strategyOne.Instrument.CountryCode = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyOne);

            var strategyTwo = ((Order)null).Random();
            strategyTwo.Instrument.CountryCode = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForWhiteListTrueSubscribers_Funds()
        {
            var fund = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Include };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                fund,
                null,
                null,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var fundOne = ((Order)null).Random();
            fundOne.OrderFund = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            var fundTwo = ((Order)null).Random();
            fundTwo.OrderFund = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForWhiteListTrueSubscribers_HighMarketCapFilter()
        {
            var fundOne = ((Order)null).Random();
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, fundOne);

            A.CallTo(() => this._highMarketCapFilter.Filter(eventOne)).Returns(false);

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            filter.OnNext(eventOne);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForWhiteListTrueSubscribers_Industries()
        {
            var industry = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Include };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                null,
                industry,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var strategyOne = ((Order)null).Random();
            strategyOne.Instrument.IndustryCode = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyOne);

            var strategyTwo = ((Order)null).Random();
            strategyTwo.Instrument.IndustryCode = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForWhiteListTrueSubscribers_Markets()
        {
            var markets = new RuleFilter { Ids = new[] { "abc", "ghi" }, Type = RuleFilterType.Include };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                markets,
                null,
                null,
                null,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var accOne = ((Order)null).Random();
            accOne.Market = new Market("1", "abc", "abc", MarketTypes.STOCKEXCHANGE);
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accOne);

            var accTwo = ((Order)null).Random();
            accTwo.Market = new Market("1", "def", "def", MarketTypes.STOCKEXCHANGE);
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accTwo);

            var exchangeOne = new EquityIntraDayTimeBarCollection(
                new Market("1", "ghi", "ghi", MarketTypes.STOCKEXCHANGE),
                DateTime.UtcNow,
                new EquityInstrumentIntraDayTimeBar[0]);
            var eventThree = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, DateTime.UtcNow, exchangeOne);

            var exchangeTwo = new EquityIntraDayTimeBarCollection(
                new Market("1", "jkl", "jkl", MarketTypes.STOCKEXCHANGE),
                DateTime.UtcNow,
                new EquityInstrumentIntraDayTimeBar[0]);
            var eventFour = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, DateTime.UtcNow, exchangeTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);
            filter.OnNext(eventThree);
            filter.OnNext(eventFour);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustNotHaveHappened();
            A.CallTo(() => this._observer.OnNext(eventThree)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventFour)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForWhiteListTrueSubscribers_Regions()
        {
            var region = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Include };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                region,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var strategyOne = ((Order)null).Random();
            strategyOne.Instrument.RegionCode = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyOne);

            var strategyTwo = ((Order)null).Random();
            strategyTwo.Instrument.RegionCode = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForWhiteListTrueSubscribers_Sectors()
        {
            var sector = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Include };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                null,
                sector,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var strategyOne = ((Order)null).Random();
            strategyOne.Instrument.SectorCode = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyOne);

            var strategyTwo = ((Order)null).Random();
            strategyTwo.Instrument.SectorCode = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForWhiteListTrueSubscribers_Strategies()
        {
            var strategy = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Include };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                null,
                null,
                null,
                strategy,
                null,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var strategyOne = ((Order)null).Random();
            strategyOne.OrderStrategy = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyOne);

            var strategyTwo = ((Order)null).Random();
            strategyTwo.OrderStrategy = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, strategyTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForWhiteListTrueSubscribers_Traders()
        {
            var traders = new RuleFilter { Ids = new[] { "abc" }, Type = RuleFilterType.Include };

            var filter = new UniverseFilterService(
                this._unsubscriber,
                this._highMarketCapFilter,
                null,
                traders,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                this._logger);

            filter.Subscribe(this._observer);

            var accOne = ((Order)null).Random();
            accOne.OrderTraderId = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accOne);

            var accTwo = ((Order)null).Random();
            accTwo.OrderTraderId = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => this._observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._observer.OnNext(eventTwo)).MustNotHaveHappened();
        }

        [SetUp]
        public void Setup()
        {
            this._unsubscriber = A.Fake<IUnsubscriberFactory<IUniverseEvent>>();
            this._highMarketCapFilter = A.Fake<IHighMarketCapFilter>();
            this._observer = A.Fake<IUniverseCloneableRule>();
            this._logger = A.Fake<ILogger<UniverseFilterService>>();
        }
    }
}