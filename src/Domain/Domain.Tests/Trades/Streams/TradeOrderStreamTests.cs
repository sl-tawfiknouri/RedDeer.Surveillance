using Domain.Market;
using FakeItEasy;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using Domain.Equity;
using Domain.Equity.Streams.Interfaces;
using Domain.Tests.Domain.Market;
using Domain.Trades.Orders;
using Domain.Trades.Streams;

namespace Domain.Tests.Trades.Streams
{
    [TestFixture]
    public class TradeOrderStreamTests
    {
        private IUnsubscriberFactory<TradeOrderFrame> _factory;
        private IObserver<TradeOrderFrame> _tradeOrderObserver;

        [SetUp]
        public void Setup()
        {
            _factory = A.Fake<IUnsubscriberFactory<TradeOrderFrame>>();
            _tradeOrderObserver = A.Fake<IObserver<TradeOrderFrame>>();
        }

        [Test]
        public void Constructor_ThrowsExceptionForNull_UnsubscriberFactory()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new TradeOrderStream<TradeOrderFrame>(null));
        }

        [Test]
        public void Subscribe_ThrowsArgumentNullException_ForNullObserver()
        {
            var tradeOrderStream = new TradeOrderStream<TradeOrderFrame>(_factory);

            // ReSharper Disable All 
            Assert.Throws<ArgumentNullException>(() => tradeOrderStream.Subscribe(null));
            // ReSharper Restore All
        }

        [Test]
        public void Subscribe_CallsUnsubscriberFactory_ReturnsUnsubscriber()
        {
            var tradeOrderStream = new TradeOrderStream<TradeOrderFrame>(_factory);

            tradeOrderStream.Subscribe(_tradeOrderObserver);

            A
                .CallTo(() => _factory.Create(
                    A<ConcurrentDictionary<IObserver<TradeOrderFrame>, IObserver<TradeOrderFrame>>>.Ignored, 
                    A<IObserver<TradeOrderFrame>>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Add_NullOrder_DoesNothing()
        {
            var tradeOrderStream = new TradeOrderStream<TradeOrderFrame>(_factory);

            tradeOrderStream.Subscribe(_tradeOrderObserver);

            Assert.DoesNotThrow(() => tradeOrderStream.Add(null));
        }

        [Test]
        public void Add_OrderIsPassedOnto_Subscribers()
        {
            var stream = new TradeOrderStream<TradeOrderFrame>(_factory);
            var obs1 = A.Fake<IObserver<TradeOrderFrame>>();
            var obs2 = A.Fake<IObserver<TradeOrderFrame>>();
            var exch = new StockExchange(new MarketId("id"), "LSE");
            var orderDates = DateTime.Now;
            const string traderId = "Trader Joe";
            const string partyBrokerId = "Broker-1";
            const string accountId = "Account-1";
            const string dealerInstruction = "Trade ASAP";
            const string tradeRationale = "Market is not pricing well";
            const string tradeStrategy = "Unknown";
            const string counterPartyBrokerId = "Broker-2";
            var securityIdentifiers = new SecurityIdentifiers("stan", "st12345", "sta123456789", "stan", "sta12345", "stan", "stan", "STAN");

            var security = new Security(
                securityIdentifiers,
                "Standard Chartered",
                "CFI",
                "Standard Chartered Bank");

            var order1 = new TradeOrderFrame(
                OrderType.Limit,
                exch,
                security,
                new Price(100, "GBX"),
                new Price(100, "GBX"),
                1000,
                1000,
                OrderPosition.Buy,
                OrderStatus.Booked,
                orderDates,
                orderDates,
                traderId,
                string.Empty,
                accountId,
                dealerInstruction,
                partyBrokerId,
                counterPartyBrokerId,
                tradeRationale,
                tradeStrategy,
                "GBX");

            var order2 = new TradeOrderFrame(
                OrderType.Market,
                exch,
                security,
                null,
                new Price(100, "GBX"),
                10,
                10,
                OrderPosition.Sell,
                OrderStatus.Fulfilled,
                orderDates,
                orderDates,
                traderId,
                string.Empty,
                accountId,
                dealerInstruction,
                partyBrokerId,
                counterPartyBrokerId,
                tradeRationale,
                tradeStrategy,
                "GBX");

            var order3 = new TradeOrderFrame(
                OrderType.Limit,
                exch,
                security,
                new Price(10, "GBX"),
                new Price(10, "GBX"),
                1000,
                1000,
                OrderPosition.Sell,
                OrderStatus.Cancelled,
                orderDates,
                orderDates,
                traderId,
                string.Empty,
                accountId,
                dealerInstruction,
                partyBrokerId,
                counterPartyBrokerId,
                tradeRationale,
                tradeStrategy,
                "GBX");

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
