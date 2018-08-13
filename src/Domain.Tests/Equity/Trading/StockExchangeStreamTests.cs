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
    }
}
