using FakeItEasy;
using NUnit.Framework;
using System;
using DomainV2.Equity.Frames;
using DomainV2.Equity.Streams;
using DomainV2.Streams;
using Microsoft.Extensions.Logging;
using TestHarness.Engine.EquitiesGenerator;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.EquitiesGenerator.Strategies;
using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;
using TestHarness.Engine.Heartbeat;
using TestHarness.Engine.Heartbeat.Interfaces;

namespace TestHarness.Tests.Engine.EquitiesGenerator
{
    [TestFixture]
    public class EquitiesMarkovProcessTests
    {
        private ILogger _logger;
        private IExchangeSeriesInitialiser _exchangeTickInitialiser;
        private IEquityDataGeneratorStrategy _strategy;
        private IHeartbeat _heartbeat;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger>();
            _exchangeTickInitialiser = A.Fake<IExchangeSeriesInitialiser>();
            _strategy = A.Fake<IEquityDataGeneratorStrategy>();
            _heartbeat = new Heartbeat(TimeSpan.FromMilliseconds(500));
        }

        [Test]
        public void InitiateWalk_ThrowsExceptionFor_NullStream()
        {
            var randomWalk = new EquitiesMarkovProcess(_exchangeTickInitialiser, _strategy, _heartbeat, _logger);

            Assert.Throws<ArgumentNullException>(() => randomWalk.InitiateWalk(null));
        }

        [Test]
        public void InitiateWalk_ReceivesTicks_AfterInitiationImmediately()
        {
            var randomWalkStrategy = new MarkovEquityStrategy();
            var randomWalk = new EquitiesMarkovProcess(_exchangeTickInitialiser, randomWalkStrategy, _heartbeat, _logger);
            var stream = new StockExchangeStream(new UnsubscriberFactory<ExchangeFrame>());
            var observer = new RecordingObserver<ExchangeFrame>(_logger, 10);
            stream.Subscribe(observer);

            randomWalk.InitiateWalk(stream);

            Assert.AreEqual(observer.Buffer.Count, 1);

            randomWalk.TerminateWalk();
        }

        [Test]
        public void InitiateWalk_GeneratesSubsequentTicks_AsExpected()
        {
            var randomWalkStrategy = new MarkovEquityStrategy();
            var randomWalk = new EquitiesMarkovProcess(new NasdaqInitialiser(), randomWalkStrategy, _heartbeat, _logger);
            var stream = new StockExchangeStream(new UnsubscriberFactory<ExchangeFrame>());
            var observer = new RecordingObserver<ExchangeFrame>(_logger, 5);
            stream.Subscribe(observer);

            randomWalk.InitiateWalk(stream);

            var timeOut = DateTime.Now.AddSeconds(5);

            while (observer.Buffer.Count < 2 
                && DateTime.Now < timeOut)
            {
                // don't sleep the thread
            }

            Assert.AreEqual(observer.Buffer.Count, 2);

            randomWalk.TerminateWalk();
        }

        [Test]
        [Explicit]
        public void InitiateWalk_WaitThenTerminateWalk_EnsuresNoMoreTicksTocked()
        {
            var randomWalk = new EquitiesMarkovProcess(new NasdaqInitialiser(), _strategy, _heartbeat, _logger);
            var stream = new StockExchangeStream(new UnsubscriberFactory<ExchangeFrame>());
            var observer = new RecordingObserver<ExchangeFrame>(_logger, 5);
            stream.Subscribe(observer);

            randomWalk.InitiateWalk(stream);

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

            A.CallTo(() => _logger.LogInformation("Random walk generator terminating walk"))
                .MustHaveHappenedOnceExactly();
        }
    }
}