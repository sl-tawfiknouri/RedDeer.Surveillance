using System;
using System.Collections.Generic;
using DomainV2.Equity.Frames;
using DomainV2.Financial;
using DomainV2.Scheduling;
using DomainV2.Streams;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.Rules.Interfaces;
using Surveillance.Universe;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.MarketEvents;
using Surveillance.Universe.Multiverse;

namespace Surveillance.Tests.Universe.Multiverse
{
    [TestFixture]
    public class MarketCloseMultiverseTransformerTests
    {
        private IObserver<IUniverseEvent> _observer;

        [SetUp]
        public void Setup()
        {
            _observer = A.Fake<IObserver<IUniverseEvent>>();
        }

        [Test]
        public void Constructor_NullUnsubscriberFactory_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarketCloseMultiverseTransformer(null));
        }

        [Test]
        public void OnCompleted_PassesOnCompleted_ToSubscribers()
        {
            var transformer = new MarketCloseMultiverseTransformer(new UnsubscriberFactory<IUniverseEvent>());
            transformer.Subscribe(_observer);

            transformer.OnCompleted();

            A.CallTo(() => _observer.OnCompleted()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnError_PassesOnError_ToSubscribers()
        {
            var transformer = new MarketCloseMultiverseTransformer(new UnsubscriberFactory<IUniverseEvent>());
            transformer.Subscribe(_observer);

            transformer.OnError(new ArgumentNullException());

            A.CallTo(() => _observer.OnError(A<ArgumentNullException>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Subscribe_AddToSubscription_ThenUnsubscribes_OnUnSub()
        {
            var transformer = new MarketCloseMultiverseTransformer(new UnsubscriberFactory<IUniverseEvent>());
            var sub = transformer.Subscribe(_observer);
            transformer.OnCompleted();

            sub.Dispose();

            transformer.OnCompleted();
            A.CallTo(() => _observer.OnCompleted()).MustHaveHappenedOnceExactly();
        }

        [Test]
        [Explicit]
        public void Subscribe_WithObserverAndUniverseCloneableRule_CallsMostSpecificMethods()
        {
            var otherSub = A.Fake<IUniverseCloneableRule>();
            var transformer = new MarketCloseMultiverseTransformer(new UnsubscriberFactory<IUniverseEvent>());

            var sub = transformer.Subscribe(_observer);
            var sub2 = transformer.Subscribe(otherSub);
        }

        [Test]
        [Explicit]
        public void Clone_SetsUpNewCloneWithSomeSharedSomeClonedDependencies()
        {
            var otherSub = A.Fake<IUniverseCloneableRule>();
            var cloneSub = A.Fake<IUniverseCloneableRule>();
            A.CallTo(() => otherSub.Clone()).Returns(cloneSub);
            var transformer = new MarketCloseMultiverseTransformer(new UnsubscriberFactory<IUniverseEvent>());

            var sub = transformer.Subscribe(_observer);
            var sub2 = transformer.Subscribe(otherSub);

            var transformerClone = (MarketCloseMultiverseTransformer)transformer.Clone();
        }

        [Test]
        public void OnNext_OverWritesMarketDataForDay_WithLastTickForDay()
        {
            var genesisDate = new DateTime(2018, 01, 01);

            A
                .CallTo(() => _observer.OnNext(A<IUniverseEvent>.Ignored))
                .Invokes(a =>
                {
                    if (((IUniverseEvent)a.Arguments[0]).StateChange == UniverseStateEvent.StockTickReddeer)
                    {
                        Console.WriteLine(((ExchangeFrame) ((IUniverseEvent) a.Arguments[0]).UnderlyingEvent).Exchange
                            .Name);
                    }
                });

            var transformer = new MarketCloseMultiverseTransformer(new UnsubscriberFactory<IUniverseEvent>());
            transformer.Subscribe(_observer);

            var initial = Genesis(genesisDate);
            var day1Open = MarketOpen(genesisDate, 0);
            var day1Close = MarketClose(genesisDate, 0);

            var day2Open = MarketOpen(genesisDate, 1);

            var tick20 = Tick(genesisDate, 1, 2, "day 2-1");
            var tick21 = Tick(genesisDate, 1, 4, "day 2-2");
            var tick22 = Tick(genesisDate, 1, 6, "day 2-3");
            var day2Close = MarketClose(genesisDate, 1);
            var tick23 = Tick(genesisDate, 1, 8, "day 2-4");

            var day3Open = MarketOpen(genesisDate, 2);
            var tick30 = Tick(genesisDate, 2, 2, "day 3-1");
            var tick31 = Tick(genesisDate, 2, 4, "day 3-2");
            var tick32 = Tick(genesisDate, 2, 6, "day 3-3");
            var tick33 = Tick(genesisDate, 2, 12, "day 3-4");
            var day3Close = MarketClose(genesisDate, 2);

            var day4Open = MarketOpen(genesisDate, 3);
            var tick40 = Tick(genesisDate, 3, 2, "day 4-1");
            var tick41 = Tick(genesisDate, 3, 4, "day 4-2");
            var tick42 = Tick(genesisDate, 3, 6, "day 4-3");
            var tick43 = Tick(genesisDate, 3, 19, "day 4-4");
            var day4Close = MarketClose(genesisDate, 3);

            var final = Eschaton(genesisDate, 4);

            transformer.OnNext(initial);

            transformer.OnNext(day1Open);
            transformer.OnNext(day1Close);

            transformer.OnNext(day2Open);

            transformer.OnNext(tick20);
            transformer.OnNext(tick21);
            transformer.OnNext(tick22);
            transformer.OnNext(tick23);

            transformer.OnNext(day2Close);

            transformer.OnNext(day3Open);
            transformer.OnNext(tick30);
            transformer.OnNext(tick31);
            transformer.OnNext(tick32);
            transformer.OnNext(tick33);
            transformer.OnNext(day3Close);

            transformer.OnNext(day4Open);
            transformer.OnNext(tick40);
            transformer.OnNext(tick41);
            transformer.OnNext(tick42);
            transformer.OnNext(tick43);
            transformer.OnNext(day4Close);

            transformer.OnNext(final);

            A.CallTo(() => _observer.OnNext(initial)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _observer.OnNext(day1Open)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _observer.OnNext(day1Close)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _observer.OnNext(day2Open)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _observer.OnNext(
                A<UniverseEvent>.That.Matches(m =>
                    m.StateChange == UniverseStateEvent.StockTickReddeer
                    && ((ExchangeFrame)m.UnderlyingEvent).Exchange.Name == "NASDAQ day 2-4")))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 4);
            A.CallTo(() => _observer.OnNext(day2Close)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _observer.OnNext(day3Open)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _observer.OnNext(
                    A<UniverseEvent>.That.Matches(m =>
                        m.StateChange == UniverseStateEvent.StockTickReddeer
                        && ((ExchangeFrame)m.UnderlyingEvent).Exchange.Name == "NASDAQ day 3-4")))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 4);
            A.CallTo(() => _observer.OnNext(day3Close)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _observer.OnNext(day4Open)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _observer.OnNext(
                    A<UniverseEvent>.That.Matches(m =>
                        m.StateChange == UniverseStateEvent.StockTickReddeer
                        && ((ExchangeFrame)m.UnderlyingEvent).Exchange.Name == "NASDAQ day 4-4")))
                .MustHaveHappenedANumberOfTimesMatching(i => i == 4);
            A.CallTo(() => _observer.OnNext(day4Close)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _observer.OnNext(final)).MustHaveHappenedOnceExactly();
        }

        private IUniverseEvent Tick(DateTime genesis, int day, int hour, string id)
        {
            var tick = new ExchangeFrame(
                new Market("1", "NASDAQ", $"NASDAQ {id}", MarketTypes.STOCKEXCHANGE),
                genesis.AddDays(day).AddHours(hour),
                new List<SecurityTick>());

            return new UniverseEvent(UniverseStateEvent.StockTickReddeer, genesis.AddDays(day).AddHours(hour), (object)tick);
        }

        private IUniverseEvent MarketOpen(DateTime genesis, int day)
        {
            var market = new MarketOpenClose("NASDAQ", genesis.AddDays(day), genesis.AddDays(day).AddHours(8));
            return new UniverseEvent(UniverseStateEvent.StockMarketOpen, genesis.AddDays(day), market);
        }

        private IUniverseEvent MarketClose(DateTime genesis, int day)
        {
            var market = new MarketOpenClose("NASDAQ", genesis.AddDays(day), genesis.AddDays(day).AddHours(8));
            return new UniverseEvent(UniverseStateEvent.StockMarketClose, genesis.AddDays(day).AddHours(8), market);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private IUniverseEvent Genesis(DateTime genesis)
        {
            var underlyingEvent = new ScheduledExecution();
            return new UniverseEvent(UniverseStateEvent.Genesis, genesis, underlyingEvent);
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private IUniverseEvent Eschaton(DateTime genesis, int days)
        {
            var underlyingEvent = new ScheduledExecution();
            return new UniverseEvent(UniverseStateEvent.Eschaton, genesis.AddDays(days), underlyingEvent);
        }
    }
}
