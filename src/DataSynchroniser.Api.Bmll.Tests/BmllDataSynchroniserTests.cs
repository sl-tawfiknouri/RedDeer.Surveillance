using System;
using DataSynchroniser.Api.Bmll.Bmll.Interfaces;
using DataSynchroniser.Api.Bmll.Filters;
using DataSynchroniser.Api.Bmll.Filters.Interfaces;
using Domain.Markets;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;

namespace DataSynchroniser.Api.Bmll.Tests
{
    [TestFixture]
    public class BmllDataSynchroniserTests
    {
        private IBmllDataRequestManager _dataRequestManager;
        private IMarketDataRequestFilter _filter;
        private ISystemProcessOperationThirdPartyDataRequestContext _requestContext;
        private ILogger<BmllDataSynchroniser> _logger;

        [SetUp]
        public void Setup()
        {
            _dataRequestManager = A.Fake<IBmllDataRequestManager>();
            _filter = new MarketDataRequestFilter();
            _requestContext = A.Fake<ISystemProcessOperationThirdPartyDataRequestContext>();
            _logger = A.Fake<ILogger<BmllDataSynchroniser>>();
        }

        [Test]
        public void Constructor_Logger_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataSynchroniser(_dataRequestManager, _filter, null));
        }

        [Test]
        public void Constructor_Filter_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataSynchroniser(_dataRequestManager, null, _logger));
        }

        [Test]
        public void Handle_SystemProcessOperationId_Null_DoesNotThrow()
        {
            var dataSynchroniser = Build();

           Assert.DoesNotThrowAsync(async () => await dataSynchroniser.Handle(null, _requestContext, new MarketDataRequest[0]));
        }

        [Test]
        public void Handle_DataRequestContext_Null_DoesNotThrow()
        {
            var dataSynchroniser = Build();

            Assert.DoesNotThrowAsync(async () => await dataSynchroniser.Handle("a", null, new MarketDataRequest[0]));
        }

        [Test]
        public void Handle_MarketDataRequests_Null_DoesNotThrow()
        {
            var dataSynchroniser = Build();

            Assert.DoesNotThrowAsync(async () => await dataSynchroniser.Handle("b", _requestContext, null));
        }

        private BmllDataSynchroniser Build()
        {
            return new BmllDataSynchroniser(_dataRequestManager, _filter, _logger);
        }

    }
}
