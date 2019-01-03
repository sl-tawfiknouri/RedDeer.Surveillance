﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Financial;
using DomainV2.Markets;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using ThirdPartySurveillanceDataSynchroniser.DataSources;
using ThirdPartySurveillanceDataSynchroniser.Manager;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Tests.Manager.Bmll
{
    [TestFixture]
    public class BmllDataRequestsManagerTests
    {
        private IBmllDataRequestsStorageManager _storageManager;
        private IBmllDataRequestsRescheduleManager _rescheduleManager;
        private ILogger<BmllDataRequestsManager> _logger;

        [SetUp]
        public void Setup()
        {
            _storageManager = A.Fake<IBmllDataRequestsStorageManager>();
            _rescheduleManager = A.Fake<IBmllDataRequestsRescheduleManager>();
            _logger = A.Fake<ILogger<BmllDataRequestsManager>>();
        }

        [Test]
        public void Constructor_ConsidersNull_StorageManager_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsManager(null, _rescheduleManager, _logger));
        }

        [Test]
        public void Constructor_ConsidersNull_Logger_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new BmllDataRequestsManager(_storageManager, _rescheduleManager, null));
        }

        [Test]
        public async Task Submit_DoesNothing_When_BmllRequests_Null()
        {
            var manager = new BmllDataRequestsManager(_storageManager, _rescheduleManager, _logger);

            await manager.Submit(null);

            A
                .CallTo(() => _storageManager.Store())
                .MustNotHaveHappened();

            A
                .CallTo(() => _rescheduleManager.RescheduleRuleRun())
                .MustNotHaveHappened();
        }

        [Test]
        public async Task Submit_DoesCall_Store_WhenBmllRequests_Submitted()
        {
            var manager = new BmllDataRequestsManager(_storageManager, _rescheduleManager, _logger);

            var request = new List<MarketDataRequestDataSource>()
            {
                new MarketDataRequestDataSource(DataSource.Bmll, BuildRequest())
            };

            await manager.Submit(request);

            A
                .CallTo(() => _storageManager.Store())
                .MustHaveHappened();

            A
                .CallTo(() => _rescheduleManager.RescheduleRuleRun())
                .MustHaveHappened();
        }

        [Test]
        public void Submit_DoesCall_Reschedule_When_BmllRequests_Submitted()
        {

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
                DateTime.Now, 
                DateTime.Now.AddMinutes(1),
                "test-process");
        }
    }
}
