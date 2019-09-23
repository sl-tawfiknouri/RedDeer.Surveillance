namespace Surveillance.Reddeer.ApiClient.Tests.Enrichment
{
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

    [TestFixture]
    public class BrokerApiRepositoryTests
    {
        private IApiClientConfiguration _configuration;

        private IHttpClientFactory _httpClientFactory;

        private ILogger<BrokerApi> _logger;

        private IPolicyFactory _policyFactory;

        [Test]
        [Explicit]
        public async Task Get()
        {
            var repo = new BrokerApi(this._configuration, this._httpClientFactory, this._policyFactory, this._logger);

            var message = new BrokerEnrichmentMessage
                              {
                                  Brokers = new[] { new BrokerEnrichmentDto { Name = "1010data Financial" } }
                              };

            var messageResult = await repo.Post(message);

            Assert.IsTrue(true);
        }

        [SetUp]
        public void Setup()
        {
            this._httpClientFactory = new HttpClientFactory(NullLogger<HttpClientFactory>.Instance);
            this._configuration = TestHelpers.Config();
            this._policyFactory = A.Fake<IPolicyFactory>();
            this._logger = A.Fake<ILogger<BrokerApi>>();
        }
    }
}