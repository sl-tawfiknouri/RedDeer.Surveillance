using Domain.Equity.Trading;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;

namespace Domain.Tests.Equity.Trading
{
    [TestFixture]
    public class StockExchangeStreamTests
    {
        private IUnsubscriberFactory _factory;

        [SetUp]
        public void Setup()
        {
            _factory = A.Fake<IUnsubscriberFactory>();
        }

        [Test]
        public void Subscribe_WithNullFactory_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StockExchangeStream(null));
        }

        [Test]
        public void Subscribe_WithNullObserver_ThrowsArgumentNull()
        {
            var stream = new StockExchangeStream(_factory);

            Assert.Throws<ArgumentNullException>(() => stream.Subscribe(null));
            A
                .CallTo(() => 
                    _factory.Create(
                        A<ConcurrentDictionary<IObserver<ExchangeTick>, IObserver<ExchangeTick>>>.Ignored, 
                        A<IObserver<ExchangeTick>>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public void Subscribe_WithValidObserver_CallsFactoryCreate()
        {
            var stream = new StockExchangeStream(_factory);
            var tick = A.Fake<IObserver<ExchangeTick>>();

            stream.Subscribe(tick);

            A
                .CallTo(() => 
                    _factory.Create(
                     A<ConcurrentDictionary<IObserver<ExchangeTick>, IObserver<ExchangeTick>>>.Ignored, A<IObserver<ExchangeTick>>.Ignored))
               .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Add_NullTick_DoesNothing()
        {
            var stream = new StockExchangeStream(_factory);
            var obs = A.Fake<IObserver<ExchangeTick>>();

            stream.Subscribe(obs);
            stream.Add(null);

            A.CallTo(() => obs.OnNext(A<ExchangeTick>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Add_Tick_CallsOnNextForAllObservers()
        {
            var stream = new StockExchangeStream(_factory);
            var obs1 = A.Fake<IObserver<ExchangeTick>>();
            var obs2 = A.Fake<IObserver<ExchangeTick>>();
            var tick1 = new ExchangeTick();
            var tick2 = new ExchangeTick();
            var tick3 = new ExchangeTick();

            stream.Subscribe(obs1);
            stream.Subscribe(obs2);

            stream.Add(tick1);
            stream.Add(tick2);
            stream.Add(tick3);

            A.CallTo(() => obs1.OnNext(tick1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => obs1.OnNext(tick2)).MustHaveHappenedOnceExactly();
            A.CallTo(() => obs1.OnNext(tick3)).MustHaveHappenedOnceExactly();
            A.CallTo(() => obs2.OnNext(tick1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => obs2.OnNext(tick2)).MustHaveHappenedOnceExactly();
            A.CallTo(() => obs2.OnNext(tick3)).MustHaveHappenedOnceExactly();
        }
    }
}
