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

    [TestFixture]
    public class ExchangeRateApiRepositoryTests
    {
        private IApiClientConfiguration _configuration;

        private IHttpClientFactory _httpClientFactory;

        private ILogger<ExchangeRateApi> _logger;

        private IPolicyFactory _policyFactory;

        [Test]
        public void Constructor_NullLogger_ConsideredThrows_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new ExchangeRateApi(this._configuration, this._httpClientFactory, this._policyFactory, null));
        }

        [Test]
        [Explicit]
        public async Task Get()
        {
            var repository = new ExchangeRateApi(
                this._configuration,
                this._httpClientFactory,
                this._policyFactory,
                this._logger);

            var response = await repository.Get(new DateTime(2017, 09, 25), new DateTime(2017, 09, 29));

            Assert.IsNotNull(response);
            Assert.IsNotEmpty(response);
        }

        [SetUp]
        public void Setup()
        {
            this._httpClientFactory = A.Fake<IHttpClientFactory>();
            this._configuration = TestHelpers.Config();
            this._policyFactory = A.Fake<IPolicyFactory>();
            this._logger = A.Fake<ILogger<ExchangeRateApi>>();
        }
    }
}