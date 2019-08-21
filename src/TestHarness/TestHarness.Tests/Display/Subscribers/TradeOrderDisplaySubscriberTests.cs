namespace TestHarness.Tests.Display.Subscribers
{
    using System;

    using FakeItEasy;

    using NUnit.Framework;

    using TestHarness.Display.Interfaces;
    using TestHarness.Display.Subscribers;
    using TestHarness.Tests.Helpers;

    [TestFixture]
    public class TradeOrderDisplaySubscriberTests
    {
        private IConsole _console;

        [Test]
        public void Constructor_ConsidersNullConsole_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new TradeOrderFrameDisplaySubscriber(null));
        }

        [Test]
        public void OnError_PassesException_OnToConsole()
        {
            var tradeOrderSubscriber = new TradeOrderFrameDisplaySubscriber(this._console);
            var exception = new Exception();

            tradeOrderSubscriber.OnError(exception);

            A.CallTo(() => this._console.OutputException(exception)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_PassesOrderFrame_OnToConsole()
        {
            var tradeOrderSubscriber = new TradeOrderFrameDisplaySubscriber(this._console);

            var order = OrderHelper.GetOrder();

            tradeOrderSubscriber.OnNext(order);

            A.CallTo(() => this._console.OutputTradeFrame(order)).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._console = A.Fake<IConsole>();
        }
    }
}