namespace Surveillance.Reddeer.ApiClient.Tests.MarketOpenClose
{
    using System;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Infrastructure.Network.HttpClient.Interfaces;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using PollyFacade.Policies.Interfaces;

    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
    using Surveillance.Reddeer.ApiClient.MarketOpenClose;
    using Surveillance.Reddeer.ApiClient.Tests.Helpers;

    [TestFixture]
    public class MarketOpenCloseApiRepositoryTests
    {
        private IApiClientConfiguration _configuration;

        private IHttpClientFactory _httpClientFactory;

        private ILogger<MarketOpenCloseApi> _logger;

        private IPolicyFactory _policyFactory;

        [Test]
        public void Constructor_ThrowsFor_NullApiRepository()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new MarketOpenCloseApi(null, this._httpClientFactory, this._policyFactory, this._logger));
        }

        [Test]
        public void Constructor_ThrowsFor_NullLogger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new MarketOpenCloseApi(this._configuration, this._httpClientFactory, this._policyFactory, null));
        }

        [Test]
        [Explicit(
            "you will need to ensure that the url and token are correct for your local environment. Don't want this running on a build server.")]
        public async Task Get()
        {
            var repository = new MarketOpenCloseApi(
                this._configuration,
                this._httpClientFactory,
                this._policyFactory,
                this._logger);

            var response = await repository.Get();

            Assert.IsNotNull(response);
            Assert.IsNotEmpty(response);
        }

        [SetUp]
        public void Setup()
        {
            this._httpClientFactory = A.Fake<IHttpClientFactory>();
            this._configuration = TestHelpers.Config();
            this._policyFactory = A.Fake<IPolicyFactory>();
            this._logger = A.Fake<ILogger<MarketOpenCloseApi>>();
        }
    }
}