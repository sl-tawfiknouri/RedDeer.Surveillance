using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataSynchroniser.Api.Factset.Factset;
using DataSynchroniser.Api.Factset.Factset.Interfaces;
using Domain.Core.Financial.Assets;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using SharedKernel.Contracts.Markets;
using Surveillance.DataLayer.Aurora.Market.Interfaces;

namespace DataSynchroniser.Api.Factset.Tests.Factset
{
    [TestFixture]
    public class FactsetDataRequestsManagerTests
    {
        private IFactsetDataRequestsApiManager _requestApi;
        private IReddeerMarketDailySummaryRepository _responseStorage;
        private ILogger<FactsetDataRequestsManager> _logger;

        [SetUp]
        public void Setup()
        {
            _requestApi = A.Fake<IFactsetDataRequestsApiManager>();
            _responseStorage = A.Fake<IReddeerMarketDailySummaryRepository>();
            _logger = A.Fake<ILogger<FactsetDataRequestsManager>>();
        }

        [Test]
        public void Constructor_RequestSender_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FactsetDataRequestsManager(null, _responseStorage, _logger));
        }

        [Test]
        public void Constructor_ResponseStorage_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FactsetDataRequestsManager(_requestApi, null, _logger));
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FactsetDataRequestsManager(_requestApi, _responseStorage, null));
        }

        [Test]
        public void Submit_NullOrEmptyRequests_Returns()
        {
            var requestsManager = BuildDataRequestsManager();

            Assert.DoesNotThrowAsync(async () => await requestsManager.Submit(null, "systemProcessOperationId"));

            A
                .CallTo(() => _requestApi.Send(A<List<MarketDataRequest>>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Submit_OnlyCompletedRequests_Returns()
        {
            var requestsManager = BuildDataRequestsManager();
            var factsetRequests = new List<MarketDataRequest> {BuildMarketDataRequests(true)};

            await requestsManager.Submit(factsetRequests, "systemProcessOperationId");

            A
                .CallTo(() => _requestApi.Send(A<List<MarketDataRequest>>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Submit_IncompleteRequests_Returns()
        {
            var requestsManager = BuildDataRequestsManager();
            var factsetRequests = new List<MarketDataRequest> { BuildMarketDataRequests(false) };

            await requestsManager.Submit(factsetRequests, "systemProcessOperationId");

            A
                .CallTo(() => _requestApi.Send(A<List<MarketDataRequest>>.Ignored))
                .MustHaveHappened();

            A
                .CallTo(() => _responseStorage.Save(A<IReadOnlyCollection<FactsetSecurityDailyResponseItem>>.Ignored))
                .MustHaveHappened();
        }

        private MarketDataRequest BuildMarketDataRequests(bool completed)
        {
            return new MarketDataRequest("a", "XLON", "e", InstrumentIdentifiers.Null(), null, null, "1", completed, DataSource.Factset);
        }

        private FactsetDataRequestsManager BuildDataRequestsManager()
        {
            return new FactsetDataRequestsManager(_requestApi, _responseStorage, _logger);
        }

    }
}
