using Domain.Equity.Trading;
using FakeItEasy;
using NLog;
using NUnit.Framework;
using System;
using TestHarness.EquitiesGenerator;

namespace TestHarness.Tests.EquitiesGenerator
{
    [TestFixture]
    public class RandomWalkGeneratorTests
    {
        private ILogger _logger;
        private IExchangeTickInitialiser _exchangeTickInitialiser;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger>();
            _exchangeTickInitialiser = A.Fake<IExchangeTickInitialiser>();
        }

        [Test]
        public void InitiateWalk_ThrowsExceptionFor_NullStream()
        {
            var randomWalk = new RandomWalkGenerator(_exchangeTickInitialiser, _logger);
            var freq = TimeSpan.FromMilliseconds(500);

            Assert.Throws<ArgumentNullException>(() => randomWalk.InitiateWalk(null, freq));
        }

        [Test]
        public void InitiateWalk_ReceivesTicks_AfterInitiationImmediately()
        {
            var randomWalk = new RandomWalkGenerator(_exchangeTickInitialiser, _logger);
            var freq = TimeSpan.FromDays(1);
            var stream = new StockExchangeStream(new UnsubscriberFactory());
            var observer = new RecordingObserver<ExchangeTick>(_logger, 10);
            stream.Subscribe(observer);

            randomWalk.InitiateWalk(stream, freq);

            Assert.AreEqual(observer.Buffer.Count, 1);
            A
                .CallTo(() => _logger.Log(LogLevel.Info, "Walk initiated in random walk generator"))
                .MustHaveHappenedOnceExactly();

            randomWalk.TerminateWalk();
        }

        [Test]
        public void InitiateWalk_GeneratesSubequentTicks_AsExpected()
        {
            var randomWalk = new RandomWalkGenerator(new NasdaqInitialiser(), _logger);
            var freq = TimeSpan.FromMilliseconds(500);
            var stream = new StockExchangeStream(new UnsubscriberFactory());
            var observer = new RecordingObserver<ExchangeTick>(_logger, 5);
            stream.Subscribe(observer);

            randomWalk.InitiateWalk(stream, freq);

            var timeOut = DateTime.Now.AddSeconds(5);

            while (observer.Buffer.Count < 2 
                && DateTime.Now < timeOut)
            {
                // don't sleep the thread
            }

            Assert.AreEqual(observer.Buffer.Count, 2);
            A
                .CallTo(() => _logger.Log(LogLevel.Info, "Walk initiated in random walk generator"))
                .MustHaveHappenedOnceExactly();

            randomWalk.TerminateWalk();
        }

        [Test]
        public void InitiateWalk_WaitThenTerminateWalk_EnsuresNoMoreTicksTocked()
        {
            var randomWalk = new RandomWalkGenerator(new NasdaqInitialiser(), _logger);
            var freq = TimeSpan.FromMilliseconds(500);
            var stream = new StockExchangeStream(new UnsubscriberFactory());
            var observer = new RecordingObserver<ExchangeTick>(_logger, 5);
            stream.Subscribe(observer);

            randomWalk.InitiateWalk(stream, freq);

            var timeOut = DateTime.Now.AddSeconds(5);

            while (observer.Buffer.Count < 2
                && DateTime.Now < timeOut)
            {
                // don't sleep the thread
            }

            randomWalk.TerminateWalk();

            Assert.AreEqual(observer.Buffer.Count, 2);

            var timerForStragglers = DateTime.Now.AddSeconds(2);

            while (DateTime.Now < timerForStragglers)
            {
                // don't sleep the thread
            }

            Assert.AreEqual(observer.Buffer.Count, 2);

            A.CallTo(() => _logger.Log(LogLevel.Info, "Random walk generator terminating walk"))
                .MustHaveHappenedOnceExactly();
        }
    }
}