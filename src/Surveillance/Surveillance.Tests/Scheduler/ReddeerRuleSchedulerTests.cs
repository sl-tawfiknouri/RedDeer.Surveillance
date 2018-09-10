using System;
using System.Collections.Generic;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Scheduler;
using Surveillance.Universe.Interfaces;

namespace Surveillance.Tests.Scheduler
{
    [TestFixture]
    public class ReddeerRuleSchedulerTests
    {
        private ISpoofingRuleFactory _spoofingRuleFactory;
        private IUniverseBuilder _universeBuilder;
        private IUniversePlayerFactory _universePlayerFactory;
        private ISpoofingRule _spoofingRule;
        private IUniverse _universe;
        private IUniversePlayer _universePlayer;

        [SetUp]
        public void Setup()
        {
            _spoofingRuleFactory = A.Fake<ISpoofingRuleFactory>();
            _universeBuilder = A.Fake<IUniverseBuilder>();
            _universePlayerFactory = A.Fake<IUniversePlayerFactory>();
            _spoofingRule = A.Fake<ISpoofingRule>();
            _universe = A.Fake<IUniverse>();
            _universePlayer = A.Fake<IUniversePlayer>();
        }

        [Test]
        public void Constructor_NullSpoofingRule_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(null, _universeBuilder, _universePlayerFactory));
        }

        [Test]
        public void Constructor_NullUniverseBuilder_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(_spoofingRuleFactory, null, _universePlayerFactory));
        }

        [Test]
        public void Constructor_NullUniversePlayerFactory_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(_spoofingRuleFactory, _universeBuilder, null));
        }

        [Test]
        public void Execute_DoesNotThrow_NullExecutionArgument()
        {
            var scheduler = new ReddeerRuleScheduler(_spoofingRuleFactory, _universeBuilder, _universePlayerFactory);

            Assert.DoesNotThrow(() => scheduler.Execute(null));
        }

        [Test]
        public void Execute_Spoofing_CallsSpoofingRule()
        {
            var scheduler = new ReddeerRuleScheduler(_spoofingRuleFactory, _universeBuilder, _universePlayerFactory);
            var schedule = new ScheduledExecution
            {
                Rules = new List<Rules.Rules> {Rules.Rules.Spoofing},
                TimeSeriesInitiation = DateTime.UtcNow.AddMinutes(-10),
                TimeSeriesTermination = DateTime.UtcNow
            };

            A.CallTo(() => _universeBuilder.Summon(A<ScheduledExecution>.Ignored)).Returns(_universe);
            A.CallTo(() => _universePlayerFactory.Build()).Returns(_universePlayer);
            A.CallTo(() => _spoofingRuleFactory.Build()).Returns(_spoofingRule);

            scheduler.Execute(schedule);

            A.CallTo(() => _spoofingRuleFactory.Build()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _universePlayerFactory.Build()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _universePlayer.Subscribe(_spoofingRule)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _universePlayer.Play(A<IUniverse>.Ignored)).MustHaveHappened();
        }

        [Test]
        public void Execute_Nothing_DoesNothing()
        {
            var scheduler = new ReddeerRuleScheduler(_spoofingRuleFactory, _universeBuilder, _universePlayerFactory);
            var schedule = new ScheduledExecution
            {
                Rules = new List<Rules.Rules> { },
                TimeSeriesInitiation = DateTime.UtcNow.AddMinutes(-10),
                TimeSeriesTermination = DateTime.UtcNow
            };

            A.CallTo(() => _universeBuilder.Summon(A<ScheduledExecution>.Ignored)).Returns(_universe);
            A.CallTo(() => _universePlayerFactory.Build()).Returns(_universePlayer);
            A.CallTo(() => _spoofingRuleFactory.Build()).Returns(_spoofingRule);

            scheduler.Execute(schedule);

            A.CallTo(() => _spoofingRuleFactory.Build()).MustNotHaveHappened();
            A.CallTo(() => _universePlayerFactory.Build()).MustNotHaveHappened();
            A.CallTo(() => _universePlayer.Subscribe(_spoofingRule)).MustNotHaveHappened();
        }

    }
}
