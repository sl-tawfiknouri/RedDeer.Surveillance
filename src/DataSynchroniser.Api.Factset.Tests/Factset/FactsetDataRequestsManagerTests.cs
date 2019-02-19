using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataSynchroniser.Api.Factset.Factset;
using DataSynchroniser.Api.Factset.Factset.Interfaces;
using Domain.Financial;
using Domain.Markets;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

namespace DataSynchroniser.Api.Factset.Tests.Factset
{
    [TestFixture]
    public class FactsetDataRequestsManagerTests
    {
        private IFactsetDataRequestsSenderManager _requestSender;
        private IFactsetDataRequestsStorageManager _responseStorage;
        private ILogger<FactsetDataRequestsManager> _logger;

        [SetUp]
        public void Setup()
        {
            _requestSender = A.Fake<IFactsetDataRequestsSenderManager>();
            _responseStorage = A.Fake<IFactsetDataRequestsStorageManager>();
            _logger = A.Fake<ILogger<FactsetDataRequestsManager>>();
        }

        [Test]
        public void Constructor_RequestSender_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FactsetDataRequestsManager(null, _responseStorage, _logger));
        }

        [Test]
        public void Constructor_ResponseStorage_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FactsetDataRequestsManager(_requestSender, null, _logger));
        }

        [Test]
        public void Constructor_Logger_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FactsetDataRequestsManager(_requestSender, _responseStorage, null));
        }

        [Test]
        public void Submit_NullOrEmptyRequests_Returns()
        {
            var requestsManager = Build();

            Assert.DoesNotThrowAsync(async () => await requestsManager.Submit(null));

            A
                .CallTo(() => _requestSender.Send(A<List<MarketDataRequest>>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Submit_OnlyCompletedRequests_Returns()
        {
            var requestsManager = Build();
            var factsetRequests = new List<MarketDataRequest> {Builder(true)};

            await requestsManager.Submit(factsetRequests);

            A
                .CallTo(() => _requestSender.Send(A<List<MarketDataRequest>>.Ignored))
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Submit_IncompleteRequests_Returns()
        {
            var requestsManager = Build();
            var factsetRequests = new List<MarketDataRequest> { Builder(false) };

            await requestsManager.Submit(factsetRequests);

            A
                .CallTo(() => _requestSender.Send(A<List<MarketDataRequest>>.Ignored))
                .MustHaveHappened();

            A
                .CallTo(() => _responseStorage.Store(A<FactsetSecurityResponseDto>.Ignored))
                .MustHaveHappened();
        }

        private MarketDataRequest Builder(bool completed)
        {
            return new MarketDataRequest("a", "XLON", "e", InstrumentIdentifiers.Null(), null, null, "1", completed);
        }

        private FactsetDataRequestsManager Build()
        {
            return new FactsetDataRequestsManager(_requestSender, _responseStorage, _logger);
        }

    }
}
