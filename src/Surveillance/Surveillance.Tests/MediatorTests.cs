using System;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.Scheduler.Interfaces;
using Surveillance.Services.Interfaces;

namespace Surveillance.Tests
{
    [TestFixture]
    public class MediatorTests
    {
        private IReddeerRuleScheduler _ruleScheduler;
        private IReddeerDistributedRuleScheduler _ruleDistributedScheduler;
        private IApplicationHeartbeatService _heartbeatService;
        private IEnrichmentService _enrichmentService;

        [SetUp]
        public void Setup()
        {
            _ruleScheduler = A.Fake<IReddeerRuleScheduler>();
            _ruleDistributedScheduler = A.Fake<IReddeerDistributedRuleScheduler>();
            _heartbeatService = A.Fake<IApplicationHeartbeatService>();
            _enrichmentService = A.Fake<IEnrichmentService>();
        }

        [Test]
        public void Constructor_NullRuleScheduler_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(null, _ruleDistributedScheduler, _heartbeatService,  _enrichmentService));
        }

        [Test]
        public void Constructor_NullSmartRuleScheduler_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(_ruleScheduler, null, _heartbeatService,  _enrichmentService));
        }

        [Test]
        public void Initiate_CallsInitiateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(_ruleScheduler, _ruleDistributedScheduler, _heartbeatService, _enrichmentService);

            mediator.Initiate();

            A.CallTo(() => _heartbeatService.Initialise()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleScheduler.Initiate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleDistributedScheduler.Initiate()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Terminate_CallsTerminateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(_ruleScheduler, _ruleDistributedScheduler, _heartbeatService, _enrichmentService);

            mediator.Terminate();

            A.CallTo(() => _ruleScheduler.Terminate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleDistributedScheduler.Terminate()).MustHaveHappenedOnceExactly();
        }
    }
}
