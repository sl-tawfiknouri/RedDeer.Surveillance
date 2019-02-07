using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Scheduling;
using DomainV2.Scheduling.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using Surveillance.Analytics.Streams.Factory.Interfaces;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Analytics.Subscriber.Factory.Interfaces;
using Surveillance.Data.Subscribers.Interfaces;
using Surveillance.DataLayer.Aurora.Analytics.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.MessageBusIO.Interfaces;
using Surveillance.RuleParameters.Manager.Interfaces;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Scheduler;
using Surveillance.Systems.Auditing.Context.Interfaces;
using Surveillance.Universe.Interfaces;
using Surveillance.Universe.Subscribers.Interfaces;
using Surveillance.Utility.Interfaces;
using Utilities.Aws_IO.Interfaces;

namespace Surveillance.Tests.Scheduler
{
    [TestFixture]
    public class ReddeerRuleSchedulerTests
    {
        private IUniverseBuilder _universeBuilder;
        private IUniversePlayerFactory _universePlayerFactory;
        private IUniversePlayer _universePlayer;
        private IUniverseRuleSubscriber _universeRuleSubscriber;
        private ISpoofingRule _spoofingRule;
        private IUniverse _universe;
        private IApiHeartbeat _apiHeartbeat;
        private ISystemProcessContext _ctx;
        private ISystemProcessOperationContext _opCtx;
        private IAwsQueueClient _awsQueueClient;
        private IAwsConfiguration _awsConfiguration;
        private IScheduledExecutionMessageBusSerialiser _messageBusSerialiser;
        private IUniverseAnalyticsSubscriberFactory _factory;
        private IRuleAnalyticsUniverseRepository _ruleAnalyticsRepository;
        private IUniverseAlertStreamFactory _alertStreamFactory;
        private IUniverseAlertStreamSubscriberFactory _alertStreamSubscriberFactory;
        private IRuleRunUpdateMessageSender _ruleRunUpdateMessageSender;
        private IUniverseDataRequestsSubscriberFactory _dataRequestsSubscriber;
        private IRuleAnalyticsAlertsRepository _alertsRepository;
        private IUniversePercentageCompletionLogger _percentageCompletionLogger;
        private IRuleParameterLeadingTimespanCalculator _leadingTimespanCalculator;
        private IRuleParameterManager _ruleParameterManager;

        private ILogger<ReddeerRuleScheduler> _logger;

        [SetUp]
        public void Setup()
        {
            _universeBuilder = A.Fake<IUniverseBuilder>();
            _universePlayerFactory = A.Fake<IUniversePlayerFactory>();
            _universeRuleSubscriber = A.Fake<IUniverseRuleSubscriber>();
            _spoofingRule = A.Fake<ISpoofingRule>();
            _universe = A.Fake<IUniverse>();
            _universePlayer = A.Fake<IUniversePlayer>();
            _awsQueueClient = A.Fake<IAwsQueueClient>();
            _awsConfiguration = A.Fake<IAwsConfiguration>();
            _messageBusSerialiser = A.Fake<IScheduledExecutionMessageBusSerialiser>();
            _apiHeartbeat = A.Fake<IApiHeartbeat>();
            _ctx = A.Fake<ISystemProcessContext>();
            _opCtx = A.Fake<ISystemProcessOperationContext>();
            _factory = A.Fake<IUniverseAnalyticsSubscriberFactory>();
            _ruleAnalyticsRepository = A.Fake<IRuleAnalyticsUniverseRepository>();
            _alertStreamFactory = A.Fake<IUniverseAlertStreamFactory>();
            _alertStreamSubscriberFactory = A.Fake<IUniverseAlertStreamSubscriberFactory>();
            _alertsRepository = A.Fake<IRuleAnalyticsAlertsRepository>();
            _percentageCompletionLogger = A.Fake<IUniversePercentageCompletionLogger>();
            _ruleRunUpdateMessageSender = A.Fake<IRuleRunUpdateMessageSender>();
            _dataRequestsSubscriber = A.Fake<IUniverseDataRequestsSubscriberFactory>();
            _ruleParameterManager = A.Fake<IRuleParameterManager>();
            _leadingTimespanCalculator = A.Fake<IRuleParameterLeadingTimespanCalculator>();

            _logger = A.Fake <ILogger<ReddeerRuleScheduler>>();
        }

        [Test]
        public void Constructor_NullUniverseBuilder_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(
                    null,
                    _universePlayerFactory,
                    _awsQueueClient,
                    _awsConfiguration,
                    _messageBusSerialiser,
                    _apiHeartbeat,
                    _ctx,
                    _universeRuleSubscriber,
                    _factory,
                    _ruleAnalyticsRepository,
                    _alertStreamFactory,
                    _alertStreamSubscriberFactory,
                    _alertsRepository,
                    _ruleRunUpdateMessageSender,
                    _dataRequestsSubscriber,
                    _ruleParameterManager,
                    _leadingTimespanCalculator,
                    _percentageCompletionLogger,
                    _logger));
        }

        [Test]
        public void Constructor_NullUniversePlayerFactory_ThrowsArgumentNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                // ReSharper disable once ObjectCreationAsStatement
                new ReddeerRuleScheduler(
                    _universeBuilder,
                    null,
                    _awsQueueClient,
                    _awsConfiguration,
                    _messageBusSerialiser,
                    _apiHeartbeat,
                    _ctx,
                    _universeRuleSubscriber,
                    _factory,
                    _ruleAnalyticsRepository,
                    _alertStreamFactory,
                    _alertStreamSubscriberFactory,
                    _alertsRepository,
                    _ruleRunUpdateMessageSender,
                    _dataRequestsSubscriber,
                    _ruleParameterManager,
                    _leadingTimespanCalculator,
                    _percentageCompletionLogger,
                    _logger));
        }

        [Test]
        public async Task Execute_Spoofing_CallsSpoofingRule()
        {
            var scheduler = new ReddeerRuleScheduler(
                _universeBuilder,
                _universePlayerFactory,
                _awsQueueClient,
                _awsConfiguration,
                _messageBusSerialiser,
                _apiHeartbeat,
                _ctx,
                _universeRuleSubscriber,
                _factory,
                _ruleAnalyticsRepository,
                _alertStreamFactory,
                _alertStreamSubscriberFactory,
                _alertsRepository,
                _ruleRunUpdateMessageSender,
                _dataRequestsSubscriber,
                _ruleParameterManager,
                _leadingTimespanCalculator,
                _percentageCompletionLogger,
                _logger);

            var schedule = new ScheduledExecution
            {
                Rules = new List<RuleIdentifier> { new RuleIdentifier {Rule = DomainV2.Scheduling.Rules.Spoofing, Ids = new string[0]}},
                TimeSeriesInitiation = DateTime.UtcNow.AddMinutes(-10),
                TimeSeriesTermination = DateTime.UtcNow
            };

            A.CallTo(() => _universeBuilder.Summon(A<ScheduledExecution>.Ignored, A<ISystemProcessOperationContext>.Ignored)).Returns(_universe);
            A.CallTo(() => _universePlayerFactory.Build()).Returns(_universePlayer);

           await scheduler.Execute(schedule, _opCtx);

            A.CallTo(() => _universePlayerFactory.Build()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _universeRuleSubscriber.SubscribeRules(A<ScheduledExecution>.Ignored, A<IUniversePlayer>.Ignored, A<IUniverseAlertStream>.Ignored, A<IUniverseDataRequestsSubscriber>.Ignored,  A<ISystemProcessOperationContext>.Ignored, A<RuleParameterDto>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _universePlayer.Play(A<IUniverse>.Ignored)).MustHaveHappened();
        }

        [Test]
        public async Task Execute_Nothing_DoesNothing()
        {
            var scheduler = new ReddeerRuleScheduler(
                _universeBuilder,
                _universePlayerFactory,
                _awsQueueClient,
                _awsConfiguration,
                _messageBusSerialiser,
                _apiHeartbeat,
                _ctx,
                _universeRuleSubscriber,
                _factory,
                _ruleAnalyticsRepository,
                _alertStreamFactory,
                _alertStreamSubscriberFactory,
                _alertsRepository,
                _ruleRunUpdateMessageSender,
                _dataRequestsSubscriber,
                _ruleParameterManager,
                _leadingTimespanCalculator,
                _percentageCompletionLogger,
                _logger);

            var schedule = new ScheduledExecution
            {
                Rules = new List<RuleIdentifier>(),
                TimeSeriesInitiation = DateTime.UtcNow.AddMinutes(-10),
                TimeSeriesTermination = DateTime.UtcNow
            };

            A.CallTo(() => _universeBuilder.Summon(A<ScheduledExecution>.Ignored, A<ISystemProcessOperationContext>.Ignored)).Returns(_universe);
            A.CallTo(() => _universePlayerFactory.Build()).Returns(_universePlayer);

            await scheduler.Execute(schedule, _opCtx);

            A.CallTo(() => _universePlayerFactory.Build()).MustNotHaveHappened();
            A.CallTo(() => _universePlayer.Subscribe(_spoofingRule)).MustNotHaveHappened();
        }

        [Test]
        [Explicit("Takes over a minute to run")]
        public async Task ExecuteDistributedMessage_ContinuesOnNormalLogicIfServicesRunning()
        {
            var serialiser = new ScheduledExecutionMessageBusSerialiser(new ScheduleExecutionDtoMapper(null));
            A.CallTo(() => _messageBusSerialiser.DeserialisedScheduledExecution(A<string>.Ignored)).Returns(null);

            var scheduler = new ReddeerRuleScheduler(
                _universeBuilder,
                _universePlayerFactory,
                _awsQueueClient,
                _awsConfiguration,
                _messageBusSerialiser,
                _apiHeartbeat,
                _ctx,
                _universeRuleSubscriber,
                _factory,
                _ruleAnalyticsRepository,
                _alertStreamFactory,
                _alertStreamSubscriberFactory,
                _alertsRepository,
                _ruleRunUpdateMessageSender,
                _dataRequestsSubscriber,
                _ruleParameterManager,
                _leadingTimespanCalculator,
                _percentageCompletionLogger,
                _logger);

            var schedule = new ScheduledExecution
            {
                Rules = new List<RuleIdentifier>(),
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
