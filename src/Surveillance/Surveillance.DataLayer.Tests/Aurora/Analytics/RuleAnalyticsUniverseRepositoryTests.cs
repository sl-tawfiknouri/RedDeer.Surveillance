using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Analytics;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.Tests.Helpers;

namespace Surveillance.DataLayer.Tests.Aurora.Analytics
{
    [TestFixture]
    public class RuleAnalyticsUniverseRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private IConnectionStringFactory _connectionStringFactory;
        private ILogger<RuleAnalyticsUniverseRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _configuration = TestHelpers.Config();
            _connectionStringFactory = A.Fake<IConnectionStringFactory>();
            _logger = A.Fake<ILogger<RuleAnalyticsUniverseRepository>>();
        }

        [Test]
        public void Ctor_NullDbconnectionFactory_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleAnalyticsUniverseRepository(null, _logger));
        }

        [Test]
        public void Ctor_NullLogger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleAnalyticsUniverseRepository(_connectionStringFactory, null));
        }

        [Test]
        public void Create_NullAnalytics_DoesNotThrow()
        {
            var repo = new RuleAnalyticsUniverseRepository(_connectionStringFactory, _logger);

            Assert.DoesNotThrowAsync(() => repo.Create(null));
        }

        [Test]
        [Explicit]
        public async Task Does_Save()
        {
            var universeAnalytics = new UniverseAnalytics
            {
                SystemProcessOperationId = 12,
                GenesisEventCount = 2,
                EschatonEventCount = 3,
                TradeReddeerCount = 4,
                TradeReddeerSubmittedCount = 5,
                StockTickReddeerCount = 6,
                StockMarketOpenCount = 7,
                StockMarketCloseCount = 8,
                UniqueTradersCount = 9,
                UniqueSecuritiesCount = 10,
                UniqueMarketsTradedOnCount = 11,
            };

            var factory = new ConnectionStringFactory(_configuration);
            var repo = new RuleAnalyticsUniverseRepository(factory, _logger);

            await repo.Create(universeAnalytics);
        }
    }
}
