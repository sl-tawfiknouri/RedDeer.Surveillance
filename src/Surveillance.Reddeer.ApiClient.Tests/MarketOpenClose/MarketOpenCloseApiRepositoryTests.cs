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

namespace Surveillance.Reddeer.ApiClient.Tests.MarketOpenClose
{
    [TestFixture]
    public class MarketOpenCloseApiRepositoryTests
    {
        private IHttpClientFactory _httpClientFactory;
        private IApiClientConfiguration _configuration;
        private IPolicyFactory _policyFactory;
        private ILogger<MarketOpenCloseApi> _logger;

        [SetUp]
        public void Setup()
        {
            _httpClientFactory = A.Fake<IHttpClientFactory>();
            _configuration = TestHelpers.Config();
            _policyFactory = A.Fake<IPolicyFactory>();
            _logger = A.Fake<ILogger<MarketOpenCloseApi>>();
        }

        [Test]
        public void Constructor_ThrowsFor_NullLogger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarketOpenCloseApi(_configuration, _httpClientFactory, _policyFactory, null));
        }

        [Test]
        public void Constructor_ThrowsFor_NullApiRepository()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new MarketOpenCloseApi(null, _httpClientFactory, _policyFactory, _logger));
        }

        [Test]
        [Explicit("you will need to ensure that the url and token are correct for your local environment. Don't want this running on a build server.")]
        public async Task Get()
        {
            var repository = new MarketOpenCloseApi(_configuration, _httpClientFactory, _policyFactory, _logger);

            var response = await repository.Get();

            Assert.IsNotNull(response);
            Assert.IsNotEmpty(response);
        }
    }
}
