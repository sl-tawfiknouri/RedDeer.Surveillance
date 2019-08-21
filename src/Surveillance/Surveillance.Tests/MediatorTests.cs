namespace Surveillance.Tests
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.Utilities.Interfaces;
    using Surveillance.Engine.DataCoordinator.Interfaces;
    using Surveillance.Engine.RuleDistributor.Interfaces;
    using Surveillance.Engine.Rules.Interfaces;
    using Surveillance.Engine.Scheduler.Interfaces;

    [TestFixture]
    public class MediatorTests
    {
        private ICoordinatorMediator _coordinatorMediator;

        private IApplicationHeartbeatService _heartbeatService;

        private ILogger<Mediator> _logger;

        private IRuleDistributorMediator _ruleDistributorMediator;

        private IRulesEngineMediator _ruleEngineMediator;

        private IRuleSchedulerMediator _ruleSchedulerMediator;

        [Test]
        public void Constructor_NullCoordinatorMediator_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new Mediator(
                    this._ruleDistributorMediator,
                    this._ruleEngineMediator,
                    null,
                    this._ruleSchedulerMediator,
                    this._heartbeatService,
                    this._logger));
        }

        [Test]
        public void Constructor_NullRuleDistributorMediator_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new Mediator(
                    null,
                    this._ruleEngineMediator,
                    this._coordinatorMediator,
                    this._ruleSchedulerMediator,
                    this._heartbeatService,
                    this._logger));
        }

        [Test]
        public void Constructor_NullRuleScheduler_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new Mediator(
                    this._ruleDistributorMediator,
                    null,
                    this._coordinatorMediator,
                    this._ruleSchedulerMediator,
                    this._heartbeatService,
                    this._logger));
        }

        [Test]
        public void Initiate_CallsInitiateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(
                this._ruleDistributorMediator,
                this._ruleEngineMediator,
                this._coordinatorMediator,
                this._ruleSchedulerMediator,
                this._heartbeatService,
                this._logger);

            mediator.Initiate();

            A.CallTo(() => this._heartbeatService.Initialise()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleDistributorMediator.Initiate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleEngineMediator.Initiate()).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._ruleEngineMediator = A.Fake<IRulesEngineMediator>();
            this._ruleDistributorMediator = A.Fake<IRuleDistributorMediator>();
            this._coordinatorMediator = A.Fake<ICoordinatorMediator>();
            this._ruleSchedulerMediator = A.Fake<IRuleSchedulerMediator>();
            this._heartbeatService = A.Fake<IApplicationHeartbeatService>();
            this._logger = A.Fake<ILogger<Mediator>>();
        }

        [Test]
        public void Terminate_CallsTerminateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(
                this._ruleDistributorMediator,
                this._ruleEngineMediator,
                this._coordinatorMediator,
                this._ruleSchedulerMediator,
                this._heartbeatService,
                this._logger);

            mediator.Terminate();

            A.CallTo(() => this._ruleDistributorMediator.Terminate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleEngineMediator.Terminate()).MustHaveHappenedOnceExactly();
        }
    }
}