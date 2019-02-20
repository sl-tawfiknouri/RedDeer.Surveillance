using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Api.ExchangeRate;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.Tests.Helpers;

namespace Surveillance.DataLayer.Tests.Api.ExchangeRate
{
    [TestFixture]
    public class ExchangeRateApiRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private ILogger<ExchangeRateApiRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _configuration = TestHelpers.Config();
            _logger = A.Fake<ILogger<ExchangeRateApiRepository>>();
        }

        [Test]
        public void Constructor_NullLogger_ConsideredThrows_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new ExchangeRateApiRepository(_configuration, null));
        }

        [Test]
        [Explicit]
        public async Task Get()
        {
            var repository = new ExchangeRateApiRepository(_configuration, _logger);

            var response = await repository.Get(new DateTime(2017, 09, 25), new DateTime(2017, 09, 29));

            Assert.IsNotNull(response);
            Assert.IsNotEmpty(response);
        }
    }
}
