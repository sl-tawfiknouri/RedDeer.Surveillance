using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using Surveillance.DataLayer.Api.FactsetMarketData;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Tests.Api.FactsetMarketData
{
    [TestFixture]
    public class FactsetDailyBarApiRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private ILogger<FactsetDailyBarApiRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _configuration = A.Fake<IDataLayerConfiguration>();
            _configuration.ClientServiceUrl = "http://localhost:8080";
            _configuration.SurveillanceUserApiAccessToken = "uwat";
            _logger = A.Fake<ILogger<FactsetDailyBarApiRepository>>();
        }

        [Test]
        [Explicit]
        public async Task Get()
        {
            var repo = new FactsetDailyBarApiRepository(_configuration, _logger);

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

            await repo.Get(message);

            Assert.IsTrue(true);
        }

        [Test]
        [Explicit]
        public async Task Heartbeating()
        {
            var repo = new FactsetDailyBarApiRepository(_configuration, _logger);
            var cts = new CancellationTokenSource();

            await repo.HeartBeating(cts.Token);

            Assert.IsTrue(true);
        }
    }
}
