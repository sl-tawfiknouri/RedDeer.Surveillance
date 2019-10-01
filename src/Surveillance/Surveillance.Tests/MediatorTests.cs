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

    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// The mediator tests.
    /// </summary>
    [TestFixture]
    public class MediatorTests
    {
        /// <summary>
        /// The coordinator mediator.
        /// </summary>
        private ICoordinatorMediator coordinatorMediator;

        /// <summary>
        /// The heartbeat service.
        /// </summary>
        private IApplicationHeartbeatService heartbeatService;

        /// <summary>
        /// The rule distributor mediator.
        /// </summary>
        private IRuleDistributorMediator ruleDistributorMediator;

        /// <summary>
        /// The rule engine mediator.
        /// </summary>
        private IRulesEngineMediator ruleEngineMediator;

        /// <summary>
        /// The rule scheduler mediator.
        /// </summary>
        private IRuleSchedulerMediator ruleSchedulerMediator;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<Mediator> logger;

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.ruleEngineMediator = A.Fake<IRulesEngineMediator>();
            this.ruleDistributorMediator = A.Fake<IRuleDistributorMediator>();
            this.coordinatorMediator = A.Fake<ICoordinatorMediator>();
            this.ruleSchedulerMediator = A.Fake<IRuleSchedulerMediator>();
            this.heartbeatService = A.Fake<IApplicationHeartbeatService>();
            this.logger = A.Fake<ILogger<Mediator>>();
        }

        /// <summary>
        /// The constructor null coordinator mediator throws exception.
        /// </summary>
        [Test]
        public void ConstructorNullCoordinatorMediatorThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Mediator(
                    this.ruleDistributorMediator,
                    this.ruleEngineMediator,
                    null,
                    this.ruleSchedulerMediator,
                    this.heartbeatService,
                    this.logger));
        }

        /// <summary>
        /// The constructor null rule distributor mediator throws exception.
        /// </summary>
        [Test]
        public void ConstructorNullRuleDistributorMediatorThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Mediator(
                    null,
                    this.ruleEngineMediator,
                    this.coordinatorMediator,
                    this.ruleSchedulerMediator,
                    this.heartbeatService,
                    this.logger));
        }

        /// <summary>
        /// The constructor null rule scheduler throws exception.
        /// </summary>
        [Test]
        public void ConstructorNullRuleSchedulerThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new Mediator(
                    this.ruleDistributorMediator,
                    null,
                    this.coordinatorMediator,
                    this.ruleSchedulerMediator,
                    this.heartbeatService,
                    this.logger));
        }

        /// <summary>
        /// The initiate calls initiate on trade service and scheduler.
        /// </summary>
        [Test]
        public void InitiateCallsInitiateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(
                this.ruleDistributorMediator,
                this.ruleEngineMediator,
                this.coordinatorMediator,
                this.ruleSchedulerMediator,
                this.heartbeatService,
                this.logger);

            mediator.Initiate();

            A.CallTo(() => this.heartbeatService.Initialise()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleDistributorMediator.Initiate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleEngineMediator.Initiate()).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The terminate calls terminate on trade service and scheduler.
        /// </summary>
        [Test]
        public void TerminateCallsTerminateOnTradeServiceAndScheduler()
        {
            var mediator = new Mediator(
                this.ruleDistributorMediator,
                this.ruleEngineMediator,
                this.coordinatorMediator,
                this.ruleSchedulerMediator,
                this.heartbeatService,
                this.logger);

            mediator.Terminate();

            A.CallTo(() => this.ruleDistributorMediator.Terminate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleEngineMediator.Terminate()).MustHaveHappenedOnceExactly();
        }
    }
}