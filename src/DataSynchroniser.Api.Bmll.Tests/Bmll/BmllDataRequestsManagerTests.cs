namespace DataSynchroniser.Api.Bmll.Tests.Bmll
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using DataSynchroniser.Api.Bmll.Bmll;
    using DataSynchroniser.Api.Bmll.Bmll.Interfaces;

    using Domain.Core.Financial.Assets;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using PollyFacade.Policies.Interfaces;

    using SharedKernel.Contracts.Markets;

    [TestFixture]
    public class BmllDataRequestsManagerTests
    {
        private IBmllDataRequestsApiManager _apiManager;

        private ILogger<BmllDataRequestsManager> _logger;

        private IPolicyFactory _policyFactory;

        private IBmllDataRequestsStorageManager _storageManager;

        [Test]
        public void Constructor_ConsidersNull_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new BmllDataRequestsManager(this._apiManager, this._storageManager, this._policyFactory, null));
        }

        [Test]
        public void Constructor_StorageManager_IsNull_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new BmllDataRequestsManager(this._apiManager, null, this._policyFactory, this._logger));
        }

        [SetUp]
        public void Setup()
        {
            this._apiManager = A.Fake<IBmllDataRequestsApiManager>();
            this._policyFactory = A.Fake<IPolicyFactory>();
            this._storageManager = A.Fake<IBmllDataRequestsStorageManager>();
            this._logger = A.Fake<ILogger<BmllDataRequestsManager>>();
        }

        [Test]
        public void SplitList_SplitsAsExpected()
        {
            var list = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

            var result = BmllDataRequestsManager.SplitList(list.ToList(), 3);

            Assert.AreEqual(result.Count, 6);
        }

        [Test]
        public async Task Submit_BmllRequests_CallsStore()
        {
            var manager = new BmllDataRequestsManager(
                this._apiManager,
                this._storageManager,
                this._policyFactory,
                this._logger);

            var request = new List<MarketDataRequest> { this.BuildRequest() };

            await manager.Submit("a", request);

            A.CallTo(() => this._storageManager.Store(A<IReadOnlyCollection<IGetTimeBarPair>>.Ignored))
                .MustHaveHappened();
        }

        private MarketDataRequest BuildRequest()
        {
            return new MarketDataRequest(
                "XLON",
                "ENTSPB",
                new InstrumentIdentifiers(null, null, null, null, null, null, "figi", null, null, null, null),
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(1),
                "test-process",
                DataSource.Any);
        }
    }
}