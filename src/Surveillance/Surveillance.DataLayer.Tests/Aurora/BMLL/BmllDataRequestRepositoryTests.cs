namespace Surveillance.DataLayer.Tests.Aurora.BMLL
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Domain.Core.Financial.Assets;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using SharedKernel.Contracts.Markets;

    using Surveillance.DataLayer.Aurora;
    using Surveillance.DataLayer.Aurora.BMLL;
    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Configuration.Interfaces;
    using Surveillance.DataLayer.Tests.Helpers;

    /// <summary>
    ///     Correct system op id in the explicit requests will vary depending on your rule ids to sys op ids locally
    /// </summary>
    [TestFixture]
    public class BmllDataRequestRepositoryTests
    {
        private IDataLayerConfiguration _configuration;

        private IConnectionStringFactory _connectionStringFactory;

        private ILogger<RuleRunDataRequestRepository> _logger;

        [Test]
        [Explicit]
        public async Task CreateDataRequest_HasDataRequestForRuleRun_VerifiesRequest()
        {
            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(this._configuration), this._logger);
            var id = "1";

            var marketDataRequest = new MarketDataRequest(
                null,
                "XLON",
                "entsbp",
                new InstrumentIdentifiers { Id = "1" },
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                id,
                true,
                DataSource.Bmll);

            await repo.CreateDataRequest(marketDataRequest);
            var result = await repo.HasDataRequestForRuleRun(id);

            Assert.IsTrue(result);
        }

        [Test]
        [Explicit]
        public async Task CreateDataRequest_SavesToDb()
        {
            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(this._configuration), this._logger);

            var marketDataRequest = new MarketDataRequest(
                null,
                "XLON",
                "entsbp",
                new InstrumentIdentifiers { Id = "1" },
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                "2",
                true,
                DataSource.Bmll);

            await repo.CreateDataRequest(marketDataRequest);
        }

        [Test]
        public void Ctor_DbConnectionFactoryNull_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new RuleRunDataRequestRepository(null, this._logger));
        }

        [Test]
        public void Ctor_LoggerNull_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new RuleRunDataRequestRepository(this._connectionStringFactory, null));
        }

        [Test]
        public async Task DataRequestsForSystemOperation_EmptyOperationId_ReturnsEmpty()
        {
            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(this._configuration), this._logger);

            await repo.DataRequestsForSystemOperation(string.Empty);
        }

        [Test]
        [Explicit]
        public async Task DataRequestsForSystemOperation_NotStoredOperationId_ReturnsEmptyDataRequests()
        {
            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(this._configuration), this._logger);

            var id = "1";

            var marketDataRequest = new MarketDataRequest(
                null,
                "XLON",
                "entsbp",
                new InstrumentIdentifiers { Id = "1" },
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                id,
                true,
                DataSource.Bmll);

            await repo.CreateDataRequest(marketDataRequest);
            var dataRequests = await repo.DataRequestsForSystemOperation("11349043");

            Assert.IsNotNull(dataRequests);
            Assert.AreEqual(dataRequests.Count, 0);
        }

        [Test]
        public async Task DataRequestsForSystemOperation_NullOperationId_ReturnsEmpty()
        {
            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(this._configuration), this._logger);

            await repo.DataRequestsForSystemOperation(null);
        }

        [Test]
        [Explicit]
        public async Task DataRequestsForSystemOperation_StoredOperationId_ReturnsDataRequests()
        {
            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(this._configuration), this._logger);

            var id = "1";

            var marketDataRequest = new MarketDataRequest(
                null,
                "XLON",
                "entsbp",
                new InstrumentIdentifiers { Id = "1" },
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                id,
                true,
                DataSource.Bmll);

            await repo.CreateDataRequest(marketDataRequest);
            var dataRequests = await repo.DataRequestsForSystemOperation("11");

            Assert.IsNotNull(dataRequests);
            Assert.AreEqual(dataRequests.Count, 1);
        }

        [Test]
        [Explicit]
        public async Task DataRequestsForSystemOperation_UpdateToCompleteWithDuplicates_UpdatesRelevantRequests()
        {
            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(this._configuration), this._logger);

            var id = "11";

            var start = DateTime.UtcNow;
            var end = DateTime.UtcNow.AddHours(1);

            var marketDataRequest1 = new MarketDataRequest(
                null,
                "TESTME",
                "entsbp",
                new InstrumentIdentifiers { Id = "1" },
                start,
                end,
                "1",
                false,
                DataSource.Bmll);

            await repo.CreateDataRequest(marketDataRequest1);

            var marketDataRequest2 = new MarketDataRequest(
                null,
                "TESTME",
                "entsbp",
                new InstrumentIdentifiers { Id = "1" },
                start,
                end,
                "2",
                false,
                DataSource.Bmll);

            await repo.CreateDataRequest(marketDataRequest2);

            var dataRequests = (await repo.DataRequestsForSystemOperation(id))
                .Where(_ => _.MarketIdentifierCode == "TESTME").ToList();

            Assert.IsNotNull(dataRequests);
            Assert.AreEqual(dataRequests.Count, 2);
            Assert.IsFalse(dataRequests.All(_ => _.IsCompleted));

            var oneDataRequest = new[] { dataRequests.FirstOrDefault() };
            await repo.UpdateToCompleteWithDuplicates(oneDataRequest);

            var dataRequests2 = (await repo.DataRequestsForSystemOperation(id))
                .Where(_ => _.MarketIdentifierCode == "TESTME").ToList();

            Assert.IsTrue(dataRequests2.All(_ => _.IsCompleted));
        }

        [Test]
        [Explicit]
        public async Task GetDataRequests_FetchesFromDb()
        {
            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(this._configuration), this._logger);

            var results = await repo.DataRequestsForSystemOperation("2");

            Assert.IsNotNull(results);
        }

        [Test]
        [Explicit]
        public async Task GetDataRequests_UpdatesAsExpected()
        {
            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(this._configuration), this._logger);

            var marketDataRequest = new MarketDataRequest(
                "1",
                "XLON",
                "entsbp",
                new InstrumentIdentifiers { Id = "1" },
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                "2",
                true,
                DataSource.Bmll);

            await repo.UpdateToCompleteWithDuplicates(new[] { marketDataRequest });
        }

        [Test]
        public async Task MissingDataRequest_EmptyDataRequestId_VerifiesRequest()
        {
            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(this._configuration), this._logger);

            var result = await repo.HasDataRequestForRuleRun(string.Empty);

            Assert.IsFalse(result);
        }

        [Test]
        [Explicit]
        public async Task MissingDataRequest_NoDataRequestForRuleRun_VerifiesRequest()
        {
            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(this._configuration), this._logger);
            var id = "9383829-test";

            var result = await repo.HasDataRequestForRuleRun(id);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task MissingDataRequest_NullDataRequestId_VerifiesRequest()
        {
            var repo = new RuleRunDataRequestRepository(new ConnectionStringFactory(this._configuration), this._logger);

            var result = await repo.HasDataRequestForRuleRun(null);

            Assert.IsFalse(result);
        }

        [SetUp]
        public void Setup()
        {
            this._configuration = TestHelpers.Config();
            this._connectionStringFactory = A.Fake<IConnectionStringFactory>();
            this._logger = new NullLogger<RuleRunDataRequestRepository>();
        }
    }
}