namespace Surveillance.Reddeer.ApiClient.Tests.RuleParameter
{
    using System;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Infrastructure.Network.HttpClient.Interfaces;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using PollyFacade.Policies.Interfaces;

    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
    using Surveillance.Reddeer.ApiClient.RuleParameter;
    using Surveillance.Reddeer.ApiClient.Tests.Helpers;

    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// The rule parameter tests.
    /// </summary>
    [TestFixture]
    public class RuleParameterApiTests
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
        private ILogger<RuleParameterApi> logger;

        /// <summary>
        /// The constructor throws for null repository.
        /// </summary>
        [Test]
        public void ConstructorThrowsForNullApiRepository()
        {
            Assert.Throws<ArgumentNullException>(
                () => new RuleParameterApi(null, this.httpClientFactory, this.policyFactory, this.logger));
        }

        /// <summary>
        /// The constructor throws for null logger.
        /// </summary>
        [Test]
        public void ConstructorThrowsForNullLogger()
        {
            Assert.Throws<ArgumentNullException>(
                () => new RuleParameterApi(this.configuration, this.httpClientFactory, this.policyFactory, null));
        }

        /// <summary>
        /// The get.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Test]
        [Explicit(
            "you will need to ensure that the url and token are correct for your local environment. Don't want this running on a build server.")]
        public async Task Get()
        {
            var repository = new RuleParameterApi(
                this.configuration,
                this.httpClientFactory,
                this.policyFactory,
                this.logger);

            var response = await repository.GetAsync();

            Assert.IsNotNull(response);
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
            this.logger = A.Fake<ILogger<RuleParameterApi>>();
        }
    }
}