namespace Surveillance.Engine.Rules.Tests.Judgements
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.DataLayer.Aurora.Rules.Interfaces;
    using Surveillance.Engine.Rules.Judgements;
    using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
    using Surveillance.Engine.Rules.Queues.Interfaces;

    [TestFixture]
    public class RuleViolationServiceFactoryTests
    {
        private ILogger<RuleViolationService> _logger;

        private IQueueCasePublisher _queueCasePublisher;

        private IRuleBreachOrdersRepository _ruleBreachOrdersRepository;

        private IRuleBreachRepository _ruleBreachRepository;

        private IRuleBreachToRuleBreachMapper _ruleBreachToRuleBreachMapper;

        private IRuleBreachToRuleBreachOrdersMapper _ruleBreachToRuleBreachOrdersMapper;

        [Test]
        public void Build_RuleViolationService_ReturnsNonNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var factory = new RuleViolationServiceFactory(
                this._queueCasePublisher,
                this._ruleBreachRepository,
                this._ruleBreachOrdersRepository,
                this._ruleBreachToRuleBreachOrdersMapper,
                this._ruleBreachToRuleBreachMapper,
                this._logger);

            var ruleViolationService = factory.Build();

            Assert.IsNotNull(ruleViolationService);
        }

        [Test]
        public void Ctor_NullLogger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationServiceFactory(
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
                () => new RuleViolationServiceFactory(
                    null,
                    this._ruleBreachRepository,
                    this._ruleBreachOrdersRepository,
                    this._ruleBreachToRuleBreachOrdersMapper,
                    this._ruleBreachToRuleBreachMapper,
                    this._logger));
        }

        [Test]
        public void Ctor_NullRuleBreachOrdersRepository_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationServiceFactory(
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
                () => new RuleViolationServiceFactory(
                    this._queueCasePublisher,
                    null,
                    this._ruleBreachOrdersRepository,
                    this._ruleBreachToRuleBreachOrdersMapper,
                    this._ruleBreachToRuleBreachMapper,
                    this._logger));
        }

        [Test]
        public void Ctor_NullRuleBreachToRuleBreachMapper_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationServiceFactory(
                    this._queueCasePublisher,
                    this._ruleBreachRepository,
                    this._ruleBreachOrdersRepository,
                    this._ruleBreachToRuleBreachOrdersMapper,
                    null,
                    this._logger));
        }

        [Test]
        public void Ctor_NullRuleBreachToRuleBreachOrdersMapper_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationServiceFactory(
                    this._queueCasePublisher,
                    this._ruleBreachRepository,
                    this._ruleBreachOrdersRepository,
                    null,
                    this._ruleBreachToRuleBreachMapper,
                    this._logger));
        }

        [SetUp]
        public void Setup()
        {
            this._queueCasePublisher = A.Fake<IQueueCasePublisher>();
            this._ruleBreachRepository = A.Fake<IRuleBreachRepository>();
            this._ruleBreachOrdersRepository = A.Fake<IRuleBreachOrdersRepository>();
            this._ruleBreachToRuleBreachOrdersMapper = A.Fake<IRuleBreachToRuleBreachOrdersMapper>();
            this._ruleBreachToRuleBreachMapper = A.Fake<IRuleBreachToRuleBreachMapper>();
            this._logger = A.Fake<ILogger<RuleViolationService>>();
        }
    }
}