using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Utilities.Interfaces;
using Surveillance.Engine.DataCoordinator.Interfaces;
using Surveillance.Engine.RuleDistributor.Interfaces;
using Surveillance.Engine.Rules.Interfaces;
using Surveillance.Engine.Scheduler.Interfaces;

namespace Surveillance.Tests
{
    [TestFixture]
    public class MediatorTests
    {
        private IRulesEngineMediator _ruleEngineMediator;
        private IRuleDistributorMediator _ruleDistributorMediator;
        private ICoordinatorMediator _coordinatorMediator;
        private IRuleSchedulerMediator _ruleSchedulerMediator;

        private IApplicationHeartbeatService _heartbeatService;
        private ILogger<Mediator> _logger;

        [SetUp]
        public void Setup()
        {
            _ruleEngineMediator = A.Fake<IRulesEngineMediator>();
            _ruleDistributorMediator = A.Fake<IRuleDistributorMediator>();
            _coordinatorMediator = A.Fake<ICoordinatorMediator>();
            _ruleSchedulerMediator = A.Fake<IRuleSchedulerMediator>();
            _heartbeatService = A.Fake<IApplicationHeartbeatService>();
            _logger = A.Fake<ILogger<Mediator>>();
        }

        [Test]
        public void Constructor_NullRuleScheduler_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(_ruleDistributorMediator, null, _coordinatorMediator, _ruleSchedulerMediator, _heartbeatService, _logger));
        }

        [Test]
        public void Constructor_NullRuleDistributorMediator_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(null, _ruleEngineMediator, _coordinatorMediator, _ruleSchedulerMediator, _heartbeatService, _logger));
        }

        [Test]
        public void Constructor_NullCoordinatorMediator_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(_ruleDistributorMediator, _ruleEngineMediator, null, _ruleSchedulerMediator, _heartbeatService, _logger));
        }

        [Test]
        public void Initiate_CallsInitiateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(_ruleDistributorMediator, _ruleEngineMediator, _coordinatorMediator, _ruleSchedulerMediator, _heartbeatService, _logger);

            mediator.Initiate();

            A.CallTo(() => _heartbeatService.Initialise()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleDistributorMediator.Initiate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleEngineMediator.Initiate()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Terminate_CallsTerminateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(_ruleDistributorMediator, _ruleEngineMediator, _coordinatorMediator, _ruleSchedulerMediator, _heartbeatService, _logger);

            mediator.Terminate();

            A.CallTo(() => _ruleDistributorMediator.Terminate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleEngineMediator.Terminate()).MustHaveHappenedOnceExactly();
        }
    }
}
