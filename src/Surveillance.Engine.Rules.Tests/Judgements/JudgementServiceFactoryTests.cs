namespace Surveillance.Engine.Rules.Tests.Judgements
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Judgements;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;

    /// <summary>
    /// The judgement service factory tests.
    /// </summary>
    [TestFixture]
    public class JudgementServiceFactoryTests
    {
        /// <summary>
        /// The high profit judgement mapper.
        /// </summary>
        private IHighProfitJudgementMapper highProfitJudgementMapper;

        /// <summary>
        /// The fixed income judgement mapper.
        /// </summary>
        private IFixedIncomeHighProfitJudgementMapper fixedIncomeJudgementMapper;

        /// <summary>
        /// The fixed income high volume judgement mapper.
        /// </summary>
        private IFixedIncomeHighVolumeJudgementMapper fixedIncomeHighVolumeJudgementMapper;
        
        /// <summary>
        /// The judgement repository.
        /// </summary>
        private IJudgementRepository judgementRepository;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<JudgementService> logger;

        /// <summary>
        /// The rule violation service factory.
        /// </summary>
        private IRuleViolationServiceFactory ruleViolationServiceFactory;

        /// <summary>
        /// The build returns valid service calls factory build.
        /// </summary>
        [Test]
        public void BuildReturnsValidServiceCallsFactoryBuild()
        {
            var serviceFactory = new JudgementServiceFactory(
                this.ruleViolationServiceFactory,
                this.judgementRepository,
                this.highProfitJudgementMapper,
                this.fixedIncomeJudgementMapper,
                this.fixedIncomeHighVolumeJudgementMapper,
                this.logger);

            var result = serviceFactory.Build();

            A.CallTo(() => this.ruleViolationServiceFactory.Build()).MustHaveHappenedOnceExactly();
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// The constructor null high profit judgement mapper throws exception.
        /// </summary>
        [Test]
        public void ConstructorNullHighProfitJudgementMapperThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementServiceFactory(
                    this.ruleViolationServiceFactory,
                    this.judgementRepository,
                    null,
                    this.fixedIncomeJudgementMapper,
                    this.fixedIncomeHighVolumeJudgementMapper,
                    this.logger));
        }

        /// <summary>
        /// The constructor null judgement repository throws exception.
        /// </summary>
        [Test]
        public void ConstructorNullJudgementRepositoryThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementServiceFactory(
                    this.ruleViolationServiceFactory,
                    null,
                    this.highProfitJudgementMapper,
                    this.fixedIncomeJudgementMapper,
                    this.fixedIncomeHighVolumeJudgementMapper,
                    this.logger));
        }

        /// <summary>
        /// The constructor null logger throws exception.
        /// </summary>
        [Test]
        public void ConstructorNullLoggerThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementServiceFactory(
                    this.ruleViolationServiceFactory,
                    this.judgementRepository,
                    this.highProfitJudgementMapper,
                    this.fixedIncomeJudgementMapper,
                    this.fixedIncomeHighVolumeJudgementMapper,
                    null));
        }

        /// <summary>
        /// The constructor null rule violation service factory throws exception.
        /// </summary>
        [Test]
        public void ConstructorNullRuleViolationServiceFactoryThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementServiceFactory(
                    null,
                    this.judgementRepository,
                    this.highProfitJudgementMapper,
                    this.fixedIncomeJudgementMapper,
                    this.fixedIncomeHighVolumeJudgementMapper,
                    this.logger));
        }

        /// <summary>
        /// The constructor null fixed income high volume judgement mapper throws exception.
        /// </summary>
        [Test]
        public void ConstructorNullFixedIncomeHighVolumeJudgementMapperThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementServiceFactory(
                    null,
                    this.judgementRepository,
                    this.highProfitJudgementMapper,
                    this.fixedIncomeJudgementMapper,
                    null,
                    this.logger));
        }

        /// <summary>
        /// The build returns judgement service test.
        /// </summary>
        [Test]
        public void BuildReturnsJudgementService()
        {
            var factory = new JudgementServiceFactory(
                this.ruleViolationServiceFactory,
                this.judgementRepository,
                this.highProfitJudgementMapper,
                this.fixedIncomeJudgementMapper,
                this.fixedIncomeHighVolumeJudgementMapper,
                this.logger);

            Assert.DoesNotThrow(() => factory.Build());
            A.CallTo(() => this.ruleViolationServiceFactory.Build()).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.ruleViolationServiceFactory = A.Fake<IRuleViolationServiceFactory>();
            this.fixedIncomeJudgementMapper = A.Fake<IFixedIncomeHighProfitJudgementMapper>();
            this.judgementRepository = A.Fake<IJudgementRepository>();
            this.highProfitJudgementMapper = A.Fake<IHighProfitJudgementMapper>();
            this.fixedIncomeHighVolumeJudgementMapper = A.Fake<IFixedIncomeHighVolumeJudgementMapper>();
            this.logger = A.Fake<ILogger<JudgementService>>();
        }
    }
}