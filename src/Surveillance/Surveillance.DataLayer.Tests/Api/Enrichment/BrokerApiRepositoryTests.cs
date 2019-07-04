using System.Threading.Tasks;
using FakeItEasy;
using Infrastructure.Network.HttpClient;
using Infrastructure.Network.HttpClient.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.BrokerEnrichment;
using Surveillance.DataLayer.Api.Enrichment;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.Tests.Helpers;

namespace Surveillance.DataLayer.Tests.Api.Enrichment
{
    [TestFixture]
    public class BrokerApiRepositoryTests
    {
        private IHttpClientFactory _httpClientFactory;
        private IDataLayerConfiguration _configuration;
        private ILogger<BrokerApi> _logger;

        [SetUp]
        public void Setup()
        {
            _httpClientFactory = new HttpClientFactory(NullLogger<HttpClientFactory>.Instance);
            _configuration = TestHelpers.Config();
            _logger = A.Fake<ILogger<BrokerApi>>();
        }

        [Test]
        [Explicit]
        public async Task Get()
        {
            var repo = new BrokerApi(_configuration, _httpClientFactory, _logger);

            var message = new BrokerEnrichmentMessage()
            {
                Brokers = new[] {new BrokerEnrichmentDto {Name = "1010data Financial" } }
            };
        
            var messageResult = await repo.Post(message);

            Assert.IsTrue(true);
        }
    }
}
