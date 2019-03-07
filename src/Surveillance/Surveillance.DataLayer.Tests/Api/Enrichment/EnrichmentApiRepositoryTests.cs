using System.Threading.Tasks;
using FakeItEasy;
using Infrastructure.Network.HttpClient.Interfaces;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;
using Surveillance.DataLayer.Api.Enrichment;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.Tests.Helpers;

namespace Surveillance.DataLayer.Tests.Api.Enrichment
{
    [TestFixture]
    public class EnrichmentApiRepositoryTests
    {
        private IHttpClientFactory _httpClientFactory;
        private IDataLayerConfiguration _configuration;
        private ILogger<EnrichmentApiRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _httpClientFactory = A.Fake<IHttpClientFactory>();
            _configuration = TestHelpers.Config();
            _logger = A.Fake<ILogger<EnrichmentApiRepository>>();
        }

        [Test]
        [Explicit]
        public async Task Get()
        {
            var repo = new EnrichmentApiRepository(_configuration, _httpClientFactory, _logger);

            var message = new SecurityEnrichmentMessage
            {
                Securities = new[] { new SecurityEnrichmentDto { Sedol = "0408284" } }
            };


            await repo.Get(message);

            Assert.IsTrue(true);
        }
    }
}
