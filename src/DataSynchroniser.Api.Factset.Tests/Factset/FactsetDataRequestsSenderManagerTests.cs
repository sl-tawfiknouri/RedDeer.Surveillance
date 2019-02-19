using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataSynchroniser.Api.Factset.Factset;
using Domain.Markets;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using Surveillance.DataLayer.Api.FactsetMarketData.Interfaces;

namespace DataSynchroniser.Api.Factset.Tests.Factset
{
    [TestFixture]
    public class FactsetDataRequestsSenderManagerTests
    {
        private IFactsetDailyBarApiRepository _dailyBarRepository;
        private ILogger<FactsetDataRequestsSenderManager> _logger;

        [SetUp]
        public void Setup()
        {
            _dailyBarRepository = A.Fake<IFactsetDailyBarApiRepository>();
            _logger = new NullLogger<FactsetDataRequestsSenderManager>();
        }

        [Test]
        public void Constructor_DailyBarRepository_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FactsetDataRequestsSenderManager(null, _logger));
        }

        [Test]
        public void Constructor_Logger_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FactsetDataRequestsSenderManager(_dailyBarRepository, null));
        }

        [Test]
        public async Task Send_FactsetRequests_Empty_Returns()
        {
            var senderManager = Build();

            await senderManager.Send(null);

            A
                .CallTo(() => _dailyBarRepository.GetWithTransientFaultHandling(A<FactsetSecurityDailyRequest>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Send_FactsetRequests_PassedToRepository()
        {
            var senderManager = Build();
            var factsetRequests = new List<MarketDataRequest> {MarketDataRequest.Null()};

            await senderManager.Send(factsetRequests);

            A
                .CallTo(() => _dailyBarRepository.GetWithTransientFaultHandling(A<FactsetSecurityDailyRequest>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        private FactsetDataRequestsSenderManager Build()
        {
            return new FactsetDataRequestsSenderManager(_dailyBarRepository, _logger);
        }
    }
}
