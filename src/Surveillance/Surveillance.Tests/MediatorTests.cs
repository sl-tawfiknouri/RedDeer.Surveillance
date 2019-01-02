using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Scheduler.Interfaces;
using Surveillance.System.Auditing.Utilities.Interfaces;

namespace Surveillance.Tests
{
    [TestFixture]
    public class MediatorTests
    {
        private IReddeerRuleScheduler _ruleScheduler;
        private IReddeerDistributedRuleScheduler _ruleDistributedScheduler;
        private IApplicationHeartbeatService _heartbeatService;
        private ILogger<Mediator> _logger;

        [SetUp]
        public void Setup()
        {
            _ruleScheduler = A.Fake<IReddeerRuleScheduler>();
            _ruleDistributedScheduler = A.Fake<IReddeerDistributedRuleScheduler>();
            _heartbeatService = A.Fake<IApplicationHeartbeatService>();
            _logger = A.Fake<ILogger<Mediator>>();
        }

        [Test]
        public void Constructor_NullRuleScheduler_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(null, _ruleDistributedScheduler, _heartbeatService, _logger));
        }

        [Test]
        public void Constructor_NullSmartRuleScheduler_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(_ruleScheduler, null, _heartbeatService, _logger));
        }

        [Test]
        public void Initiate_CallsInitiateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(_ruleScheduler, _ruleDistributedScheduler, _heartbeatService, _logger);

            mediator.Initiate();

            A.CallTo(() => _heartbeatService.Initialise()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleScheduler.Initiate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleDistributedScheduler.Initiate()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Terminate_CallsTerminateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(_ruleScheduler, _ruleDistributedScheduler, _heartbeatService, _logger);

            mediator.Terminate();

            A.CallTo(() => _ruleScheduler.Terminate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleDistributedScheduler.Terminate()).MustHaveHappenedOnceExactly();
        }
    }
}
