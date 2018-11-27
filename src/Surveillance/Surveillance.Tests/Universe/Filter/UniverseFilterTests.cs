using System;
using Domain.Equity.Frames;
using Domain.Equity.Streams.Interfaces;
using Domain.Market;
using Domain.Trades.Orders;
using FakeItEasy;
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

        [SetUp]
        public void Setup()
        {
            _unsubscriber = A.Fake<IUnsubscriberFactory<IUniverseEvent>>();
            _observer = A.Fake<IObserver<IUniverseEvent>>();
        }

        [Test]
        public void Constructor_ConsidersNullUnsubscriber_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new UniverseFilter(null,
                new RuleFilter(),
                new RuleFilter(),
                new RuleFilter()));
        }

        [Test]
        public void OnCompleted_CallsOnCompleted_ForSubscribers()
        {
            var filter = new UniverseFilter(_unsubscriber, null, null, null);
            filter.Subscribe(_observer);

            filter.OnCompleted();

            A.CallTo(() => _observer.OnCompleted()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnError_CallsOnCompleted_ForSubscribers()
        {
            var filter = new UniverseFilter(_unsubscriber, null, null, null);
            filter.Subscribe(_observer);

            filter.OnError(new ArgumentNullException());

            A.CallTo(() => _observer.OnError(A<ArgumentNullException>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_CallsOnCompleted_ForSubscribers()
        {
            var filter = new UniverseFilter(_unsubscriber, null, null, null);
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

            var filter = new UniverseFilter(_unsubscriber, account, null, null);

            filter.Subscribe(_observer);

            var accOne = ((TradeOrderFrame) null).Random();
            accOne.AccountId = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.UtcNow, accOne);

            var accTwo = ((TradeOrderFrame)null).Random();
            accTwo.AccountId = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.UtcNow, accTwo);

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

            var filter = new UniverseFilter(_unsubscriber, account, null, null);

            filter.Subscribe(_observer);

            var accOne = ((TradeOrderFrame)null).Random();
            accOne.AccountId = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.UtcNow, accOne);

            var accTwo = ((TradeOrderFrame)null).Random();
            accTwo.AccountId = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.UtcNow, accTwo);

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

            var filter = new UniverseFilter(_unsubscriber, account, null, null);

            filter.Subscribe(_observer);

            var accOne = ((TradeOrderFrame)null).Random();
            accOne.AccountId = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.UtcNow, accOne);

            var accTwo = ((TradeOrderFrame)null).Random();
            accTwo.AccountId = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.UtcNow, accTwo);

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

            var filter = new UniverseFilter(_unsubscriber, null, traders, null);

            filter.Subscribe(_observer);

            var accOne = ((TradeOrderFrame)null).Random();
            accOne.TraderId = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.UtcNow, accOne);

            var accTwo = ((TradeOrderFrame)null).Random();
            accTwo.TraderId = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.UtcNow, accTwo);

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

            var filter = new UniverseFilter(_unsubscriber, null, traders, null);

            filter.Subscribe(_observer);

            var accOne = ((TradeOrderFrame)null).Random();
            accOne.TraderId = "abc";
            var eventOne = new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.UtcNow, accOne);

            var accTwo = ((TradeOrderFrame)null).Random();
            accTwo.TraderId = "def";
            var eventTwo = new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.UtcNow, accTwo);

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

            var filter = new UniverseFilter(_unsubscriber, null, null, markets);

            filter.Subscribe(_observer);

            var accOne = ((TradeOrderFrame)null).Random();
            accOne.Market = new StockExchange(new Market.MarketId("abc"), "abc");
            var eventOne = new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.UtcNow, accOne);

            var accTwo = ((TradeOrderFrame)null).Random();
            accTwo.Market = new StockExchange(new Market.MarketId("def"), "def");
            var eventTwo = new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.UtcNow, accTwo);

            var exchangeOne = new ExchangeFrame(new StockExchange(new Market.MarketId("ghi"), "ghi"), DateTime.UtcNow, new SecurityTick[0]);
            var eventThree = new UniverseEvent(UniverseStateEvent.StockTickReddeer, DateTime.UtcNow, exchangeOne);

            var exchangeTwo = new ExchangeFrame(new StockExchange(new Market.MarketId("jkl"), "jkl"), DateTime.UtcNow, new SecurityTick[0]);
            var eventFour = new UniverseEvent(UniverseStateEvent.StockTickReddeer, DateTime.UtcNow, exchangeTwo);

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

            var filter = new UniverseFilter(_unsubscriber, null, null, markets);

            filter.Subscribe(_observer);

            var accOne = ((TradeOrderFrame)null).Random();
            accOne.Market = new StockExchange(new Market.MarketId("abc"), "abc");
            var eventOne = new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.UtcNow, accOne);

            var accTwo = ((TradeOrderFrame)null).Random();
            accTwo.Market = new StockExchange(new Market.MarketId("def"), "def");
            var eventTwo = new UniverseEvent(UniverseStateEvent.TradeReddeer, DateTime.UtcNow, accTwo);

            var exchangeOne = new ExchangeFrame(new StockExchange(new Market.MarketId("ghi"), "ghi"), DateTime.UtcNow, new SecurityTick[0]);
            var eventThree = new UniverseEvent(UniverseStateEvent.StockTickReddeer, DateTime.UtcNow, exchangeOne);

            var exchangeTwo = new ExchangeFrame(new StockExchange(new Market.MarketId("jkl"), "jkl"), DateTime.UtcNow, new SecurityTick[0]);
            var eventFour = new UniverseEvent(UniverseStateEvent.StockTickReddeer, DateTime.UtcNow, exchangeTwo);

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
