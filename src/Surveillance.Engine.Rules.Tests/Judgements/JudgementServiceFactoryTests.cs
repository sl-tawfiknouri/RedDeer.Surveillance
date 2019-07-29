using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora.Judgements.Interfaces;
using Surveillance.Engine.Rules.Judgements;
using Surveillance.Engine.Rules.Judgements.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Judgements
{
    [TestFixture]
    public class JudgementServiceFactoryTests
    {
        private IRuleViolationServiceFactory _ruleViolationServiceFactory;
        private IJudgementRepository _judgementRepository;
        private IHighProfitJudgementMapper _highProfitJudgementMapper;
        private ILogger<JudgementService> _logger;

        [SetUp]
        public void Setup()
        {
            _ruleViolationServiceFactory = A.Fake<IRuleViolationServiceFactory>();
            _judgementRepository = A.Fake<IJudgementRepository>();
            _highProfitJudgementMapper = A.Fake<IHighProfitJudgementMapper>();
            _logger = A.Fake<ILogger<JudgementService>>();
        }

        [Test]
        public void Ctor_NullRuleViolationServiceFactory_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new JudgementServiceFactory(null, _judgementRepository, _highProfitJudgementMapper, _logger));
        }

        [Test]
        public void Ctor_NullJudgementRepository_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new JudgementServiceFactory(_ruleViolationServiceFactory, null, _highProfitJudgementMapper, _logger));
        }

        [Test]
        public void Ctor_NullHighProfitJudgementMapper_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new JudgementServiceFactory(_ruleViolationServiceFactory, _judgementRepository, null, _logger));
        }

        [Test]
        public void Ctor_NullLogger_ThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new JudgementServiceFactory(_ruleViolationServiceFactory, _judgementRepository, _highProfitJudgementMapper, null));
        }

        [Test]
        public void Build_ReturnsValidService_CallsFactoryBuild()
        {
            var serviceFactory = new JudgementServiceFactory(_ruleViolationServiceFactory, _judgementRepository, _highProfitJudgementMapper, _logger);
            var result = serviceFactory.Build();

            A.CallTo(() => _ruleViolationServiceFactory.Build()).MustHaveHappenedOnceExactly();
            Assert.IsNotNull(result);
        }
    }
}
