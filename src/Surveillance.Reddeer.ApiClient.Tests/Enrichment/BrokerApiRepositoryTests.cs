using System.Threading.Tasks;
using FakeItEasy;
using Infrastructure.Network.HttpClient;
using Infrastructure.Network.HttpClient.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using PollyFacade.Policies.Interfaces;
using RedDeer.Contracts.SurveillanceService.Api.BrokerEnrichment;
using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
using Surveillance.Reddeer.ApiClient.Enrichment;
using Surveillance.Reddeer.ApiClient.Tests.Helpers;

namespace Surveillance.Reddeer.ApiClient.Tests.Enrichment
{
    [TestFixture]
    public class BrokerApiRepositoryTests
    {
        private IHttpClientFactory _httpClientFactory;
        private IApiClientConfiguration _configuration;
        private IPolicyFactory _policyFactory;
        private ILogger<BrokerApi> _logger;

        [SetUp]
        public void Setup()
        {
            _httpClientFactory = new HttpClientFactory(NullLogger<HttpClientFactory>.Instance);
            _configuration = TestHelpers.Config();
            _policyFactory = A.Fake<IPolicyFactory>();
            _logger = A.Fake<ILogger<BrokerApi>>();
        }

        [Test]
        [Explicit]
        public async Task Get()
        {
            var repo = new BrokerApi(_configuration, _httpClientFactory, _policyFactory, _logger);

            var message = new BrokerEnrichmentMessage()
            {
                Brokers = new[] {new BrokerEnrichmentDto {Name = "1010data Financial" } }
            };
        
            var messageResult = await repo.Post(message);

            Assert.IsTrue(true);
        }
    }
}
