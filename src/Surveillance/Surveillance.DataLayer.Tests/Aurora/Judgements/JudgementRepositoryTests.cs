namespace Surveillance.DataLayer.Tests.Aurora.Judgements
{
    using Domain.Surveillance.Judgement.Equity;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.DataLayer.Aurora;
    using Surveillance.DataLayer.Aurora.Judgements;
    using Surveillance.DataLayer.Configuration.Interfaces;
    using Surveillance.DataLayer.Tests.Helpers;
    using System.Threading.Tasks;

    [TestFixture]
    public class JudgementRepositoryTests
    {
        private IDataLayerConfiguration _configuration;

        private ILogger<JudgementRepository> _logger;

        [Test]
        [Explicit("db integration")]
        public async Task SaveHighProfit_SavesHighProfit_ToDb()
        {
            var connectionStringFactory = new ConnectionStringFactory(this._configuration);
            var repo = new JudgementRepository(connectionStringFactory, this._logger);

            var hpJudgement = new HighProfitJudgement(
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

            await repo.SaveAsync(hpJudgement);

            Assert.IsTrue(true);
        }

        [SetUp]
        public void Setup()
        {
            this._configuration = TestHelpers.Config();
            this._logger = new NullLogger<JudgementRepository>();
        }
    }
}