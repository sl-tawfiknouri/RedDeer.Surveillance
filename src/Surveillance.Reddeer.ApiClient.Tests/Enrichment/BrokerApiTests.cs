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

    /// <summary>
    /// The broker repository tests.
    /// </summary>
    [TestFixture]
    public class BrokerApiTests
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
        private ILogger<BrokerApi> logger;

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
            var repo = new BrokerApi(this.configuration, this.httpClientFactory, this.policyFactory, this.logger);

            var message = new BrokerEnrichmentMessage
                              {
                                  Brokers = new[] { new BrokerEnrichmentDto { Name = "1010data Financial" } }
                              };

            var messageResult = await repo.PostAsync(message);

            Assert.IsTrue(true);
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.httpClientFactory = new HttpClientFactory(NullLogger<HttpClientFactory>.Instance);
            this.configuration = TestHelpers.Config();
            this.policyFactory = A.Fake<IPolicyFactory>();
            this.logger = A.Fake<ILogger<BrokerApi>>();
        }
    }
}