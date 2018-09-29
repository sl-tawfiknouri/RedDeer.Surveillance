using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Api.RuleParameter;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Tests.Api.RuleParameter
{
    [TestFixture]
    public class RuleParameterApiRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private ILogger<RuleParameterApiRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _configuration = A.Fake<IDataLayerConfiguration>();
            _configuration.ClientServiceUrl = "http://localhost:8080";
            _configuration.SurveillanceUserApiAccessToken = "uwat";
            _logger = A.Fake<ILogger<RuleParameterApiRepository>>();
        }

        [Test]
        public void Constructor_ThrowsFor_NullLogger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleParameterApiRepository(_configuration, null));
        }

        [Test]
        public void Constructor_ThrowsFor_NullApiRepository()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleParameterApiRepository(null, _logger));
        }

        [Test]
        [Explicit("you will need to ensure that the url and token are correct for your local environment. Don't want this running on a build server.")]
        public async Task Get()
        {
            var repository = new RuleParameterApiRepository(_configuration, _logger);

            var response = await repository.Get();

            Assert.IsNotNull(response);
        }
    }
}
