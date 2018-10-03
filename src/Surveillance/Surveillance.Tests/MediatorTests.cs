using System;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.Scheduler.Interfaces;
using Surveillance.Services.Interfaces;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.Tests
{
    [TestFixture]
    public class MediatorTests
    {
        private IReddeerTradeService _tradeService;
        private IReddeerRuleScheduler _ruleScheduler;
        private IReddeerSmartRuleScheduler _ruleSmartScheduler;
        private ISystemProcessRepository _processRepository;

        [SetUp]
        public void Setup()
        {
            _tradeService = A.Fake<IReddeerTradeService>();
            _ruleScheduler = A.Fake<IReddeerRuleScheduler>();
            _ruleSmartScheduler = A.Fake<IReddeerSmartRuleScheduler>();
            _processRepository = A.Fake<ISystemProcessRepository>();
        }

        [Test]
        public void Constructor_NullTradeService_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(null, _ruleScheduler, _ruleSmartScheduler, _processRepository));
        }

        [Test]
        public void Constructor_NullRuleScheduler_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(_tradeService, null, _ruleSmartScheduler, _processRepository));
        }

        [Test]
        public void Constructor_NullSmartRuleScheduler_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(_tradeService, _ruleScheduler, null, _processRepository));
        }

        [Test]
        public void Initiate_CallsInitiateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(_tradeService, _ruleScheduler, _ruleSmartScheduler, _processRepository);

            mediator.Initiate();

            A.CallTo(() => _tradeService.Initialise()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleScheduler.Initiate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleSmartScheduler.Initiate()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Terminate_CallsTerminateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(_tradeService, _ruleScheduler, _ruleSmartScheduler, _processRepository);

            mediator.Terminate();

            A.CallTo(() => _tradeService.Dispose()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleScheduler.Terminate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleSmartScheduler.Terminate()).MustHaveHappenedOnceExactly();
        }
    }
}
