namespace DataSynchroniser.Api.Factset.Tests.Factset
{
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

    [TestFixture]
    public class FactsetDataRequestsManagerTests
    {
        private ILogger<FactsetDataRequestsManager> _logger;

        private IFactsetDataRequestsApiManager _requestApi;

        private IReddeerMarketDailySummaryRepository _responseStorage;

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new FactsetDataRequestsManager(this._requestApi, this._responseStorage, null));
        }

        [Test]
        public void Constructor_RequestSender_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new FactsetDataRequestsManager(null, this._responseStorage, this._logger));
        }

        [Test]
        public void Constructor_ResponseStorage_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new FactsetDataRequestsManager(this._requestApi, null, this._logger));
        }

        [SetUp]
        public void Setup()
        {
            this._requestApi = A.Fake<IFactsetDataRequestsApiManager>();
            this._responseStorage = A.Fake<IReddeerMarketDailySummaryRepository>();
            this._logger = A.Fake<ILogger<FactsetDataRequestsManager>>();
        }

        [Test]
        public async Task Submit_IncompleteRequests_Returns()
        {
            var requestsManager = this.BuildDataRequestsManager();
            var factsetRequests = new List<MarketDataRequest> { this.BuildMarketDataRequests(false) };

            await requestsManager.Submit(factsetRequests, "systemProcessOperationId");

            A.CallTo(() => this._requestApi.Send(A<List<MarketDataRequest>>.Ignored)).MustHaveHappened();

            A.CallTo(() => this._responseStorage.Save(A<IReadOnlyCollection<FactsetSecurityDailyResponseItem>>.Ignored))
                .MustHaveHappened();
        }

        [Test]
        public void Submit_NullOrEmptyRequests_Returns()
        {
            var requestsManager = this.BuildDataRequestsManager();

            Assert.DoesNotThrowAsync(async () => await requestsManager.Submit(null, "systemProcessOperationId"));

            A.CallTo(() => this._requestApi.Send(A<List<MarketDataRequest>>.Ignored)).MustNotHaveHappened();
        }

        [Test]
        public async Task Submit_OnlyCompletedRequests_Returns()
        {
            var requestsManager = this.BuildDataRequestsManager();
            var factsetRequests = new List<MarketDataRequest> { this.BuildMarketDataRequests(true) };

            await requestsManager.Submit(factsetRequests, "systemProcessOperationId");

            A.CallTo(() => this._requestApi.Send(A<List<MarketDataRequest>>.Ignored)).MustNotHaveHappened();
        }

        private FactsetDataRequestsManager BuildDataRequestsManager()
        {
            return new FactsetDataRequestsManager(this._requestApi, this._responseStorage, this._logger);
        }

        private MarketDataRequest BuildMarketDataRequests(bool completed)
        {
            return new MarketDataRequest(
                "a",
                "XLON",
                "e",
                InstrumentIdentifiers.Null(),
                null,
                null,
                "1",
                completed,
                DataSource.Factset);
        }
    }
}