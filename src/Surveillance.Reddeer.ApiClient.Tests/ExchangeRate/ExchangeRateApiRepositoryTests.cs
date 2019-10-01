namespace Surveillance.Reddeer.ApiClient.Tests.ExchangeRate
{
    using System;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Infrastructure.Network.HttpClient.Interfaces;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using PollyFacade.Policies.Interfaces;

    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
    using Surveillance.Reddeer.ApiClient.ExchangeRate;
    using Surveillance.Reddeer.ApiClient.Tests.Helpers;

    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// The exchange rate api repository tests.
    /// </summary>
    [TestFixture]
    public class ExchangeRateApiRepositoryTests
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
        private ILogger<ExchangeRateApi> logger;

        /// <summary>
        /// The constructor null logger considered throws exception.
        /// </summary>
        [Test]
        public void ConstructorNullLoggerConsideredThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ExchangeRateApi(
                    this.configuration, 
                    this.httpClientFactory,
                    this.policyFactory, 
                    null));
        }

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
            var repository = new ExchangeRateApi(
                this.configuration,
                this.httpClientFactory,
                this.policyFactory,
                this.logger);

            var response =
                await repository
                    .GetAsync(
                        new DateTime(2017, 09, 25),
                        new DateTime(2017, 09, 29));

            Assert.IsNotNull(response);
            Assert.IsNotEmpty(response);
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
            this.logger = A.Fake<ILogger<ExchangeRateApi>>();
        }
    }
}