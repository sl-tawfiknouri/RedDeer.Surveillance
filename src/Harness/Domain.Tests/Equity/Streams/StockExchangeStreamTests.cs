using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Streams.Interfaces;
using Domain.Market;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Domain.Tests.Equity.Streams
{
    [TestFixture]
    public class StockExchangeStreamTests
    {
        private IUnsubscriberFactory<ExchangeFrame> _factory;

        [SetUp]
        public void Setup()
        {
            _factory = A.Fake<IUnsubscriberFactory<ExchangeFrame>>();
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
                        A<ConcurrentDictionary<IObserver<ExchangeFrame>, IObserver<ExchangeFrame>>>.Ignored, 
                        A<IObserver<ExchangeFrame>>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public void Subscribe_WithValidObserver_CallsFactoryCreate()
        {
            var stream = new StockExchangeStream(_factory);
            var tick = A.Fake<IObserver<ExchangeFrame>>();

            stream.Subscribe(tick);

            A
                .CallTo(() => 
                    _factory.Create(
                     A<ConcurrentDictionary<IObserver<ExchangeFrame>, IObserver<ExchangeFrame>>>.Ignored, A<IObserver<ExchangeFrame>>.Ignored))
               .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Add_NullTick_DoesNothing()
        {
            var stream = new StockExchangeStream(_factory);
            var obs = A.Fake<IObserver<ExchangeFrame>>();

            stream.Subscribe(obs);
            stream.Add(null);

            A.CallTo(() => obs.OnNext(A<ExchangeFrame>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Add_Tick_CallsOnNextForAllObservers()
        {
            var stream = new StockExchangeStream(_factory);
            var obs1 = A.Fake<IObserver<ExchangeFrame>>();
            var obs2 = A.Fake<IObserver<ExchangeFrame>>();
            var exch = new StockExchange(new MarketId("id"), "LSE");
            var tick1 = new ExchangeFrame(exch, new List<SecurityFrame>());
            var tick2 = new ExchangeFrame(exch, new List<SecurityFrame>());
            var tick3 = new ExchangeFrame(exch, new List<SecurityFrame>());

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
