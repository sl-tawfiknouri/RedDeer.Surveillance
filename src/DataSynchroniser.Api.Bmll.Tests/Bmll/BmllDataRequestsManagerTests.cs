using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Api.Bmll.Bmll;
using DataSynchroniser.Api.Bmll.Bmll.Interfaces;
using Domain.Financial;
using Domain.Markets;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PollyFacade.Policies.Interfaces;

namespace DataSynchroniser.Api.Bmll.Tests.Bmll
{
    [TestFixture]
    public class BmllDataRequestsManagerTests
    {
        private IBmllDataRequestsApiManager _apiManager;
        private IPolicyFactory _policyFactory;
        private IBmllDataRequestsStorageManager _storageManager;
        private ILogger<BmllDataRequestsManager> _logger;

        [SetUp]
        public void Setup()
        {
            _apiManager = A.Fake<IBmllDataRequestsApiManager>();
            _policyFactory = A.Fake<IPolicyFactory>();
            _storageManager = A.Fake<IBmllDataRequestsStorageManager>();
            _logger = A.Fake<ILogger<BmllDataRequestsManager>>();
        }

        [Test]
        public void Constructor_StorageManager_IsNull_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsManager(_apiManager, null, _policyFactory, _logger));
        }

        [Test]
        public void Constructor_ConsidersNull_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsManager(_apiManager, _storageManager, _policyFactory, null));
        }

        [Test]
        public async Task Submit_BmllRequests_CallsStore()
        {
            var manager = new BmllDataRequestsManager(_apiManager, _storageManager, _policyFactory, _logger);

            var request = new List<MarketDataRequest>()
            {
                BuildRequest()
            };

            await manager.Submit("a", request);

            A
                .CallTo(() => _storageManager.Store(A<IReadOnlyCollection<IGetTimeBarPair>>.Ignored))
                .MustHaveHappened();
        }

        [Test]
        public void SplitList_SplitsAsExpected()
        {
            var list = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};

            var result = BmllDataRequestsManager.SplitList(list.ToList(), 3);

            Assert.AreEqual(result.Count, 6);
        }

        private MarketDataRequest BuildRequest()
        {
            return new MarketDataRequest(
                "XLON",
                "ENTSPB",
                new InstrumentIdentifiers(
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    "figi",
                    null,
                    null,
                    null,
                    null),
                DateTime.UtcNow, 
                DateTime.UtcNow.AddMinutes(1),
                "test-process");
        }
    }
}
