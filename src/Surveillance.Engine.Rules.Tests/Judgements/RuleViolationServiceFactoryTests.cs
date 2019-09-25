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

    /// <summary>
    /// The rule violation service factory tests.
    /// </summary>
    [TestFixture]
    public class RuleViolationServiceFactoryTests
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
        /// The build rule violation service returns non null.
        /// </summary>
        [Test]
        public void BuildRuleViolationServiceReturnsNonNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var factory = new RuleViolationServiceFactory(
                this.queueCasePublisher,
                this.ruleBreachRepository,
                this.ruleBreachOrdersRepository,
                this.ruleBreachToRuleBreachOrdersMapper,
                this.ruleBreachToRuleBreachMapper,
                this.logger);

            var ruleViolationService = factory.Build();

            Assert.IsNotNull(ruleViolationService);
        }

        /// <summary>
        /// The constructor null logger is exceptional.
        /// </summary>
        [Test]
        public void ConstructorNullLoggerIsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationServiceFactory(
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
                () => new RuleViolationServiceFactory(
                    null,
                    this.ruleBreachRepository,
                    this.ruleBreachOrdersRepository,
                    this.ruleBreachToRuleBreachOrdersMapper,
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
                () => new RuleViolationServiceFactory(
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
                () => new RuleViolationServiceFactory(
                    this.queueCasePublisher,
                    null,
                    this.ruleBreachOrdersRepository,
                    this.ruleBreachToRuleBreachOrdersMapper,
                    this.ruleBreachToRuleBreachMapper,
                    this.logger));
        }

        /// <summary>
        /// The constructor null rule breach to rule breach mapper is exceptional.
        /// </summary>
        [Test]
        public void ConstructorNullRuleBreachToRuleBreachMapperIsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationServiceFactory(
                    this.queueCasePublisher,
                    this.ruleBreachRepository,
                    this.ruleBreachOrdersRepository,
                    this.ruleBreachToRuleBreachOrdersMapper,
                    null,
                    this.logger));
        }

        /// <summary>
        /// The constructor null rule breach to rule breach orders mapper is exceptional.
        /// </summary>
        [Test]
        public void ConstructorNullRuleBreachToRuleBreachOrdersMapperIsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleViolationServiceFactory(
                    this.queueCasePublisher,
                    this.ruleBreachRepository,
                    this.ruleBreachOrdersRepository,
                    null,
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
            this.ruleBreachOrdersRepository = A.Fake<IRuleBreachOrdersRepository>();
            this.ruleBreachToRuleBreachOrdersMapper = A.Fake<IRuleBreachToRuleBreachOrdersMapper>();
            this.ruleBreachToRuleBreachMapper = A.Fake<IRuleBreachToRuleBreachMapper>();
            this.logger = A.Fake<ILogger<RuleViolationService>>();
        }
    }
}