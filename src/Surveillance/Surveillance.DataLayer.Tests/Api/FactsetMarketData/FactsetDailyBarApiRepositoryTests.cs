using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PollyFacade.Policies.Interfaces;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using Surveillance.DataLayer.Api.FactsetMarketData;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.Tests.Helpers;
using Utilities.HttpClient.Interfaces;

namespace Surveillance.DataLayer.Tests.Api.FactsetMarketData
{
    [TestFixture]
    public class FactsetDailyBarApiRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private IHttpClientFactory _httpClientFactory;
        private IPolicyFactory _policyFactory;
        private ILogger<FactsetDailyBarApiRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _policyFactory = A.Fake<IPolicyFactory>();
            _httpClientFactory = A.Fake<IHttpClientFactory>();
            _configuration = TestHelpers.Config();
            _logger = A.Fake<ILogger<FactsetDailyBarApiRepository>>();
        }

        [Test]
        [Explicit]
        public async Task Get()
        {
            var repo = new FactsetDailyBarApiRepository(_configuration, _httpClientFactory, _policyFactory, _logger);

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
            var repo = new FactsetDailyBarApiRepository(_configuration, _httpClientFactory, _policyFactory, _logger);
            var cts = new CancellationTokenSource();

            await repo.HeartBeating(cts.Token);

            Assert.IsTrue(true);
        }
    }
}
