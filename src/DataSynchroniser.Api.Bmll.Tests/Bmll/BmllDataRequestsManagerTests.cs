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

namespace DataSynchroniser.Api.Bmll.Tests.Bmll
{
    [TestFixture]
    public class BmllDataRequestsManagerTests
    {
        private IBmllDataRequestsSenderManager _senderManager;
        private IBmllDataRequestsStorageManager _storageManager;
        private ILogger<BmllDataRequestsManager> _logger;

        [SetUp]
        public void Setup()
        {
            _senderManager = A.Fake<IBmllDataRequestsSenderManager>();
            _storageManager = A.Fake<IBmllDataRequestsStorageManager>();
            _logger = A.Fake<ILogger<BmllDataRequestsManager>>();
        }

        [Test]
        public void Constructor_ConsidersNull_StorageManager_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsManager(_senderManager, null,  _logger));
        }

        [Test]
        public void Constructor_ConsidersNull_Logger_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsManager(_senderManager, _storageManager, null));
        }

        [Test]
        public async Task Submit_DoesCall_Store_WhenBmllRequests_Submitted()
        {
            var manager = new BmllDataRequestsManager(_senderManager, _storageManager, _logger);

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
