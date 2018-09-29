using System;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Api;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Tests.Api
{
    [TestFixture]
    public class MarketOpenCloseApiRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private ILogger<MarketOpenCloseApiRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _configuration = A.Fake<IDataLayerConfiguration>();
            _configuration.ClientServiceUrl = "http://localhost:8080";
            _configuration.SurveillanceUserApiAccessToken = "uwat";
            _logger = A.Fake<ILogger<MarketOpenCloseApiRepository>>();
        }

        [Test]
        public void Constructor_ThrowsFor_NullLogger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarketOpenCloseApiRepository(_configuration, null));
        }

        [Test]
        public void Constructor_ThrowsFor_NullApiRepository()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarketOpenCloseApiRepository(null, _logger));
        }

        [Test]
        [Explicit("you will need to ensure that the url and token are correct for your local environment. Don't want this running on a build server.")]
        public async Task Get()
        {
            var repository = new MarketOpenCloseApiRepository(_configuration, _logger);

            var response = await repository.Get();

            Assert.IsNotNull(response);
            Assert.IsNotEmpty(response);
        }
    }
}
