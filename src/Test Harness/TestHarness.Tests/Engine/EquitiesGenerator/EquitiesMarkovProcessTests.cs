using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Streams;
using FakeItEasy;
using NLog;
using NUnit.Framework;
using System;
using TestHarness.Engine.EquitiesGenerator;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Strategies;
using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;

namespace TestHarness.Tests.Engine.EquitiesGenerator
{
    [TestFixture]
    public class EquitiesMarkovProcessTests
    {
        private ILogger _logger;
        private IExchangeSeriesInitialiser _exchangeTickInitialiser;
        private IEquityDataGeneratorStrategy _strategy;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger>();
            _exchangeTickInitialiser = A.Fake<IExchangeSeriesInitialiser>();
            _strategy = A.Fake<IEquityDataGeneratorStrategy>();
        }

        [Test]
        public void InitiateWalk_ThrowsExceptionFor_NullStream()
        {
            var randomWalk = new EquitiesMarkovProcess(_exchangeTickInitialiser, _strategy, _logger);
            var freq = TimeSpan.FromMilliseconds(500);

            Assert.Throws<ArgumentNullException>(() => randomWalk.InitiateWalk(null, freq));
        }

        [Test]
        public void InitiateWalk_ReceivesTicks_AfterInitiationImmediately()
        {
            var randomWalkStrategy = new MarkovEquityStrategy();
            var randomWalk = new EquitiesMarkovProcess(_exchangeTickInitialiser, randomWalkStrategy, _logger);
            var freq = TimeSpan.FromDays(1);
            var stream = new StockExchangeStream(new UnsubscriberFactory<ExchangeFrame>());
            var observer = new RecordingObserver<ExchangeFrame>(_logger, 10);
            stream.Subscribe(observer);

            randomWalk.InitiateWalk(stream, freq);

            Assert.AreEqual(observer.Buffer.Count, 1);
            A
                .CallTo(() => _logger.Log(LogLevel.Info, "Walk initiated in equity generator"))
                .MustHaveHappenedOnceExactly();

            randomWalk.TerminateWalk();
        }

        [Test]
        public void InitiateWalk_GeneratesSubequentTicks_AsExpected()
        {
            var randomWalkStrategy = new MarkovEquityStrategy();
            var randomWalk = new EquitiesMarkovProcess(new NasdaqInitialiser(), randomWalkStrategy, _logger);
            var freq = TimeSpan.FromMilliseconds(500);
            var stream = new StockExchangeStream(new UnsubscriberFactory<ExchangeFrame>());
            var observer = new RecordingObserver<ExchangeFrame>(_logger, 5);
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
                .CallTo(() => _logger.Log(LogLevel.Info, "Walk initiated in equity generator"))
                .MustHaveHappenedOnceExactly();

            randomWalk.TerminateWalk();
        }

        [Test]
        public void InitiateWalk_WaitThenTerminateWalk_EnsuresNoMoreTicksTocked()
        {
            var randomWalk = new EquitiesMarkovProcess(new NasdaqInitialiser(), _strategy, _logger);
            var freq = TimeSpan.FromMilliseconds(500);
            var stream = new StockExchangeStream(new UnsubscriberFactory<ExchangeFrame>());
            var observer = new RecordingObserver<ExchangeFrame>(_logger, 5);
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