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

    [TestFixture]
    public class JudgementServiceFactoryTests
    {
        private IHighProfitJudgementMapper highProfitJudgementMapper;

        private IFixedIncomeHighProfitJudgementMapper fixedIncomeJudgementMapper;

        private IJudgementRepository judgementRepository;

        private ILogger<JudgementService> logger;

        private IRuleViolationServiceFactory ruleViolationServiceFactory;

        [Test]
        public void Build_ReturnsValidService_CallsFactoryBuild()
        {
            var serviceFactory = new JudgementServiceFactory(
                this.ruleViolationServiceFactory,
                this.judgementRepository,
                this.highProfitJudgementMapper,
                this.fixedIncomeJudgementMapper,
                this.logger);
            var result = serviceFactory.Build();

            A.CallTo(() => this.ruleViolationServiceFactory.Build()).MustHaveHappenedOnceExactly();
            Assert.IsNotNull(result);
        }

        [Test]
        public void Ctor_NullHighProfitJudgementMapper_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementServiceFactory(
                    this.ruleViolationServiceFactory,
                    this.judgementRepository,
                    null,
                    this.fixedIncomeJudgementMapper,
                    this.logger));
        }

        [Test]
        public void Ctor_NullJudgementRepository_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementServiceFactory(
                    this.ruleViolationServiceFactory,
                    null,
                    this.highProfitJudgementMapper,
                    this.fixedIncomeJudgementMapper,
                    this.logger));
        }

        [Test]
        public void Ctor_NullLogger_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementServiceFactory(
                    this.ruleViolationServiceFactory,
                    this.judgementRepository,
                    this.highProfitJudgementMapper,
                    this.fixedIncomeJudgementMapper,
                    null));
        }

        [Test]
        public void Ctor_NullRuleViolationServiceFactory_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementServiceFactory(
                    null,
                    this.judgementRepository,
                    this.highProfitJudgementMapper,
                    this.fixedIncomeJudgementMapper,
                    this.logger));
        }

        [SetUp]
        public void Setup()
        {
            this.ruleViolationServiceFactory = A.Fake<IRuleViolationServiceFactory>();
            this.fixedIncomeJudgementMapper = A.Fake<IFixedIncomeHighProfitJudgementMapper>();
            this.judgementRepository = A.Fake<IJudgementRepository>();
            this.highProfitJudgementMapper = A.Fake<IHighProfitJudgementMapper>();
            this.logger = A.Fake<ILogger<JudgementService>>();
        }
    }
}