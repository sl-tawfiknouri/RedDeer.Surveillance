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
        private IReddeerTradeService _tradeService;
        private IReddeerRuleScheduler _ruleScheduler;

        [SetUp]
        public void Setup()
        {
            _tradeService = A.Fake<IReddeerTradeService>();
            _ruleScheduler = A.Fake<IReddeerRuleScheduler>();
        }

        [Test]
        public void Constructor_NullTradeService_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(null, _ruleScheduler));
        }

        [Test]
        public void Constructor_NullRuleScheduler_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(_tradeService, null));
        }

        [Test]
        public void Initiate_CallsInitiateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(_tradeService, _ruleScheduler);

            mediator.Initiate();

            A.CallTo(() => _tradeService.Initialise()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleScheduler.Initiate()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Terminate_CallsTerminateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(_tradeService, _ruleScheduler);

            mediator.Terminate();

            A.CallTo(() => _tradeService.Dispose()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleScheduler.Terminate()).MustHaveHappenedOnceExactly();
        }
    }
}
