using System.Threading.Tasks;
using FakeItEasy;
using Infrastructure.Network.HttpClient.Interfaces;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PollyFacade.Policies.Interfaces;
using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;
using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
using Surveillance.Reddeer.ApiClient.Enrichment;
using Surveillance.Reddeer.ApiClient.Tests.Helpers;

namespace Surveillance.Reddeer.ApiClient.Tests.Enrichment
{
    [TestFixture]
    public class EnrichmentApiRepositoryTests
    {
        private IHttpClientFactory _httpClientFactory;
        private IApiClientConfiguration _configuration;
        private IPolicyFactory _policyFactory;
        private ILogger<EnrichmentApi> _logger;

        [SetUp]
        public void Setup()
        {
            _httpClientFactory = A.Fake<IHttpClientFactory>();
            _configuration = TestHelpers.Config();
            _policyFactory = A.Fake<IPolicyFactory>();
            _logger = A.Fake<ILogger<EnrichmentApi>>();
        }

        [Test]
        [Explicit]
        public async Task Get()
        {
            var repo = new EnrichmentApi(_configuration, _httpClientFactory, _policyFactory, _logger);

            var message = new SecurityEnrichmentMessage
            {
                Securities = new[] { new SecurityEnrichmentDto { Sedol = "0408284" } }
            };


            await repo.Post(message);

            Assert.IsTrue(true);
        }
    }
}
