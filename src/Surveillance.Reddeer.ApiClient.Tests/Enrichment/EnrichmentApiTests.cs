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

    /// <summary>
    /// The enrichment api tests.
    /// </summary>
    [TestFixture]
    public class EnrichmentApiTests
    {
        /// <summary>
        /// The configuration.
        /// </summary>
        private IApiClientConfiguration configuration;

        /// <summary>
        /// The http client factory.
        /// </summary>
        private IHttpClientFactory httpClientFactory;

        /// <summary>
        /// The policy factory.
        /// </summary>
        private IPolicyFactory policyFactory;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<EnrichmentApi> logger;

        /// <summary>
        /// The get.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        [Explicit]
        public async Task Get()
        {
            var repo = new EnrichmentApi(
                this.configuration,
                this.httpClientFactory,
                this.policyFactory,
                this.logger);

            var message = new SecurityEnrichmentMessage
              {
                  Securities = new[] { new SecurityEnrichmentDto { Sedol = "0408284" } }
              };

            await repo.PostAsync(message);

            Assert.IsTrue(true);
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.httpClientFactory = A.Fake<IHttpClientFactory>();
            this.configuration = TestHelpers.Config();
            this.policyFactory = A.Fake<IPolicyFactory>();
            this.logger = A.Fake<ILogger<EnrichmentApi>>();
        }
    }
}