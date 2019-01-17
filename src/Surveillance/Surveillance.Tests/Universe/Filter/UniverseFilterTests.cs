using System;
using DomainV2.Equity.Streams.Interfaces;
using DomainV2.Equity.TimeBars;
using DomainV2.Financial;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.RuleParameters.Filter;
using Surveillance.Tests.Helpers;
using Surveillance.Universe;
using Surveillance.Universe.Filter;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Tests.Universe.Filter
{
    [TestFixture]
    public class UniverseFilterTests
    {
        private IUnsubscriberFactory<IUniverseEvent> _unsubscriber;
        private IObserver<IUniverseEvent> _observer;
        private ILogger<UniverseFilter> _logger;

        [SetUp]
        public void Setup()
        {
            _unsubscriber = A.Fake<IUnsubscriberFactory<IUniverseEvent>>();
            _observer = A.Fake<IObserver<IUniverseEvent>>();
            _logger = A.Fake<ILogger<UniverseFilter>>();
        }

        [Test]
        public void Constructor_ConsidersNullUnsubscriber_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseFilter(null,
                new RuleFilter(),
                new RuleFilter(),
                new RuleFilter(),
                _logger));
        }

        [Test]
        public void OnCompleted_CallsOnCompleted_ForSubscribers()
        {
            var filter = new UniverseFilter(_unsubscriber, null, null, null, _logger);
            filter.Subscribe(_observer);

            filter.OnCompleted();

            A.CallTo(() => _observer.OnCompleted()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnError_CallsOnCompleted_ForSubscribers()
        {
            var filter = new UniverseFilter(_unsubscriber, null, null, null, _logger);
            filter.Subscribe(_observer);

            filter.OnError(new ArgumentNullException());

            A.CallTo(() => _observer.OnError(A<ArgumentNullException>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForSubscribers()
        {
            var filter = new UniverseFilter(_unsubscriber, null, null, null, _logger);
            filter.Subscribe(_observer);

            filter.OnNext(new UniverseEvent(UniverseStateEvent.Genesis, DateTime.UtcNow, new object()));

            A.CallTo(() => _observer.OnNext(A<IUniverseEvent>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForWhiteListTrueSubscribers_Accounts()
        {
            var account = new RuleFilter
            {
                Ids = new string[] {"abc"},
                Type = RuleFilterType.Include
            };

            var filter = new UniverseFilter(_unsubscriber, account, null, null, _logger);

            filter.Subscribe(_observer);

            var accOne = ((Order) null).Random();
            accOne.OrderClientAccountAttributionId = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accOne);

            var accTwo = ((Order)null).Random();
            accTwo.OrderClientAccountAttributionId = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => _observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _observer.OnNext(eventTwo)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForBlackListTrueSubscribers_Accounts()
        {
            var account = new RuleFilter
            {
                Ids = new string[] { "abc" },
                Type = RuleFilterType.Exclude
            };

            var filter = new UniverseFilter(_unsubscriber, account, null, null, _logger);

            filter.Subscribe(_observer);

            var accOne = ((Order)null).Random();
            accOne.OrderClientAccountAttributionId = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accOne);

            var accTwo = ((Order)null).Random();
            accTwo.OrderClientAccountAttributionId = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => _observer.OnNext(eventOne)).MustNotHaveHappened();
            A.CallTo(() => _observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForNoneTrueSubscribers_Accounts()
        {
            var account = new RuleFilter
            {
                Ids = new string[] { "abc" },
                Type = RuleFilterType.None
            };

            var filter = new UniverseFilter(_unsubscriber, account, null, null, _logger);

            filter.Subscribe(_observer);

            var accOne = ((Order)null).Random();
            accOne.OrderClientAccountAttributionId = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accOne);

            var accTwo = ((Order)null).Random();
            accTwo.OrderClientAccountAttributionId = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => _observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForWhiteListTrueSubscribers_Traders()
        {
            var traders = new RuleFilter
            {
                Ids = new string[] { "abc" },
                Type = RuleFilterType.Include
            };

            var filter = new UniverseFilter(_unsubscriber, null, traders, null, _logger);

            filter.Subscribe(_observer);

            var accOne = ((Order)null).Random();
            accOne.OrderTraderId = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accOne);

            var accTwo = ((Order)null).Random();
            accTwo.OrderTraderId = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => _observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _observer.OnNext(eventTwo)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForBlackListTrueSubscribers_Traders()
        {
            var traders = new RuleFilter
            {
                Ids = new string[] { "abc" },
                Type = RuleFilterType.Exclude
            };

            var filter = new UniverseFilter(_unsubscriber, null, traders, null, _logger);

            filter.Subscribe(_observer);

            var accOne = ((Order)null).Random();
            accOne.OrderTraderId = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accOne);

            var accTwo = ((Order)null).Random();
            accTwo.OrderTraderId = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);

            A.CallTo(() => _observer.OnNext(eventOne)).MustNotHaveHappened();
            A.CallTo(() => _observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForWhiteListTrueSubscribers_Markets()
        {
            var markets = new RuleFilter
            {
                Ids = new string[] { "abc", "ghi" },
                Type = RuleFilterType.Include
            };

            var filter = new UniverseFilter(_unsubscriber, null, null, markets, _logger);

            filter.Subscribe(_observer);

            var accOne = ((Order)null).Random();
            accOne.Market = new Market("1", "abc", "abc", MarketTypes.STOCKEXCHANGE);
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accOne);

            var accTwo = ((Order)null).Random();
            accTwo.Market = new Market("1", "def", "def", MarketTypes.STOCKEXCHANGE);
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accTwo);

            var exchangeOne = new MarketTimeBarCollection(new Market("1", "ghi", "ghi", MarketTypes.STOCKEXCHANGE), DateTime.UtcNow, new FinancialInstrumentTimeBar[0]);
            var eventThree = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, DateTime.UtcNow, exchangeOne);

            var exchangeTwo = new MarketTimeBarCollection(new Market("1", "jkl", "jkl", MarketTypes.STOCKEXCHANGE), DateTime.UtcNow, new FinancialInstrumentTimeBar[0]);
            var eventFour = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, DateTime.UtcNow, exchangeTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);
            filter.OnNext(eventThree);
            filter.OnNext(eventFour);

            A.CallTo(() => _observer.OnNext(eventOne)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _observer.OnNext(eventTwo)).MustNotHaveHappened();
            A.CallTo(() => _observer.OnNext(eventThree)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _observer.OnNext(eventFour)).MustNotHaveHappened();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForBlackListTrueSubscribers_Markets()
        {
            var markets = new RuleFilter
            {
                Ids = new string[] { "abc", "ghi" },
                Type = RuleFilterType.Exclude
            };

            var filter = new UniverseFilter(_unsubscriber, null, null, markets, _logger);

            filter.Subscribe(_observer);

            var accOne = ((Order)null).Random();
            accOne.Market = new Market("1", "abc", "abc", MarketTypes.STOCKEXCHANGE);
            var eventOne = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accOne);

            var accTwo = ((Order)null).Random();
            accTwo.Market = new Market("1", "def", "def", MarketTypes.STOCKEXCHANGE);
            var eventTwo = new UniverseEvent(UniverseStateEvent.Order, DateTime.UtcNow, accTwo);

            var exchangeOne = new MarketTimeBarCollection(new Market("1", "ghi", "ghi", MarketTypes.STOCKEXCHANGE), DateTime.UtcNow, new FinancialInstrumentTimeBar[0]);
            var eventThree = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, DateTime.UtcNow, exchangeOne);

            var exchangeTwo = new MarketTimeBarCollection(new Market("1", "jkl", "jkl", MarketTypes.STOCKEXCHANGE), DateTime.UtcNow, new FinancialInstrumentTimeBar[0]);
            var eventFour = new UniverseEvent(UniverseStateEvent.EquityIntradayTick, DateTime.UtcNow, exchangeTwo);

            filter.OnNext(eventOne);
            filter.OnNext(eventTwo);
            filter.OnNext(eventThree);
            filter.OnNext(eventFour);

            A.CallTo(() => _observer.OnNext(eventOne)).MustNotHaveHappened();
            A.CallTo(() => _observer.OnNext(eventTwo)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _observer.OnNext(eventThree)).MustNotHaveHappened();
            A.CallTo(() => _observer.OnNext(eventFour)).MustHaveHappenedOnceExactly();
        }
    }
}
