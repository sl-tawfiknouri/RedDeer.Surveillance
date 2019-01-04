using FakeItEasy;
using NUnit.Framework;
using System;
using DomainV2.Equity.TimeBars;
using TestHarness.Display.Interfaces;
using TestHarness.Display.Subscribers;

namespace TestHarness.Tests.Display.Subscribers
{
    [TestFixture]
    public class ExchangeFrameDisplaySubscriberTests
    {
        private IConsole _console;

        [SetUp]
        public void Setup()
        {
            _console = A.Fake<IConsole>();
        }

        [Test]
        public void Constructor_NullConsole_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new ExchangeFrameDisplaySubscriber(null));
        }

        [Test]
        public void OnError_PassesException_ToConsole()
        {
            var subscriber = new ExchangeFrameDisplaySubscriber(_console);
            var exception = new Exception();

            subscriber.OnError(exception);

            A
                .CallTo(() => _console.OutputException(exception))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void OnNext_PassesFrame_ToConsole()
        {
            var subscriber = new ExchangeFrameDisplaySubscriber(_console);
            var frame = new MarketTimeBarCollection(null, DateTime.UtcNow, null);

            subscriber.OnNext(frame);

            A
                .CallTo(() => _console.OutputMarketFrame(frame))
                .MustHaveHappenedOnceExactly();
        }
    }
}
