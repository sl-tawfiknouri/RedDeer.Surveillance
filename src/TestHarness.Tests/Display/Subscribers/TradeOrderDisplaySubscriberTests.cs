using Domain.Equity.Trading.Orders;
using FakeItEasy;
using NUnit.Framework;
using System;
using TestHarness.Display;
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
                    10,
                    OrderDirection.Buy,
                    OrderStatus.Cancelled);

            tradeOrderSubscriber.OnNext(tradeOrderFrame);

            A
                .CallTo(() => _console.OutputTradeFrame(tradeOrderFrame))
                .MustHaveHappenedOnceExactly();
        }
    }
}
