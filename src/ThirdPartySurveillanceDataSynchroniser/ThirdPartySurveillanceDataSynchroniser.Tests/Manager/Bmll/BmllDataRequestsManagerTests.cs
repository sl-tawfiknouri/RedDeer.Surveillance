using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSynchroniser.Manager.Bmll;
using DataSynchroniser.Manager.Bmll.Interfaces;
using Domain.Financial;
using Domain.Markets;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace DataSynchroniser.Tests.Manager.Bmll
{
    [TestFixture]
    public class BmllDataRequestsManagerTests
    {
        private IBmllDataRequestsSenderManager _senderManager;
        private IBmllDataRequestsStorageManager _storageManager;
        private IBmllDataRequestsRescheduleManager _rescheduleManager;
        private ILogger<BmllDataRequestsManager> _logger;

        [SetUp]
        public void Setup()
        {
            _senderManager = A.Fake<IBmllDataRequestsSenderManager>();
            _storageManager = A.Fake<IBmllDataRequestsStorageManager>();
            _rescheduleManager = A.Fake<IBmllDataRequestsRescheduleManager>();
            _logger = A.Fake<ILogger<BmllDataRequestsManager>>();
        }

        [Test]
        public void Constructor_ConsidersNull_StorageManager_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsManager(_senderManager, null, _rescheduleManager, _logger));
        }

        [Test]
        public void Constructor_ConsidersNull_Logger_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsManager(_senderManager, _storageManager, _rescheduleManager, null));
        }

        [Test]
        public async Task Submit_ReschedulesRun_When_BmllRequests_Null()
        {
            var manager = new BmllDataRequestsManager(_senderManager, _storageManager, _rescheduleManager, _logger);

            await manager.Submit("a", null);

            A
                .CallTo(() => _storageManager.Store(A<IReadOnlyCollection<IGetTimeBarPair>>.Ignored))
                .MustNotHaveHappened();

            A
                .CallTo(() => _rescheduleManager.RescheduleRuleRun("a", A<List<MarketDataRequest>>.Ignored))
                .MustHaveHappened();
        }

        [Test]
        public async Task Submit_DoesCall_Store_WhenBmllRequests_Submitted()
        {
            var manager = new BmllDataRequestsManager(_senderManager, _storageManager, _rescheduleManager, _logger);

            var request = new List<MarketDataRequest>()
            {
                BuildRequest()
            };

            await manager.Submit("a", request);

            A
                .CallTo(() => _storageManager.Store(A<IReadOnlyCollection<IGetTimeBarPair>>.Ignored))
                .MustHaveHappened();

            A
                .CallTo(() => _rescheduleManager.RescheduleRuleRun("a", A<List<MarketDataRequest>>.Ignored))
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
