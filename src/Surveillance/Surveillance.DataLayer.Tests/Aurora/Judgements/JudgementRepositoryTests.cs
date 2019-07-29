using Domain.Surveillance.Judgement.Equity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Judgements;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.Tests.Helpers;

namespace Surveillance.DataLayer.Tests.Aurora.Judgements
{
    [TestFixture]
    public class JudgementRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private ILogger<JudgementRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _configuration = TestHelpers.Config();
            _logger = new NullLogger<JudgementRepository>();
        }

        [Test]
        [Explicit("db integration")]
        public void SaveHighProfit_SavesHighProfit_ToDb()
        {
            var connectionStringFactory = new ConnectionStringFactory(_configuration);
            var repo = new JudgementRepository(connectionStringFactory, _logger);

            var hpJudgement =
                new HighProfitJudgement(
                    "rule-1",
                    "rule-correlation-1",
                    "order100",
                    "ABCD-100",
                    100,
                    "EUR",
                    92,
                    "some params",
                    false,
                    false);

            repo.Save(hpJudgement).Wait();

            Assert.IsTrue(true);
        }       
    }
}
