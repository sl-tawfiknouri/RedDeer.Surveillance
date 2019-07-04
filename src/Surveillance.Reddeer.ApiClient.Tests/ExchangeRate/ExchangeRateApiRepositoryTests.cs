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

namespace Surveillance.Reddeer.ApiClient.Tests.ExchangeRate
{
    [TestFixture]
    public class ExchangeRateApiRepositoryTests
    {
        private IHttpClientFactory _httpClientFactory;
        private IApiClientConfiguration _configuration;
        private IPolicyFactory _policyFactory;
        private ILogger<ExchangeRateApi> _logger;

        [SetUp]
        public void Setup()
        {
            _httpClientFactory = A.Fake<IHttpClientFactory>();
            _configuration = TestHelpers.Config();
            _policyFactory = A.Fake<IPolicyFactory>();
            _logger = A.Fake<ILogger<ExchangeRateApi>>();
        }

        [Test]
        public void Constructor_NullLogger_ConsideredThrows_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new ExchangeRateApi(_configuration, _httpClientFactory, _policyFactory, null));
        }

        [Test]
        [Explicit]
        public async Task Get()
        {
            var repository = new ExchangeRateApi(_configuration, _httpClientFactory, _policyFactory, _logger);

            var response = await repository.Get(new DateTime(2017, 09, 25), new DateTime(2017, 09, 29));

            Assert.IsNotNull(response);
            Assert.IsNotEmpty(response);
        }
    }
}
