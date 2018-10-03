﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Scheduling;
using Domain.Scheduling.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Api.RuleParameter.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Scheduler;
using Surveillance.Universe.Interfaces;
using Surveillance.Utility.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.Tests.Scheduler
{
    [TestFixture]
    public class ReddeerRuleSchedulerTests
    {
        private ISpoofingRuleFactory _spoofingRuleFactory;
        private ICancelledOrderRuleFactory _cancelledOrderRuleFactory;
        private IHighProfitRuleFactory _highProfitRuleFactory;
        private IMarkingTheCloseRuleFactory _markingTheCloseRuleFactory;
        private IUniverseBuilder _universeBuilder;
        private IUniversePlayerFactory _universePlayerFactory;
        private ISpoofingRule _spoofingRule;
        private IUniverse _universe;
        private IUniversePlayer _universePlayer;
        private IRuleParameterApiRepository _ruleApiRepository;
        private IRuleParameterToRulesMapper _parameterMapper;
        private IApiHeartbeat _apiHeartbeat;
        
        private IAwsQueueClient _awsQueueClient;
        private IAwsConfiguration _awsConfiguration;
        private IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;

        private ILogger<ReddeerRuleScheduler> _logger;

        [SetUp]
        public void Setup()
        {
            _spoofingRuleFactory = A.Fake<ISpoofingRuleFactory>();
            _cancelledOrderRuleFactory = A.Fake<ICancelledOrderRuleFactory>();
            _highProfitRuleFactory = A.Fake<IHighProfitRuleFactory>();
            _markingTheCloseRuleFactory = A.Fake<IMarkingTheCloseRuleFactory>();
            _universeBuilder = A.Fake<IUniverseBuilder>();
            _universePlayerFactory = A.Fake<IUniversePlayerFactory>();
            _spoofingRule = A.Fake<ISpoofingRule>();
            _universe = A.Fake<IUniverse>();
            _universePlayer = A.Fake<IUniversePlayer>();
            _awsQueueClient = A.Fake<IAwsQueueClient>();
            _awsConfiguration = A.Fake<IAwsConfiguration>();
            _messageBusSerialiser = A.Fake<IScheduledExecutionMessageBusSerialiser>();
            _ruleApiRepository = A.Fake<IRuleParameterApiRepository>();
            _parameterMapper = A.Fake<IRuleParameterToRulesMapper>();
            _apiHeartbeat = A.Fake<IApiHeartbeat>();
            _logger = A.Fake <ILogger<ReddeerRuleScheduler>>();
        }

        [Test]
        public void Constructor_NullSpoofingRule_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(
                    null,
                    _cancelledOrderRuleFactory,
                    _highProfitRuleFactory,
                    _markingTheCloseRuleFactory,
                    _universeBuilder,
                    _universePlayerFactory,
                    _awsQueueClient,
                    _awsConfiguration,
                    _messageBusSerialiser,
                    _ruleApiRepository,
                    _parameterMapper,
                    _apiHeartbeat,
                    _logger));
        }

        [Test]
        public void Constructor_NullUniverseBuilder_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(
                    _spoofingRuleFactory,
                    _cancelledOrderRuleFactory,
                    _highProfitRuleFactory,
                    _markingTheCloseRuleFactory,
                    null,
                    _universePlayerFactory,
                    _awsQueueClient,
                    _awsConfiguration,
                    _messageBusSerialiser,
                    _ruleApiRepository,
                    _parameterMapper,
                    _apiHeartbeat,
                    _logger));
        }

        [Test]
        public void Constructor_NullUniversePlayerFactory_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(
                    _spoofingRuleFactory,
                    _cancelledOrderRuleFactory,
                    _highProfitRuleFactory,
                    _markingTheCloseRuleFactory,
                    _universeBuilder,
                    null,
                    _awsQueueClient,
                    _awsConfiguration,
                    _messageBusSerialiser,
                    _ruleApiRepository,
                    _parameterMapper,
                    _apiHeartbeat,
                    _logger));
        }

        [Test]
        public async Task Execute_Spoofing_CallsSpoofingRule()
        {
            var scheduler = new ReddeerRuleScheduler(
                _spoofingRuleFactory,
                _cancelledOrderRuleFactory,
                _highProfitRuleFactory,
                _markingTheCloseRuleFactory,
                _universeBuilder,
                _universePlayerFactory,
                _awsQueueClient,
                _awsConfiguration,
                _messageBusSerialiser,
                _ruleApiRepository,
                _parameterMapper,
                _apiHeartbeat,
                _logger);

            var schedule = new ScheduledExecution
            {
                Rules = new List<Domain.Scheduling.Rules> {Domain.Scheduling.Rules.Spoofing},
                TimeSeriesInitiation = DateTime.UtcNow.AddMinutes(-10),
                TimeSeriesTermination = DateTime.UtcNow
            };

            A.CallTo(() => _universeBuilder.Summon(A<ScheduledExecution>.Ignored)).Returns(_universe);
            A.CallTo(() => _universePlayerFactory.Build()).Returns(_universePlayer);
            A.CallTo(() => _spoofingRuleFactory.Build(A<ISpoofingRuleParameters>.Ignored)).Returns(_spoofingRule);

           await scheduler.Execute(schedule);

            A.CallTo(() => _spoofingRuleFactory.Build(A<ISpoofingRuleParameters>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _universePlayerFactory.Build()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _universePlayer.Subscribe(_spoofingRule)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _universePlayer.Play(A<IUniverse>.Ignored)).MustHaveHappened();
        }

        [Test]
        public async Task Execute_Nothing_DoesNothing()
        {
            var scheduler = new ReddeerRuleScheduler(
                _spoofingRuleFactory,
                _cancelledOrderRuleFactory,
                _highProfitRuleFactory,
                _markingTheCloseRuleFactory,
                _universeBuilder,
                _universePlayerFactory,
                _awsQueueClient,
                _awsConfiguration,
                _messageBusSerialiser,
                _ruleApiRepository,
                _parameterMapper,
                _apiHeartbeat,
                _logger);

            var schedule = new ScheduledExecution
            {
                Rules = new List<Domain.Scheduling.Rules>(),
                TimeSeriesInitiation = DateTime.UtcNow.AddMinutes(-10),
                TimeSeriesTermination = DateTime.UtcNow
            };

            A.CallTo(() => _universeBuilder.Summon(A<ScheduledExecution>.Ignored)).Returns(_universe);
            A.CallTo(() => _universePlayerFactory.Build()).Returns(_universePlayer);
            A.CallTo(() => _spoofingRuleFactory.Build(A<ISpoofingRuleParameters>.Ignored)).Returns(_spoofingRule);

            await scheduler.Execute(schedule);

            A.CallTo(() => _spoofingRuleFactory.Build(A<ISpoofingRuleParameters>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _universePlayerFactory.Build()).MustNotHaveHappened();
            A.CallTo(() => _universePlayer.Subscribe(_spoofingRule)).MustNotHaveHappened();
        }

        [Test]
        [Explicit("Takes over a minute to run")]
        public async Task ExecuteDistributedMessage_ContinuesOnNormalLogicIfServicesRunning()
        {
            var serialiser = new ScheduledExecutionMessageBusSerialiser();
            A.CallTo(() => _messageBusSerialiser.DeserialisedScheduledExecution(A<string>.Ignored)).Returns(null);

            var scheduler = new ReddeerRuleScheduler(
                _spoofingRuleFactory,
                _cancelledOrderRuleFactory,
                _highProfitRuleFactory,
                _markingTheCloseRuleFactory,
                _universeBuilder,
                _universePlayerFactory,
                _awsQueueClient,
                _awsConfiguration,
                _messageBusSerialiser,
                _ruleApiRepository,
                _parameterMapper,
                _apiHeartbeat,
                _logger);

            var schedule = new ScheduledExecution
            {
                Rules = new List<Domain.Scheduling.Rules>(),
                TimeSeriesInitiation = DateTime.UtcNow.AddMinutes(-10),
                TimeSeriesTermination = DateTime.UtcNow
            };

            A.CallTo(() => _apiHeartbeat.HeartsBeating()).ReturnsNextFromSequence(
                new[]
                {
                    Task.FromResult(false),
                    Task.FromResult(true)
                });

            var message = serialiser.SerialiseScheduledExecution(schedule);

            await scheduler.ExecuteDistributedMessage("etc", message);
            
            A.CallTo(() => _messageBusSerialiser.DeserialisedScheduledExecution(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _apiHeartbeat.HeartsBeating()).MustHaveHappenedTwiceExactly();
        }
    }
}
