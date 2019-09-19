namespace DataSynchroniser.Api.Factset.Tests.Factset
{
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

    [TestFixture]
    public class FactsetDataRequestsSenderManagerTests
    {
        private IFactsetDailyBarApi _dailyBarRepository;

        private ILogger<FactsetDataRequestsApiManager> _logger;

        [Test]
        public void Constructor_DailyBarRepository_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FactsetDataRequestsApiManager(null, this._logger));
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new FactsetDataRequestsApiManager(this._dailyBarRepository, null));
        }

        [Test]
        public async Task Send_FactsetRequests_Empty_Returns()
        {
            var senderManager = this.BuildRequestsSenderManager();

            await senderManager.Send(null);

            A.CallTo(
                    () => this._dailyBarRepository.GetWithTransientFaultHandlingAsync(
                        A<FactsetSecurityDailyRequest>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Send_FactsetRequests_PassedToRepository()
        {
            var senderManager = this.BuildRequestsSenderManager();
            var factsetRequests = new List<MarketDataRequest> { MarketDataRequest.Null() };

            await senderManager.Send(factsetRequests);

            A.CallTo(
                    () => this._dailyBarRepository.GetWithTransientFaultHandlingAsync(
                        A<FactsetSecurityDailyRequest>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._dailyBarRepository = A.Fake<IFactsetDailyBarApi>();
            this._logger = new NullLogger<FactsetDataRequestsApiManager>();
        }

        private FactsetDataRequestsApiManager BuildRequestsSenderManager()
        {
            return new FactsetDataRequestsApiManager(this._dailyBarRepository, this._logger);
        }
    }
}