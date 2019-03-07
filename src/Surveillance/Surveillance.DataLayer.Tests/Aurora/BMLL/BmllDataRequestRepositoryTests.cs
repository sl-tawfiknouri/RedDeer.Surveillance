using System;
using System.Threading.Tasks;
using Domain.Core.Financial;
using Domain.Core.Financial.Assets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using SharedKernel.Contracts.Markets;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.BMLL;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.Tests.Helpers;

namespace Surveillance.DataLayer.Tests.Aurora.BMLL
{
    [TestFixture]
    public class BmllDataRequestRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private ILogger<RuleRunDataRequestRepository> _logger;
        
        [SetUp]
        public void Setup()
        {
            _configuration = TestHelpers.Config();
            _logger = new NullLogger<RuleRunDataRequestRepository>();
        }

        [Test]
        [Explicit]
        public async Task CreateDataRequest_SavesToDb()
        {

            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(_configuration), _logger);

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
            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(_configuration), _logger);

            var results = await repo.DataRequestsForSystemOperation("2");

            Assert.IsNotNull(results);
        }

        [Test]
        [Explicit]
        public async Task GetDataRequests_UpdatesAsExpected()
        {
            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(_configuration), _logger);

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

            await repo.UpdateToCompleteWithDuplicates(new [] { marketDataRequest});
        }
    }
}
