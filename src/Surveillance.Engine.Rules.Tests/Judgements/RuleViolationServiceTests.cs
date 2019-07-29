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

namespace Surveillance.Engine.Rules.Tests.Judgements
{
    [TestFixture]
    public class RuleViolationServiceTests
    {
        private IQueueCasePublisher _queueCasePublisher;
        private IRuleBreachRepository _ruleBreachRepository;
        private IRuleBreach _ruleBreach;
        private IRuleBreachOrdersRepository _ruleBreachOrdersRepository;
        private IRuleBreachToRuleBreachOrdersMapper _ruleBreachToRuleBreachOrdersMapper;
        private IRuleBreachToRuleBreachMapper _ruleBreachToRuleBreachMapper;
        private ILogger<RuleViolationService> _logger;

        [SetUp]
        public void Setup()
        {
            _queueCasePublisher = A.Fake<IQueueCasePublisher>();
            _ruleBreachRepository = A.Fake<IRuleBreachRepository>();
            _ruleBreach = A.Fake<IRuleBreach>();
            _ruleBreachOrdersRepository = A.Fake<IRuleBreachOrdersRepository>();
            _ruleBreachToRuleBreachOrdersMapper = A.Fake<IRuleBreachToRuleBreachOrdersMapper>();
            _ruleBreachToRuleBreachMapper = A.Fake<IRuleBreachToRuleBreachMapper>();
            _logger = new NullLogger<RuleViolationService>();
        }

        [Test]
        public void Ctor_NullQueueCasePublisher_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleViolationService(null, _ruleBreachRepository, _ruleBreachOrdersRepository, _ruleBreachToRuleBreachOrdersMapper, _ruleBreachToRuleBreachMapper, _logger));
        }

        [Test]
        public void Ctor_NullRuleBreachRepository_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleViolationService(_queueCasePublisher, null, _ruleBreachOrdersRepository, _ruleBreachToRuleBreachOrdersMapper, _ruleBreachToRuleBreachMapper, _logger));
        }

        [Test]
        public void Ctor_NullRuleBreachOrdersRepository_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleViolationService(_queueCasePublisher, _ruleBreachRepository, null, _ruleBreachToRuleBreachOrdersMapper, _ruleBreachToRuleBreachMapper, _logger));
        }

        [Test]
        public void Ctor_NullRuleBreachOrdersMapper_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleViolationService(_queueCasePublisher, _ruleBreachRepository, _ruleBreachOrdersRepository, null, _ruleBreachToRuleBreachMapper, _logger));
        }

        [Test]
        public void Ctor_NullRuleBreachMapper_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleViolationService(_queueCasePublisher, _ruleBreachRepository, _ruleBreachOrdersRepository, _ruleBreachToRuleBreachOrdersMapper, null, _logger));
        }

        [Test]
        public void Ctor_NullLogger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleViolationService(_queueCasePublisher, _ruleBreachRepository, _ruleBreachOrdersRepository, _ruleBreachToRuleBreachOrdersMapper, _ruleBreachToRuleBreachMapper, null));
        }

        [Test]
        public void AddRuleViolation_NullRuleBreach_DoesNotThrow()
        {
            var ruleViolationService =
                new RuleViolationService(
                    _queueCasePublisher,
                    _ruleBreachRepository,
                    _ruleBreachOrdersRepository,
                    _ruleBreachToRuleBreachOrdersMapper,
                    _ruleBreachToRuleBreachMapper,
                    _logger);

            Assert.DoesNotThrow(() => ruleViolationService.AddRuleViolation(null));
        }

        [Test]
        public void AddRuleViolation_RuleBreachWithNoOrders_AddsToRuleBreachRepository()
        {
            var ruleViolationService =
                new RuleViolationService(
                    _queueCasePublisher,
                    _ruleBreachRepository,
                    _ruleBreachOrdersRepository,
                    _ruleBreachToRuleBreachOrdersMapper,
                    _ruleBreachToRuleBreachMapper,
                    _logger);

            ruleViolationService.AddRuleViolation(_ruleBreach);
            ruleViolationService.ProcessRuleViolationCache();

            A.CallTo(() => _ruleBreachToRuleBreachMapper.RuleBreachItem(_ruleBreach)).MustNotHaveHappened();
            A.CallTo(() => _ruleBreachRepository.Create(A<RuleBreach>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => _ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(A<IRuleBreach>.Ignored, A<string>.Ignored))
                .MustNotHaveHappened();
            A.CallTo(() => _ruleBreachRepository.HasDuplicate(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void AddRuleViolation_RuleBreachForTunedParam_AddsToRuleBreachRepositoryButDoesNotSend()
        {
            var ruleViolationService =
                new RuleViolationService(
                    _queueCasePublisher,
                    _ruleBreachRepository,
                    _ruleBreachOrdersRepository,
                    _ruleBreachToRuleBreachOrdersMapper,
                    _ruleBreachToRuleBreachMapper,
                    _logger);

            var tradePosition = new TradePosition(new List<Order> { OrderHelper.Orders(OrderStatus.Filled)});
            A.CallTo(() => _ruleBreach.Trades).Returns(tradePosition);
            A.CallTo(() => _ruleBreachRepository.Create(A<RuleBreach>.Ignored)).Returns(100);

            ruleViolationService.AddRuleViolation(_ruleBreach);
            ruleViolationService.ProcessRuleViolationCache();

            A.CallTo(() => _ruleBreachToRuleBreachMapper.RuleBreachItem(_ruleBreach)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleBreachRepository.Create(A<RuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(A<IRuleBreach>.Ignored, A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleBreachRepository.HasDuplicate(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _queueCasePublisher.Send(A<CaseMessage>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public void AddRuleViolation_RuleBreach_AddsToRuleBreachRepository()
        {
            var ruleViolationService =
                new RuleViolationService(
                    _queueCasePublisher,
                    _ruleBreachRepository,
                    _ruleBreachOrdersRepository,
                    _ruleBreachToRuleBreachOrdersMapper,
                    _ruleBreachToRuleBreachMapper,
                    _logger);

            var tradePosition = new TradePosition(new List<Order> { OrderHelper.Orders(OrderStatus.Filled) });
            A.CallTo(() => _ruleBreach.Trades).Returns(tradePosition);
            A.CallTo(() => _ruleBreach.RuleParameters.TunedParam).Returns(null);
            A.CallTo(() => _ruleBreachRepository.Create(A<RuleBreach>.Ignored)).Returns(100);
            
            ruleViolationService.AddRuleViolation(_ruleBreach);
            ruleViolationService.ProcessRuleViolationCache();

            A.CallTo(() => _ruleBreachToRuleBreachMapper.RuleBreachItem(_ruleBreach)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleBreachRepository.Create(A<RuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(A<IRuleBreach>.Ignored, A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleBreachRepository.HasDuplicate(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _queueCasePublisher.Send(A<CaseMessage>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void AddRuleViolation_RuleBreach_OnlySendsOnceForMultipleCalls()
        {
            var ruleViolationService =
                new RuleViolationService(
                    _queueCasePublisher,
                    _ruleBreachRepository,
                    _ruleBreachOrdersRepository,
                    _ruleBreachToRuleBreachOrdersMapper,
                    _ruleBreachToRuleBreachMapper,
                    _logger);

            var tradePosition = new TradePosition(new List<Order> { OrderHelper.Orders(OrderStatus.Filled) });
            A.CallTo(() => _ruleBreach.Trades).Returns(tradePosition);
            A.CallTo(() => _ruleBreach.RuleParameters.TunedParam).Returns(null);
            A.CallTo(() => _ruleBreachRepository.Create(A<RuleBreach>.Ignored)).Returns(100);

            ruleViolationService.AddRuleViolation(_ruleBreach);
            ruleViolationService.ProcessRuleViolationCache();
            ruleViolationService.ProcessRuleViolationCache();

            A.CallTo(() => _ruleBreachToRuleBreachMapper.RuleBreachItem(_ruleBreach)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleBreachRepository.Create(A<RuleBreach>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleBreachToRuleBreachOrdersMapper.ProjectToOrders(A<IRuleBreach>.Ignored, A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _ruleBreachRepository.HasDuplicate(A<string>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _queueCasePublisher.Send(A<CaseMessage>.Ignored)).MustHaveHappenedOnceExactly();
        }



    }
}
