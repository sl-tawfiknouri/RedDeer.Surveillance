namespace Surveillance.Reddeer.ApiClient.Tests.Enrichment
{
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

    [TestFixture]
    public class EnrichmentApiRepositoryTests
    {
        private IApiClientConfiguration _configuration;

        private IHttpClientFactory _httpClientFactory;

        private ILogger<EnrichmentApi> _logger;

        private IPolicyFactory _policyFactory;

        [Test]
        [Explicit]
        public async Task Get()
        {
            var repo = new EnrichmentApi(
                this._configuration,
                this._httpClientFactory,
                this._policyFactory,
                this._logger);

            var message = new SecurityEnrichmentMessage
                              {
                                  Securities = new[] { new SecurityEnrichmentDto { Sedol = "0408284" } }
                              };

            await repo.Post(message);

            Assert.IsTrue(true);
        }

        [SetUp]
        public void Setup()
        {
            this._httpClientFactory = A.Fake<IHttpClientFactory>();
            this._configuration = TestHelpers.Config();
            this._policyFactory = A.Fake<IPolicyFactory>();
            this._logger = A.Fake<ILogger<EnrichmentApi>>();
        }
    }
}