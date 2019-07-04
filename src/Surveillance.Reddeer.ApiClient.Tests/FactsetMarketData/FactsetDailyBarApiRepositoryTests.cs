using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Infrastructure.Network.HttpClient.Interfaces;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PollyFacade.Policies.Interfaces;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
using Surveillance.Reddeer.ApiClient.FactsetMarketData;
using Surveillance.Reddeer.ApiClient.Tests.Helpers;

namespace Surveillance.Reddeer.ApiClient.Tests.FactsetMarketData
{
    [TestFixture]
    public class FactsetDailyBarApiRepositoryTests
    {
        private IApiClientConfiguration _configuration;
        private IHttpClientFactory _httpClientFactory;
        private IPolicyFactory _policyFactory;
        private ILogger<FactsetDailyBarApi> _logger;

        [SetUp]
        public void Setup()
        {
            _policyFactory = A.Fake<IPolicyFactory>();
            _httpClientFactory = A.Fake<IHttpClientFactory>();
            _configuration = TestHelpers.Config();
            _logger = A.Fake<ILogger<FactsetDailyBarApi>>();
        }

        [Test]
        [Explicit]
        public async Task Get()
        {
            var repo = new FactsetDailyBarApi(_configuration, _httpClientFactory, _policyFactory, _logger);

            var message = new FactsetSecurityDailyRequest
            {
                Requests = new List<FactsetSecurityRequestItem>
                {
                    new FactsetSecurityRequestItem
                    {
                        Figi = "BBG000C6K6G9",
                        From = new DateTime(2018, 01, 01),
                        To = new DateTime(2018, 01, 05)
                    }
                }
            };

            await repo.GetWithTransientFaultHandling(message);

            Assert.IsTrue(true);
        }

        [Test]
        [Explicit]
        public async Task Heartbeating()
        {
            var repo = new FactsetDailyBarApi(_configuration, _httpClientFactory, _policyFactory, _logger);
            var cts = new CancellationTokenSource();

            await repo.HeartBeating(cts.Token);

            Assert.IsTrue(true);
        }
    }
}
