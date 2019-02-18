using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataSynchroniser.Api.Factset.Factset.Interfaces;
using DataSynchroniser.Api.Factset.Filters.Interfaces;
using Domain.Financial;
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
        private IMarketDataRequestFilter _requestFilter;
        private ISystemProcessOperationThirdPartyDataRequestContext _requestContext;
        private ILogger<FactsetDataSynchroniser> _logger;

        [SetUp]
        public void Setup()
        {
            _dataRequestsManager = A.Fake<IFactsetDataRequestsManager>();

            _requestFilter = A.Fake<IMarketDataRequestFilter>();
            A.CallTo(() => _requestFilter.Filter(A<MarketDataRequest>.Ignored)).Returns(true);

            _requestContext = A.Fake<ISystemProcessOperationThirdPartyDataRequestContext>();
            _logger = A.Fake<ILogger<FactsetDataSynchroniser>>();
        }

        [Test]
        public void Constructor_NullLogger_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new FactsetDataSynchroniser(_dataRequestsManager, _requestFilter, null));
        }

        [Test]
        public void Handle_Null_SystemProcessOperationId_DoesNotThrow()
        {
            var synchroniser = Build();

            Assert.DoesNotThrowAsync(async () => await synchroniser.Handle(null, _requestContext, new MarketDataRequest[0]));
        }

        [Test]
        public void Handle_Null_DataRequestContext_DoesNotThrow()
        {
            var synchroniser = Build();

            Assert.DoesNotThrowAsync(async () => await synchroniser.Handle("id", null, new MarketDataRequest[0]));
        }

        [Test]
        public void Handle_Null_MarketDataRequests_DoesNotThrow()
        {
            var synchroniser = Build();

            Assert.DoesNotThrowAsync(async () => await synchroniser.Handle("id", _requestContext, null));
        }

        [Test]
        public async Task Handle_NonRelevantRequests_DoesNotThrow()
        {
            var synchroniser = Build();
            A.CallTo(() => _requestFilter.Filter(A<MarketDataRequest>.Ignored)).Returns(false);

            var request = new List<MarketDataRequest>
            {
                BuildRequest("d")
            };

           Assert.DoesNotThrowAsync(async () => await synchroniser.Handle("a", _requestContext, request));
        }

        private FactsetDataSynchroniser Build()
        {
            return new FactsetDataSynchroniser(_dataRequestsManager, _requestFilter, _logger);
        }

        private MarketDataRequest BuildRequest(string cfi)
        {
            return new MarketDataRequest("a", "XLON", cfi, InstrumentIdentifiers.Null(), null, null, "1", true);
        }
    }
}
