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
        private IHighProfitJudgementMapper _highProfitJudgementMapper;

        private IJudgementRepository _judgementRepository;

        private ILogger<JudgementService> _logger;

        private IRuleViolationServiceFactory _ruleViolationServiceFactory;

        [Test]
        public void Build_ReturnsValidService_CallsFactoryBuild()
        {
            var serviceFactory = new JudgementServiceFactory(
                this._ruleViolationServiceFactory,
                this._judgementRepository,
                this._highProfitJudgementMapper,
                this._logger);
            var result = serviceFactory.Build();

            A.CallTo(() => this._ruleViolationServiceFactory.Build()).MustHaveHappenedOnceExactly();
            Assert.IsNotNull(result);
        }

        [Test]
        public void Ctor_NullHighProfitJudgementMapper_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementServiceFactory(
                    this._ruleViolationServiceFactory,
                    this._judgementRepository,
                    null,
                    this._logger));
        }

        [Test]
        public void Ctor_NullJudgementRepository_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementServiceFactory(
                    this._ruleViolationServiceFactory,
                    null,
                    this._highProfitJudgementMapper,
                    this._logger));
        }

        [Test]
        public void Ctor_NullLogger_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementServiceFactory(
                    this._ruleViolationServiceFactory,
                    this._judgementRepository,
                    this._highProfitJudgementMapper,
                    null));
        }

        [Test]
        public void Ctor_NullRuleViolationServiceFactory_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new JudgementServiceFactory(
                    null,
                    this._judgementRepository,
                    this._highProfitJudgementMapper,
                    this._logger));
        }

        [SetUp]
        public void Setup()
        {
            this._ruleViolationServiceFactory = A.Fake<IRuleViolationServiceFactory>();
            this._judgementRepository = A.Fake<IJudgementRepository>();
            this._highProfitJudgementMapper = A.Fake<IHighProfitJudgementMapper>();
            this._logger = A.Fake<ILogger<JudgementService>>();
        }
    }
}