using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataSynchroniser.Api.Factset.Factset.Interfaces;
using DataSynchroniser.Api.Factset.Filters.Interfaces;
using Domain.Core.Financial;
using Domain.Markets;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;

namespace DataSynchroniser.Api.Factset.Tests
{
    [TestFixture]
    public class FactsetDataSynchroniserTests
    {
        private IFactsetDataRequestsManager _dataRequestsManager;
        private IFactsetDataRequestFilter _requestFilter;
        private ISystemProcessOperationThirdPartyDataRequestContext _requestContext;
        private ILogger<FactsetDataSynchroniser> _logger;

        [SetUp]
        public void Setup()
        {
            _dataRequestsManager = A.Fake<IFactsetDataRequestsManager>();

            _requestFilter = A.Fake<IFactsetDataRequestFilter>();
            A.CallTo(() => _requestFilter.ValidAssetType(A<MarketDataRequest>.Ignored)).Returns(true);

            _requestContext = A.Fake<ISystemProcessOperationThirdPartyDataRequestContext>();
            _logger = A.Fake<ILogger<FactsetDataSynchroniser>>();
        }

        [Test]
        public void Constructor_NullLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FactsetDataSynchroniser(_dataRequestsManager, _requestFilter, null));
        }

        [Test]
        public void Handle_Null_SystemProcessOperationId_DoesNotThrow()
        {
            var synchroniser = BuildDataSynchroniser();

            Assert.DoesNotThrowAsync(async () => await synchroniser.Handle(null, _requestContext, new MarketDataRequest[0]));
        }

        [Test]
        public void Handle_Null_DataRequestContext_DoesNotThrow()
        {
            var synchroniser = BuildDataSynchroniser();

            Assert.DoesNotThrowAsync(async () => await synchroniser.Handle("id", null, new MarketDataRequest[0]));
        }

        [Test]
        public void Handle_Null_MarketDataRequests_DoesNotThrow()
        {
            var synchroniser = BuildDataSynchroniser();

            Assert.DoesNotThrowAsync(async () => await synchroniser.Handle("id", _requestContext, null));
        }

        [Test]
        public async Task Handle_NonRelevantRequests_DoesNotThrow()
        {
            var synchroniser = BuildDataSynchroniser();
            A.CallTo(() => _requestFilter.ValidAssetType(A<MarketDataRequest>.Ignored)).Returns(false);

            var request = new List<MarketDataRequest>
            {
                BuildMarketDataRequest("d")
            };

           Assert.DoesNotThrowAsync(async () => await synchroniser.Handle("a", _requestContext, request));
        }

        private FactsetDataSynchroniser BuildDataSynchroniser()
        {
            return new FactsetDataSynchroniser(_dataRequestsManager, _requestFilter, _logger);
        }

        private MarketDataRequest BuildMarketDataRequest(string cfi)
        {
            return new MarketDataRequest("a", "XLON", cfi, InstrumentIdentifiers.Null(), null, null, "1", true);
        }
    }
}
