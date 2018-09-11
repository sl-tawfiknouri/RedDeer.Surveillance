using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Scheduling;
using Domain.Scheduling.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.AwsQueue.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;
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

        private IAwsQueueClient _awsQueueClient;
        private IDataLayerConfiguration _dataLayerConfiguration;
        private IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;

        private ILogger<ReddeerRuleScheduler> _logger;

        [SetUp]
        public void Setup()
        {
            _spoofingRuleFactory = A.Fake<ISpoofingRuleFactory>();
            _universeBuilder = A.Fake<IUniverseBuilder>();
            _universePlayerFactory = A.Fake<IUniversePlayerFactory>();
            _spoofingRule = A.Fake<ISpoofingRule>();
            _universe = A.Fake<IUniverse>();
            _universePlayer = A.Fake<IUniversePlayer>();
            _awsQueueClient = A.Fake<IAwsQueueClient>();
            _dataLayerConfiguration = A.Fake<IDataLayerConfiguration>();
            _messageBusSerialiser = A.Fake<IScheduledExecutionMessageBusSerialiser>();
            _logger = A.Fake <ILogger<ReddeerRuleScheduler>>();
        }

        [Test]
        public void Constructor_NullSpoofingRule_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(
                    null,
                    _universeBuilder,
                    _universePlayerFactory,
                    _awsQueueClient,
                    _dataLayerConfiguration,
                    _messageBusSerialiser,
                    _logger));
        }

        [Test]
        public void Constructor_NullUniverseBuilder_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(
                    _spoofingRuleFactory,
                    null,
                    _universePlayerFactory,
                    _awsQueueClient,
                    _dataLayerConfiguration,
                    _messageBusSerialiser,
                    _logger));
        }

        [Test]
        public void Constructor_NullUniversePlayerFactory_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(
                    _spoofingRuleFactory,
                    _universeBuilder,
                    null,
                    _awsQueueClient,
                    _dataLayerConfiguration,
                    _messageBusSerialiser,
                    _logger));
        }

        [Test]
        public async Task Execute_Spoofing_CallsSpoofingRule()
        {
            var scheduler = new ReddeerRuleScheduler(
                _spoofingRuleFactory,
                _universeBuilder,
                _universePlayerFactory,
                _awsQueueClient,
                _dataLayerConfiguration,
                _messageBusSerialiser,
                _logger);

            var schedule = new ScheduledExecution
            {
                Rules = new List<Domain.Scheduling.Rules> {Domain.Scheduling.Rules.Spoofing},
                TimeSeriesInitiation = DateTime.UtcNow.AddMinutes(-10),
                TimeSeriesTermination = DateTime.UtcNow
            };

            A.CallTo(() => _universeBuilder.Summon(A<ScheduledExecution>.Ignored)).Returns(_universe);
            A.CallTo(() => _universePlayerFactory.Build()).Returns(_universePlayer);
            A.CallTo(() => _spoofingRuleFactory.Build()).Returns(_spoofingRule);

           await scheduler.Execute(schedule);

            A.CallTo(() => _spoofingRuleFactory.Build()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _universePlayerFactory.Build()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _universePlayer.Subscribe(_spoofingRule)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _universePlayer.Play(A<IUniverse>.Ignored)).MustHaveHappened();
        }

        [Test]
        public async Task Execute_Nothing_DoesNothing()
        {
            var scheduler = new ReddeerRuleScheduler(
                _spoofingRuleFactory,
                _universeBuilder,
                _universePlayerFactory,
                _awsQueueClient,
                _dataLayerConfiguration,
                _messageBusSerialiser,
                _logger);

            var schedule = new ScheduledExecution
            {
                Rules = new List<Domain.Scheduling.Rules> { },
                TimeSeriesInitiation = DateTime.UtcNow.AddMinutes(-10),
                TimeSeriesTermination = DateTime.UtcNow
            };

            A.CallTo(() => _universeBuilder.Summon(A<ScheduledExecution>.Ignored)).Returns(_universe);
            A.CallTo(() => _universePlayerFactory.Build()).Returns(_universePlayer);
            A.CallTo(() => _spoofingRuleFactory.Build()).Returns(_spoofingRule);

            await scheduler.Execute(schedule);

            A.CallTo(() => _spoofingRuleFactory.Build()).MustNotHaveHappened();
            A.CallTo(() => _universePlayerFactory.Build()).MustNotHaveHappened();
            A.CallTo(() => _universePlayer.Subscribe(_spoofingRule)).MustNotHaveHappened();
        }

    }
}
