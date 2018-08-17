using Domain.Equity.Market;
using Domain.Equity.Trading;
using Domain.Equity.Trading.Orders;
using Domain.Equity.Trading.Streams.Interfaces;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;

namespace Domain.Tests.Equity.Trading
{
    [TestFixture]
    public class TradeOrderStreamTests
    {
        private IUnsubscriberFactory<TradeOrder> _factory;
        private IObserver<TradeOrder> _tradeOrderObserver;

        [SetUp]
        public void Setup()
        {
            _factory = A.Fake<IUnsubscriberFactory<TradeOrder>>();
            _tradeOrderObserver = A.Fake<IObserver<TradeOrder>>();
        }

        [Test]
        public void Constructor_ThrowsExceptionForNull_UnsubscriberFactory()
        {
            Assert.Throws<ArgumentNullException>(() => new TradeOrderStream(null));
        }

        [Test]
        public void Subscribe_ThrowsArgumentNullException_ForNullObserver()
        {
            var tradeOrderStream = new TradeOrderStream(_factory);

            Assert.Throws<ArgumentNullException>(() => tradeOrderStream.Subscribe(null));
        }

        [Test]
        public void Subscribe_CallsUnsubscriberFactory_ReturnsUnsubscriber()
        {
            var tradeOrderStream = new TradeOrderStream(_factory);

            tradeOrderStream.Subscribe(_tradeOrderObserver);

            A
                .CallTo(() => _factory.Create(
                    A<ConcurrentDictionary<IObserver<TradeOrder>, IObserver<TradeOrder>>>.Ignored, 
                    A<IObserver<TradeOrder>>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Add_NullOrder_DoesNothing()
        {
            var tradeOrderStream = new TradeOrderStream(_factory);

            tradeOrderStream.Subscribe(_tradeOrderObserver);

            Assert.DoesNotThrow(() => tradeOrderStream.Add(null));
        }

        [Test]
        public void Add_OrderIsPassedOnto_Subscribers()
        {
            var stream = new TradeOrderStream(_factory);
            var obs1 = A.Fake<IObserver<TradeOrder>>();
            var obs2 = A.Fake<IObserver<TradeOrder>>();
            var exch = new StockExchange(new Market.MarketId("id"), "LSE");
            var order1 = new TradeOrder(
                OrderType.Limit,
                exch,
                new Domain.Equity.Security(
                    new Domain.Equity.Security.SecurityId("stan"),
                    "Standard Chartered",
                    "LSE"),
                new Price(100),
                1000,
                OrderDirection.Buy,
                OrderStatus.Placed);

            var order2 = new TradeOrder(
                OrderType.Market,
                exch,
                new Domain.Equity.Security(
                    new Domain.Equity.Security.SecurityId("stan"),
                    "Standard Chartered",
                    "LSE"),
                null,
                10,
                OrderDirection.Sell,
                OrderStatus.Fulfilled);

            var order3 = new TradeOrder(
                OrderType.Limit,
                exch,
                new Domain.Equity.Security(
                    new Domain.Equity.Security.SecurityId("stan"),
                    "Standard Chartered",
                    "LSE"),
                new Price(10),
                1000,
                OrderDirection.Sell,
                OrderStatus.Cancelled);

            stream.Subscribe(obs1);
            stream.Subscribe(obs2);

            stream.Add(order1);
            stream.Add(order2);
            stream.Add(order3);

            A.CallTo(() => obs1.OnNext(order1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => obs1.OnNext(order2)).MustHaveHappenedOnceExactly();
            A.CallTo(() => obs1.OnNext(order3)).MustHaveHappenedOnceExactly();
            A.CallTo(() => obs2.OnNext(order1)).MustHaveHappenedOnceExactly();
            A.CallTo(() => obs2.OnNext(order2)).MustHaveHappenedOnceExactly();
            A.CallTo(() => obs2.OnNext(order3)).MustHaveHappenedOnceExactly();
        }
    }
}
