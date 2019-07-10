using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataSynchroniser.Api.Factset.Factset;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using SharedKernel.Contracts.Markets;
using Surveillance.Reddeer.ApiClient.FactsetMarketData.Interfaces;

namespace DataSynchroniser.Api.Factset.Tests.Factset
{
    [TestFixture]
    public class FactsetDataRequestsSenderManagerTests
    {
        private IFactsetDailyBarApi _dailyBarRepository;
        private ILogger<FactsetDataRequestsApiManager> _logger;

        [SetUp]
        public void Setup()
        {
            _dailyBarRepository = A.Fake<IFactsetDailyBarApi>();
            _logger = new NullLogger<FactsetDataRequestsApiManager>();
        }

        [Test]
        public void Constructor_DailyBarRepository_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FactsetDataRequestsApiManager(null, _logger));
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FactsetDataRequestsApiManager(_dailyBarRepository, null));
        }

        [Test]
        public async Task Send_FactsetRequests_Empty_Returns()
        {
            var senderManager = BuildRequestsSenderManager();

            await senderManager.Send(null);

            A
                .CallTo(() => _dailyBarRepository.GetWithTransientFaultHandling(A<FactsetSecurityDailyRequest>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Send_FactsetRequests_PassedToRepository()
        {
            var senderManager = BuildRequestsSenderManager();
            var factsetRequests = new List<MarketDataRequest> {MarketDataRequest.Null()};

            await senderManager.Send(factsetRequests);

            A
                .CallTo(() => _dailyBarRepository.GetWithTransientFaultHandling(A<FactsetSecurityDailyRequest>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        private FactsetDataRequestsApiManager BuildRequestsSenderManager()
        {
            return new FactsetDataRequestsApiManager(_dailyBarRepository, _logger);
        }
    }
}
