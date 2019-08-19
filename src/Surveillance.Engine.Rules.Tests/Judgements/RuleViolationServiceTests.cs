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

    [TestFixture]
    public class RuleViolationServiceTests
    {
        private ILogger<RuleViolationService> _logger;

        private IQueueCasePublisher _queueCasePublisher;

        private IRuleBreach _ruleBreach;

        private IRuleBreachOrdersRepository _ruleBreachOrdersRepository;

        private IRuleBreachRepository _ruleBreachRepository;

        private IRuleBreachToRuleBreachMapper _ruleBreachToRuleBreachMapper;

        private IRuleBreachToRuleBreachOrdersMapper _ruleBreachToRuleBreachOrdersMapper;

        [Test]
        public void AddRuleViolation_NullRuleBreach_DoesNotThrow()
        {
            var ruleViolationService = new RuleViolationService(
                this._queueCasePublisher,
                this._ruleBreachRepository,
                this._ruleBreachOrdersRepository,
                this._ruleBreachToRuleBreachOrdersMapper,
                this._ruleBreachToRuleBreachMapper,
                this._logger);

            Assert.DoesNotThrow(() => ruleViolationService.AddRuleViolation(null));
        }

        [Test]
        public void AddRuleViolation_RuleBreach_AddsToRuleBreachRepository()
        {
            var ruleViolationService = new RuleViolationService(
                this._queueCasePublisher,
                this._ruleBreachRepository,
                this._ruleBreachOrdersRepository,
                this._ruleBreachToRuleBreachOrdersMapper,
                this._ruleBreachToRuleBreachMapper,
                this._logger);

            var tradePosition = new TradePosition(new List<Order> { OrderHelper.Orders(OrderStatus.Filled) });
            A.CallTo(() => this._ruleBreach.Trades).Returns(tradePosition);
            A.CallTo(() => this._ruleBreach.RuleParameters.TunedParam).Returns(null);
            A.CallTo(() => this._ruleBreachRepository.Create(A<RuleBreach>.Ignored)).Returns(100);

            ruleViolationService.AddRuleViolation(this._ruleBreach);
            ruleViolationService.ProcessRuleViolationCache();

            A.CallTo(() => this._ruleBreachToRuleBreachMapper.RuleBreachItem(this._ruleBreach))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleBreachRepository.Create(A<RuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => this._ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(
                    A<IRuleBreach>.Ignored,
                    A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleBreachRepository.HasDuplicate(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._queueCasePublisher.Send(A<CaseMessage>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void AddRuleViolation_RuleBreach_OnlySendsOnceForMultipleCalls()
        {
            var ruleViolationService = new RuleViolationService(
                this._queueCasePublisher,
                this._ruleBreachRepository,
                this._ruleBreachOrdersRepository,
                this._ruleBreachToRuleBreachOrdersMapper,
                this._ruleBreachToRuleBreachMapper,
                this._logger);

            var tradePosition = new TradePosition(new List<Order> { OrderHelper.Orders(OrderStatus.Filled) });
            A.CallTo(() => this._ruleBreach.Trades).Returns(tradePosition);
            A.CallTo(() => this._ruleBreach.RuleParameters.TunedParam).Returns(null);
            A.CallTo(() => this._ruleBreachRepository.Create(A<RuleBreach>.Ignored)).Returns(100);

            ruleViolationService.AddRuleViolation(this._ruleBreach);
            ruleViolationService.ProcessRuleViolationCache();
            ruleViolationService.ProcessRuleViolationCache();

            A.CallTo(() => this._ruleBreachToRuleBreachMapper.RuleBreachItem(this._ruleBreach))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleBreachRepository.Create(A<RuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => this._ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(
                    A<IRuleBreach>.Ignored,
                    A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleBreachRepository.HasDuplicate(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._queueCasePublisher.Send(A<CaseMessage>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void AddRuleViolation_RuleBreachForTunedParam_AddsToRuleBreachRepositoryButDoesNotSend()
        {
            var ruleViolationService = new RuleViolationService(
                this._queueCasePublisher,
                this._ruleBreachRepository,
                this._ruleBreachOrdersRepository,
                this._ruleBreachToRuleBreachOrdersMapper,
                this._ruleBreachToRuleBreachMapper,
                this._logger);

            var tradePosition = new TradePosition(new List<Order> { OrderHelper.Orders(OrderStatus.Filled) });
            A.CallTo(() => this._ruleBreach.Trades).Returns(tradePosition);
            A.CallTo(() => this._ruleBreachRepository.Create(A<RuleBreach>.Ignored)).Returns(100);

            ruleViolationService.AddRuleViolation(this._ruleBreach);
            ruleViolationService.ProcessRuleViolationCache();

            A.CallTo(() => this._ruleBreachToRuleBreachMapper.RuleBreachItem(this._ruleBreach))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleBreachRepository.Create(A<RuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(
                () => this._ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(
                    A<IRuleBreach>.Ignored,
                    A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._ruleBreachRepository.HasDuplicate(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._queueCasePublisher.Send(A<CaseMessage>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void AddRuleViolation_RuleBreachWithNoOrders_AddsToRuleBreachRepository()
        {
            var ruleViolationService = new RuleViolationService(
                this._queueCasePublisher,
                this._ruleBreachRepository,
                this._ruleBreachOrdersRepository,
                this._ruleBreachToRuleBreachOrdersMapper,
                this._ruleBreachToRuleBreachMapper,
                this._logger);

            ruleViolationService.AddRuleViolation(this._ruleBreach);
            ruleViolationService.ProcessRuleViolationCache();

            A.CallTo(() => this._ruleBreachToRuleBreachMapper.RuleBreachItem(this._ruleBreach)).MustNotHaveHappened();
            A.CallTo(() => this._ruleBreachRepository.Create(A<RuleBreach>.Ignored)).MustNotHaveHappened();
            A.CallTo(
                () => this._ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(
                    A<IRuleBreach>.Ignored,
                    A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => this._ruleBreachRepository.HasDuplicate(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void Ctor_NullLogger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationService(
                    this._queueCasePublisher,
                    this._ruleBreachRepository,
                    this._ruleBreachOrdersRepository,
                    this._ruleBreachToRuleBreachOrdersMapper,
                    this._ruleBreachToRuleBreachMapper,
                    null));
        }

        [Test]
        public void Ctor_NullQueueCasePublisher_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationService(
                    null,
                    this._ruleBreachRepository,
                    this._ruleBreachOrdersRepository,
                    this._ruleBreachToRuleBreachOrdersMapper,
                    this._ruleBreachToRuleBreachMapper,
                    this._logger));
        }

        [Test]
        public void Ctor_NullRuleBreachMapper_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationService(
                    this._queueCasePublisher,
                    this._ruleBreachRepository,
                    this._ruleBreachOrdersRepository,
                    this._ruleBreachToRuleBreachOrdersMapper,
                    null,
                    this._logger));
        }

        [Test]
        public void Ctor_NullRuleBreachOrdersMapper_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationService(
                    this._queueCasePublisher,
                    this._ruleBreachRepository,
                    this._ruleBreachOrdersRepository,
                    null,
                    this._ruleBreachToRuleBreachMapper,
                    this._logger));
        }

        [Test]
        public void Ctor_NullRuleBreachOrdersRepository_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationService(
                    this._queueCasePublisher,
                    this._ruleBreachRepository,
                    null,
                    this._ruleBreachToRuleBreachOrdersMapper,
                    this._ruleBreachToRuleBreachMapper,
                    this._logger));
        }

        [Test]
        public void Ctor_NullRuleBreachRepository_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationService(
                    this._queueCasePublisher,
                    null,
                    this._ruleBreachOrdersRepository,
                    this._ruleBreachToRuleBreachOrdersMapper,
                    this._ruleBreachToRuleBreachMapper,
                    this._logger));
        }

        [SetUp]
        public void Setup()
        {
            this._queueCasePublisher = A.Fake<IQueueCasePublisher>();
            this._ruleBreachRepository = A.Fake<IRuleBreachRepository>();
            this._ruleBreach = A.Fake<IRuleBreach>();
            this._ruleBreachOrdersRepository = A.Fake<IRuleBreachOrdersRepository>();
            this._ruleBreachToRuleBreachOrdersMapper = A.Fake<IRuleBreachToRuleBreachOrdersMapper>();
            this._ruleBreachToRuleBreachMapper = A.Fake<IRuleBreachToRuleBreachMapper>();
            this._logger = new NullLogger<RuleViolationService>();
        }
    }
}