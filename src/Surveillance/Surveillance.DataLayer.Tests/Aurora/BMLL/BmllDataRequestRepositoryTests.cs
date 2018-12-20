using System;
using System.Threading.Tasks;
using DomainV2.Financial;
using DomainV2.Markets;
using FakeItEasy;
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
        private ILogger<BmllDataRequestRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new NullLogger<BmllDataRequestRepository>();
        }

        [Test]
        public async Task CreateDataRequest_SavesToDb()
        {
            var config = new DataLayerConfiguration
            {
                AuroraConnectionString = "server=127.0.0.1; port=3306;uid=root;pwd='drunkrabbit101';database=dev_surveillance; Allow User Variables=True"
            };

            var repo = new BmllDataRequestRepository(new ConnectionStringFactory(config), _logger);

            var marketDataRequest =
                new MarketDataRequest(
                    "XLON",
                    new InstrumentIdentifiers { Id = "1" },
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(1),
                    "1");

            await repo.CreateDataRequest(marketDataRequest);
        }
    }
}
