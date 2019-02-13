using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Engines.Interfaces.Mediator;
using Surveillance.Scheduler.Interfaces;
using Surveillance.Systems.Auditing.Utilities.Interfaces;

namespace Surveillance.Tests
{
    [TestFixture]
    public class MediatorTests
    {
        private IReddeerRuleScheduler _ruleScheduler;
        private IMediator _ruleDistributorMediator;
        private IApplicationHeartbeatService _heartbeatService;
        private ILogger<Mediator> _logger;

        [SetUp]
        public void Setup()
        {
            _ruleScheduler = A.Fake<IReddeerRuleScheduler>();
            _ruleDistributorMediator = A.Fake<IMediator>();
            _heartbeatService = A.Fake<IApplicationHeartbeatService>();
            _logger = A.Fake<ILogger<Mediator>>();
        }

        [Test]
        public void Constructor_NullRuleScheduler_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(null, _ruleDistributorMediator, _heartbeatService, _logger));
        }

        [Test]
        public void Constructor_NullRuleDistributorMediator_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(_ruleScheduler, null, _heartbeatService, _logger));
        }

        [Test]
        public void Initiate_CallsInitiateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(_ruleScheduler, _ruleDistributorMediator, _heartbeatService, _logger);

            mediator.Initiate();

            A.CallTo(() => _heartbeatService.Initialise()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleScheduler.Initiate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleDistributorMediator.Initiate()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Terminate_CallsTerminateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(_ruleScheduler, _ruleDistributorMediator, _heartbeatService, _logger);

            mediator.Terminate();

            A.CallTo(() => _ruleScheduler.Terminate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleDistributorMediator.Terminate()).MustHaveHappenedOnceExactly();
        }
    }
}
