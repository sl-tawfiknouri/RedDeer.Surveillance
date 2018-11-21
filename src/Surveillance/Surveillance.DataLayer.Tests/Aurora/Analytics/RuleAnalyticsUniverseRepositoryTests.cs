using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Analytics;
using Surveillance.DataLayer.Configuration;

namespace Surveillance.DataLayer.Tests.Aurora.Analytics
{
    [TestFixture]
    public class RuleAnalyticsUniverseRepositoryTests
    {
        private ILogger<RuleAnalyticsUniverseRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<RuleAnalyticsUniverseRepository>>();
        }

        [Test]
        [Explicit]
        public async Task Does_Save()
        {
            var universeAnalytics = new UniverseAnalytics
            {
                UnknownEventCount = 1,
                GenesisEventCount = 1,
                EschatonEventCount = 1,
                TradeReddeerCount = 100,
                TradeReddeerSubmittedCount = 50,
                StockTickReddeerCount = 150,
                StockMarketOpenCount = 400,
                StockMarketCloseCount = 400,
                UniqueTradersCount = 30,
                UniqueSecuritiesCount = 100,
                UniqueMarketsTradedOnCount = 20,
                SystemProcessOperationId = 1
            };

            var config = new DataLayerConfiguration
            {
                AuroraConnectionString = "server=127.0.0.1; port=3306;uid=root;pwd='drunkrabbit101';database=dev_surveillance; Allow User Variables=True"
            };

            var factory = new ConnectionStringFactory(config);
            var repo = new RuleAnalyticsUniverseRepository(factory, _logger);

            await repo.Create(universeAnalytics);
        }
    }
}
