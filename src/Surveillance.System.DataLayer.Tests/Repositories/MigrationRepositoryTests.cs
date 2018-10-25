using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.System.DataLayer.Repositories;
using Surveillance.System.DataLayer.Repositories.Interfaces;

namespace Surveillance.System.DataLayer.Tests.Repositories
{
    [TestFixture]
    public class MigrationRepositoryTests
    {
        private ILogger<ISystemProcessRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<ISystemProcessRepository>>();
        }

        [Test]
        [Explicit]
        public async Task Get_MigrationValue()
        {
            var config = new SystemDataLayerConfig
            {
                SurveillanceAuroraConnectionString = "server=127.0.0.1; port=3306;uid=root;pwd='drunkrabbit101';database=dev_surveillance; Allow User Variables=True"
            };
            var repository = new MigrationRepository(new ConnectionStringFactory(config), _logger);

            var version = await repository.LatestMigrationVersion();

            Assert.AreEqual(version, 0);
        }

        [Test]
        [Explicit]
        public async Task Get_LatestAvailable_MigrationValue()
        {
            var config = new SystemDataLayerConfig
            {
                SurveillanceAuroraConnectionString = "server=127.0.0.1; port=3306;uid=root;pwd='drunkrabbit101';database=dev_surveillance; Allow User Variables=True"
            };
            var repository = new MigrationRepository(new ConnectionStringFactory(config), _logger);

            var version = repository.LatestMigrationAvailable();

            Assert.AreEqual(version, 3);
        }

        [Test]
        [Explicit]
        public async Task UpdateMigrations()
        {
            var config = new SystemDataLayerConfig
            {
                SurveillanceAuroraConnectionString = "server=127.0.0.1; port=3306;uid=root;pwd='drunkrabbit101';database=dev_surveillance; Allow User Variables=True"
            };
            var repository = new MigrationRepository(new ConnectionStringFactory(config), _logger);

            await repository.UpdateMigrations();

            var executed = await repository.LatestMigrationVersion();
            var available = repository.LatestMigrationAvailable();

            Assert.AreEqual(executed, available);
        }
    }
}
