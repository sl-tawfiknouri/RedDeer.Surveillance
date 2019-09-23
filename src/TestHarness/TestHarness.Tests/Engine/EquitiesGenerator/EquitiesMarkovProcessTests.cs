namespace TestHarness.Tests.Engine.EquitiesGenerator
{
    using System;

    using Domain.Core.Markets.Collections;
    using Domain.Surveillance.Streams;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using TestHarness.Engine.EquitiesGenerator;
    using TestHarness.Engine.EquitiesGenerator.Interfaces;
    using TestHarness.Engine.EquitiesGenerator.Strategies;
    using TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces;
    using TestHarness.Engine.Heartbeat;
    using TestHarness.Engine.Heartbeat.Interfaces;

    [TestFixture]
    public class EquitiesMarkovProcessTests
    {
        private IExchangeSeriesInitialiser _exchangeTickInitialiser;

        private IHeartbeat _heartbeat;

        private ILogger _logger;

        private IEquityDataGeneratorStrategy _strategy;

        [Test]
        public void InitiateWalk_GeneratesSubsequentTicks_AsExpected()
        {
            var randomWalkStrategy = new MarkovEquityStrategy();
            var randomWalk = new EquitiesMarkovProcess(
                new NasdaqInitialiser(),
                randomWalkStrategy,
                this._heartbeat,
                this._logger);
            var stream = new ExchangeStream(new UnsubscriberFactory<EquityIntraDayTimeBarCollection>());
            var observer = new RecordingObserver<EquityIntraDayTimeBarCollection>(this._logger, 5);
            stream.Subscribe(observer);

            randomWalk.InitiateWalk(stream);

            var timeOut = DateTime.UtcNow.AddSeconds(5);

            while (observer.Buffer.Count < 2 && DateTime.UtcNow < timeOut)
            {
                // don't sleep the thread
            }

            Assert.AreEqual(observer.Buffer.Count, 2);

            randomWalk.TerminateWalk();
        }

        [Test]
        public void InitiateWalk_ReceivesTicks_AfterInitiationImmediately()
        {
            var randomWalkStrategy = new MarkovEquityStrategy();
            var randomWalk = new EquitiesMarkovProcess(
                this._exchangeTickInitialiser,
                randomWalkStrategy,
                this._heartbeat,
                this._logger);
            var stream = new ExchangeStream(new UnsubscriberFactory<EquityIntraDayTimeBarCollection>());
            var observer = new RecordingObserver<EquityIntraDayTimeBarCollection>(this._logger, 10);
            stream.Subscribe(observer);

            randomWalk.InitiateWalk(stream);

            Assert.AreEqual(observer.Buffer.Count, 1);

            randomWalk.TerminateWalk();
        }

        [Test]
        public void InitiateWalk_ThrowsExceptionFor_NullStream()
        {
            var randomWalk = new EquitiesMarkovProcess(
                this._exchangeTickInitialiser,
                this._strategy,
                this._heartbeat,
                this._logger);

            Assert.Throws<ArgumentNullException>(() => randomWalk.InitiateWalk(null));
        }

        [Test]
        [Explicit]
        public void InitiateWalk_WaitThenTerminateWalk_EnsuresNoMoreTicksTocked()
        {
            var randomWalk = new EquitiesMarkovProcess(
                new NasdaqInitialiser(),
                this._strategy,
                this._heartbeat,
                this._logger);
            var stream = new ExchangeStream(new UnsubscriberFactory<EquityIntraDayTimeBarCollection>());
            var observer = new RecordingObserver<EquityIntraDayTimeBarCollection>(this._logger, 5);
            stream.Subscribe(observer);

            randomWalk.InitiateWalk(stream);

            var timeOut = DateTime.UtcNow.AddSeconds(5);

            while (observer.Buffer.Count < 2 && DateTime.UtcNow < timeOut)
            {
                // don't sleep the thread
            }

            randomWalk.TerminateWalk();

            Assert.AreEqual(observer.Buffer.Count, 2);

            var timerForStragglers = DateTime.UtcNow.AddSeconds(2);

            while (DateTime.UtcNow < timerForStragglers)
            {
                // don't sleep the thread
            }

            Assert.AreEqual(observer.Buffer.Count, 2);

            A.CallTo(() => this._logger.LogInformation("Random walk generator terminating walk"))
                .MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._logger = A.Fake<ILogger>();
            this._exchangeTickInitialiser = A.Fake<IExchangeSeriesInitialiser>();
            this._strategy = A.Fake<IEquityDataGeneratorStrategy>();
            this._heartbeat = new Heartbeat(TimeSpan.FromMilliseconds(500));
        }
    }
}