using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora.Rules.Interfaces;
using Surveillance.Engine.Rules.Judgements;
using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
using Surveillance.Engine.Rules.Queues.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Judgements
{
    [TestFixture]
    public class RuleViolationServiceFactoryTests
    {
        private IQueueCasePublisher _queueCasePublisher;
        private IRuleBreachRepository _ruleBreachRepository;
        private IRuleBreachOrdersRepository _ruleBreachOrdersRepository;
        private IRuleBreachToRuleBreachOrdersMapper _ruleBreachToRuleBreachOrdersMapper;
        private IRuleBreachToRuleBreachMapper _ruleBreachToRuleBreachMapper;
        private ILogger<RuleViolationService> _logger;

        [SetUp]
        public void Setup()
        {
            _queueCasePublisher = A.Fake<IQueueCasePublisher>();
            _ruleBreachRepository = A.Fake<IRuleBreachRepository>();
            _ruleBreachOrdersRepository = A.Fake<IRuleBreachOrdersRepository>();
            _ruleBreachToRuleBreachOrdersMapper = A.Fake<IRuleBreachToRuleBreachOrdersMapper>();
            _ruleBreachToRuleBreachMapper = A.Fake<IRuleBreachToRuleBreachMapper>();
            _logger = A.Fake<ILogger<RuleViolationService>>();
        }

        [Test]
        public void Ctor_NullQueueCasePublisher_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleViolationServiceFactory(null, _ruleBreachRepository, _ruleBreachOrdersRepository, _ruleBreachToRuleBreachOrdersMapper, _ruleBreachToRuleBreachMapper, _logger));
        }

        [Test]
        public void Ctor_NullRuleBreachRepository_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleViolationServiceFactory(_queueCasePublisher, null, _ruleBreachOrdersRepository, _ruleBreachToRuleBreachOrdersMapper, _ruleBreachToRuleBreachMapper, _logger));
        }

        [Test]
        public void Ctor_NullRuleBreachOrdersRepository_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleViolationServiceFactory(_queueCasePublisher, _ruleBreachRepository, null, _ruleBreachToRuleBreachOrdersMapper, _ruleBreachToRuleBreachMapper, _logger));
        }

        [Test]
        public void Ctor_NullRuleBreachToRuleBreachOrdersMapper_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleViolationServiceFactory(_queueCasePublisher, _ruleBreachRepository, _ruleBreachOrdersRepository, null, _ruleBreachToRuleBreachMapper, _logger));
        }

        [Test]
        public void Ctor_NullRuleBreachToRuleBreachMapper_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleViolationServiceFactory(_queueCasePublisher, _ruleBreachRepository, _ruleBreachOrdersRepository, _ruleBreachToRuleBreachOrdersMapper, null, _logger));
        }

        [Test]
        public void Ctor_NullLogger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleViolationServiceFactory(_queueCasePublisher, _ruleBreachRepository, _ruleBreachOrdersRepository, _ruleBreachToRuleBreachOrdersMapper, _ruleBreachToRuleBreachMapper, null));
        }

        [Test]
        public void Build_RuleViolationService_ReturnsNonNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var factory =
                new RuleViolationServiceFactory(
                    _queueCasePublisher,
                    _ruleBreachRepository,
                    _ruleBreachOrdersRepository,
                    _ruleBreachToRuleBreachOrdersMapper,
                    _ruleBreachToRuleBreachMapper,
                    _logger);

            var ruleViolationService = factory.Build();

            Assert.IsNotNull(ruleViolationService);
        }

    }
}
