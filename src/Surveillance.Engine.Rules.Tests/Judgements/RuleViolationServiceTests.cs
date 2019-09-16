namespace Surveillance.Engine.Rules.Tests.Judgements
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Rules;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using RedDeer.Contracts.SurveillanceService;

    using Surveillance.DataLayer.Aurora.Rules.Interfaces;
    using Surveillance.Engine.Rules.Judgements;
    using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
    using Surveillance.Engine.Rules.Queues.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Tests.Trades;
    using Surveillance.Engine.Rules.Trades;

    /// <summary>
    /// The rule violation service tests.
    /// </summary>
    [TestFixture]
    public class RuleViolationServiceTests
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<RuleViolationService> logger;

        /// <summary>
        /// The queue case publisher.
        /// </summary>
        private IQueueCasePublisher queueCasePublisher;

        /// <summary>
        /// The rule breach.
        /// </summary>
        private IRuleBreach ruleBreach;

        /// <summary>
        /// The rule breach orders repository.
        /// </summary>
        private IRuleBreachOrdersRepository ruleBreachOrdersRepository;

        /// <summary>
        /// The rule breach repository.
        /// </summary>
        private IRuleBreachRepository ruleBreachRepository;

        /// <summary>
        /// The rule breach to rule breach mapper.
        /// </summary>
        private IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper;

        /// <summary>
        /// The rule breach to rule breach orders mapper.
        /// </summary>
        private IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper;

        /// <summary>
        /// The add rule violation null rule breach does not throw.
        /// </summary>
        [Test]
        public void AddRuleViolationNullRuleBreachDoesNotThrow()
        {
            var ruleViolationService = new RuleViolationService(
                this.queueCasePublisher,
                this.ruleBreachRepository,
                this.ruleBreachOrdersRepository,
                this.ruleBreachToRuleBreachOrdersMapper,
                this.ruleBreachToRuleBreachMapper,
                this.logger);

            Assert.DoesNotThrow(() => ruleViolationService.AddRuleViolation(null));
        }

        /// <summary>
        /// The add rule violation rule breach adds to rule breach repository.
        /// </summary>
        [Test]
        public void AddRuleViolationRuleBreachAddsToRuleBreachRepository()
        {
            var ruleViolationService = new RuleViolationService(
                this.queueCasePublisher,
                this.ruleBreachRepository,
                this.ruleBreachOrdersRepository,
                this.ruleBreachToRuleBreachOrdersMapper,
                this.ruleBreachToRuleBreachMapper,
                this.logger);

            var tradePosition = new TradePosition(new List<Order> { OrderHelper.Orders(OrderStatus.Filled) });
            A.CallTo(() => this.ruleBreach.Trades).Returns(tradePosition);
            A.CallTo(() => this.ruleBreach.RuleParameters.TunedParameters).Returns(null);
            A.CallTo(() => this.ruleBreachRepository.Create(A<RuleBreach>.Ignored)).Returns(100);

            ruleViolationService.AddRuleViolation(this.ruleBreach);
            ruleViolationService.ProcessRuleViolationCache();

            A.CallTo(() => this.ruleBreachToRuleBreachMapper.RuleBreachItem(this.ruleBreach))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleBreachRepository.Create(A<RuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => this.ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(
                    A<IRuleBreach>.Ignored,
                    A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleBreachRepository.HasDuplicate(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.queueCasePublisher.Send(A<CaseMessage>.Ignored)).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The add rule violation rule breach only sends once for multiple calls.
        /// </summary>
        [Test]
        public void AddRuleViolationRuleBreachOnlySendsOnceForMultipleCalls()
        {
            var ruleViolationService = new RuleViolationService(
                this.queueCasePublisher,
                this.ruleBreachRepository,
                this.ruleBreachOrdersRepository,
                this.ruleBreachToRuleBreachOrdersMapper,
                this.ruleBreachToRuleBreachMapper,
                this.logger);

            var tradePosition = new TradePosition(new List<Order> { OrderHelper.Orders(OrderStatus.Filled) });
            A.CallTo(() => this.ruleBreach.Trades).Returns(tradePosition);
            A.CallTo(() => this.ruleBreach.RuleParameters.TunedParameters).Returns(null);
            A.CallTo(() => this.ruleBreachRepository.Create(A<RuleBreach>.Ignored)).Returns(100);

            ruleViolationService.AddRuleViolation(this.ruleBreach);
            ruleViolationService.ProcessRuleViolationCache();
            ruleViolationService.ProcessRuleViolationCache();

            A.CallTo(() => this.ruleBreachToRuleBreachMapper.RuleBreachItem(this.ruleBreach))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleBreachRepository.Create(A<RuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => this.ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(
                    A<IRuleBreach>.Ignored,
                    A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleBreachRepository.HasDuplicate(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.queueCasePublisher.Send(A<CaseMessage>.Ignored)).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The add rule violation rule breach for tuned parameter adds to rule breach repository but does not send.
        /// </summary>
        [Test]
        public void AddRuleViolationRuleBreachForTunedParameterAddsToRuleBreachRepositoryButDoesNotSend()
        {
            var ruleViolationService = new RuleViolationService(
                this.queueCasePublisher,
                this.ruleBreachRepository,
                this.ruleBreachOrdersRepository,
                this.ruleBreachToRuleBreachOrdersMapper,
                this.ruleBreachToRuleBreachMapper,
                this.logger);

            var tradePosition = new TradePosition(new List<Order> { OrderHelper.Orders(OrderStatus.Filled) });
            A.CallTo(() => this.ruleBreach.Trades).Returns(tradePosition);
            A.CallTo(() => this.ruleBreachRepository.Create(A<RuleBreach>.Ignored)).Returns(100);

            ruleViolationService.AddRuleViolation(this.ruleBreach);
            ruleViolationService.ProcessRuleViolationCache();

            A.CallTo(() => this.ruleBreachToRuleBreachMapper.RuleBreachItem(this.ruleBreach))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleBreachRepository.Create(A<RuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => this.ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(
                    A<IRuleBreach>.Ignored,
                    A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.ruleBreachRepository.HasDuplicate(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.queueCasePublisher.Send(A<CaseMessage>.Ignored)).MustNotHaveHappened();
        }

        /// <summary>
        /// The add rule violation rule breach with no orders adds to rule breach repository.
        /// </summary>
        [Test]
        public void AddRuleViolationRuleBreachWithNoOrdersAddsToRuleBreachRepository()
        {
            var ruleViolationService = new RuleViolationService(
                this.queueCasePublisher,
                this.ruleBreachRepository,
                this.ruleBreachOrdersRepository,
                this.ruleBreachToRuleBreachOrdersMapper,
                this.ruleBreachToRuleBreachMapper,
                this.logger);

            ruleViolationService.AddRuleViolation(this.ruleBreach);
            ruleViolationService.ProcessRuleViolationCache();

            A.CallTo(() => this.ruleBreachToRuleBreachMapper.RuleBreachItem(this.ruleBreach)).MustNotHaveHappened();
            A.CallTo(() => this.ruleBreachRepository.Create(A<RuleBreach>.Ignored)).MustNotHaveHappened();
            A.CallTo(
                () => this.ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(
                    A<IRuleBreach>.Ignored,
                    A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this.ruleBreachRepository.HasDuplicate(A<string>.Ignored)).MustNotHaveHappened();
        }

        /// <summary>
        /// The constructor null logger is exceptional.
        /// </summary>
        [Test]
        public void ConstructorNullLoggerIsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationService(
                    this.queueCasePublisher,
                    this.ruleBreachRepository,
                    this.ruleBreachOrdersRepository,
                    this.ruleBreachToRuleBreachOrdersMapper,
                    this.ruleBreachToRuleBreachMapper,
                    null));
        }

        /// <summary>
        /// The constructor null queue case publisher is exceptional.
        /// </summary>
        [Test]
        public void ConstructorNullQueueCasePublisherIsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationService(
                    null,
                    this.ruleBreachRepository,
                    this.ruleBreachOrdersRepository,
                    this.ruleBreachToRuleBreachOrdersMapper,
                    this.ruleBreachToRuleBreachMapper,
                    this.logger));
        }

        /// <summary>
        /// The constructor null rule breach mapper is exceptional.
        /// </summary>
        [Test]
        public void ConstructorNullRuleBreachMapperIsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationService(
                    this.queueCasePublisher,
                    this.ruleBreachRepository,
                    this.ruleBreachOrdersRepository,
                    this.ruleBreachToRuleBreachOrdersMapper,
                    null,
                    this.logger));
        }

        /// <summary>
        /// The constructor null rule breach orders mapper is exceptional.
        /// </summary>
        [Test]
        public void ConstructorNullRuleBreachOrdersMapperIsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationService(
                    this.queueCasePublisher,
                    this.ruleBreachRepository,
                    this.ruleBreachOrdersRepository,
                    null,
                    this.ruleBreachToRuleBreachMapper,
                    this.logger));
        }

        /// <summary>
        /// The constructor null rule breach orders repository is exceptional.
        /// </summary>
        [Test]
        public void ConstructorNullRuleBreachOrdersRepositoryIsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationService(
                    this.queueCasePublisher,
                    this.ruleBreachRepository,
                    null,
                    this.ruleBreachToRuleBreachOrdersMapper,
                    this.ruleBreachToRuleBreachMapper,
                    this.logger));
        }

        /// <summary>
        /// The constructor null rule breach repository is exceptional.
        /// </summary>
        [Test]
        public void ConstructorNullRuleBreachRepositoryIsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationService(
                    this.queueCasePublisher,
                    null,
                    this.ruleBreachOrdersRepository,
                    this.ruleBreachToRuleBreachOrdersMapper,
                    this.ruleBreachToRuleBreachMapper,
                    this.logger));
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.queueCasePublisher = A.Fake<IQueueCasePublisher>();
            this.ruleBreachRepository = A.Fake<IRuleBreachRepository>();
            this.ruleBreach = A.Fake<IRuleBreach>();
            this.ruleBreachOrdersRepository = A.Fake<IRuleBreachOrdersRepository>();
            this.ruleBreachToRuleBreachOrdersMapper = A.Fake<IRuleBreachToRuleBreachOrdersMapper>();
            this.ruleBreachToRuleBreachMapper = A.Fake<IRuleBreachToRuleBreachMapper>();
            this.logger = new NullLogger<RuleViolationService>();
        }
    }
}