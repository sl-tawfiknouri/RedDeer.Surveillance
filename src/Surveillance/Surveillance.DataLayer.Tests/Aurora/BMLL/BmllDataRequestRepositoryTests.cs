using System;
using System.Threading.Tasks;
using DomainV2.Financial;
using DomainV2.Markets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.BMLL;
using Surveillance.DataLayer.Configuration;

namespace Surveillance.DataLayer.Tests.Aurora.BMLL
{
    [TestFixture]
    public class BmllDataRequestRepositoryTests
    {
        private ILogger<RuleRunDataRequestRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new NullLogger<RuleRunDataRequestRepository>();
        }

        [Test]
        [Explicit]
        public async Task CreateDataRequest_SavesToDb()
        {
            var config = new DataLayerConfiguration
            {
                AuroraConnectionString = "server=dev-temporary.cgedh3fdlw42.eu-west-1.rds.amazonaws.com; port=3306;uid=hackinguser;pwd='WillDelete3101';database=hackingdb1; Allow User Variables=True"
            };

            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(config), _logger);

            var marketDataRequest =
                new MarketDataRequest(
                    null,
                    "XLON",
                    "entsbp",
                    new InstrumentIdentifiers { Id = "1" },
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(1),
                    "2",
                    true);

            await repo.CreateDataRequest(marketDataRequest);
        }

        [Test]
        [Explicit]
        public async Task GetDataRequests_FetchesFromDb()
        {
            var config = new DataLayerConfiguration
            {
                AuroraConnectionString = "server=dev-temporary.cgedh3fdlw42.eu-west-1.rds.amazonaws.com; port=3306;uid=hackinguser;pwd='WillDelete3101';database=hackingdb1; Allow User Variables=True"
            };

            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(config), _logger);

            var results = await repo.DataRequestsForRuleRun("2");

            Assert.IsNotNull(results);
        }

        [Test]
        [Explicit]
        public async Task GetDataRequests_UpdatesAsExpected()
        {
            var config = new DataLayerConfiguration
            {
                AuroraConnectionString = "server=dev-temporary.cgedh3fdlw42.eu-west-1.rds.amazonaws.com; port=3306;uid=hackinguser;pwd='WillDelete3101';database=hackingdb1; Allow User Variables=True"
            };

            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(config), _logger);

            var marketDataRequest =
                new MarketDataRequest(
                    "1",
                    "XLON",
                    "entsbp",
                    new InstrumentIdentifiers { Id = "1" },
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(1),
                    "2",
                    true);

            await repo.UpdateToComplete(new [] { marketDataRequest});
        }
    }
}
