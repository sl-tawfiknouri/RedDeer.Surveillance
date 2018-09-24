using FakeItEasy;
using NUnit.Framework;
using System;
using Domain.Equity;
using Domain.Trades.Orders;
using TestHarness.Display;
using TestHarness.Display.Interfaces;
using TestHarness.Display.Subscribers;

namespace TestHarness.Tests.Display.Subscribers
{
    [TestFixture]
    public class TradeOrderDisplaySubscriberTests
    {
        private IConsole _console;

        [SetUp]
        public void Setup()
        {
            _console = A.Fake<IConsole>();
        }

        [Test]
        public void Constructor_ConsidersNullConsole_ToBeExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new TradeOrderFrameDisplaySubscriber(null));
        }

        [Test]
        public void OnError_PassesException_OnToConsole()
        {
            var tradeOrderSubscriber = new TradeOrderFrameDisplaySubscriber(_console);
            var exception = new Exception();

            tradeOrderSubscriber.OnError(exception);

            A
                .CallTo(() => _console.OutputException(exception))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_PassesOrderFrame_OnToConsole()
        {
            var tradeOrderSubscriber = new TradeOrderFrameDisplaySubscriber(_console);
            var tradeOrderFrame = 
                new TradeOrderFrame(
                    OrderType.Market,
                    null,
                    null,
                    null,
                    new Price(10, "GBX"), 
                    10,
                    10,
                    OrderPosition.Buy,
                    OrderStatus.Cancelled,
                    DateTime.Now,
                    DateTime.Now,
                    "trader-id",
                    "trader-client-id",
                    "account-id",
                    "dealer-instruction",
                    "party-broker",
                    "counterParty-broker",
                    "trader rationale",
                    "trade strategy");

            tradeOrderSubscriber.OnNext(tradeOrderFrame);

            A
                .CallTo(() => _console.OutputTradeFrame(tradeOrderFrame))
                .MustHaveHappenedOnceExactly();
        }
    }
}
