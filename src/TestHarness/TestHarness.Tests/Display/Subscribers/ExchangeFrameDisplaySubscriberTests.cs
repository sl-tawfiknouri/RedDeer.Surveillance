namespace TestHarness.Tests.Display.Subscribers
{
    using System;

    using Domain.Core.Markets.Collections;

    using FakeItEasy;

    using NUnit.Framework;

    using TestHarness.Display.Interfaces;
    using TestHarness.Display.Subscribers;

    [TestFixture]
    public class ExchangeFrameDisplaySubscriberTests
    {
        private IConsole _console;

        [Test]
        public void Constructor_NullConsole_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new ExchangeFrameDisplaySubscriber(null));
        }

        [Test]
        public void OnError_PassesException_ToConsole()
        {
            var subscriber = new ExchangeFrameDisplaySubscriber(this._console);
            var exception = new Exception();

            subscriber.OnError(exception);

            A.CallTo(() => this._console.OutputException(exception)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_PassesFrame_ToConsole()
        {
            var subscriber = new ExchangeFrameDisplaySubscriber(this._console);
            var frame = new EquityIntraDayTimeBarCollection(null, DateTime.UtcNow, null);

            subscriber.OnNext(frame);

            A.CallTo(() => this._console.OutputMarketFrame(frame)).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._console = A.Fake<IConsole>();
        }
    }
}