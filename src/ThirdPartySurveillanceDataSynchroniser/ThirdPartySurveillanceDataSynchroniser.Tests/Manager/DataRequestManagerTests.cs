namespace DataSynchroniser.Tests.Manager
{
    using System;
    using System.Threading.Tasks;

    using DataSynchroniser.Api.Bmll.Interfaces;
    using DataSynchroniser.Api.Factset.Interfaces;
    using DataSynchroniser.Api.Markit.Interfaces;
    using DataSynchroniser.Api.Refinitiv.Interfaces;
    using DataSynchroniser.Manager;
    using DataSynchroniser.Queues.Interfaces;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.DataLayer.Aurora.BMLL.Interfaces;

    [TestFixture]
    public class DataRequestManagerTests
    {
        private IBmllDataSynchroniser _bmllSynchroniser;

        private ISystemProcessOperationThirdPartyDataRequestContext _dataRequestContext;

        private IFactsetDataSynchroniser _factsetSynchroniser;

        private ILogger<DataRequestManager> _logger;

        private IMarkitDataSynchroniser _markitSynchroniser;

        private IRefinitivDataSynchroniser _refinitivSynchroniser;

        private IRuleRunDataRequestRepository _repository;

        private IScheduleRulePublisher _scheduleRulePublisher;

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new DataRequestManager(
                    this._bmllSynchroniser,
                    this._factsetSynchroniser,
                    this._markitSynchroniser,
                    this._refinitivSynchroniser,
                    this._scheduleRulePublisher,
                    this._repository,
                    null));
        }

        [Test]
        public void Constructor_Null_Repository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new DataRequestManager(
                    this._bmllSynchroniser,
                    this._factsetSynchroniser,
                    this._markitSynchroniser,
                    this._refinitivSynchroniser,
                    this._scheduleRulePublisher,
                    null,
                    this._logger));
        }

        [Test]
        public async Task Handle_FetchesDataFromRepo()
        {
            var manager = this.BuildManager();

            await manager.Handle("1", this._dataRequestContext);

            A.CallTo(() => this._repository.DataRequestsForSystemOperation("1")).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Handle_NullRuleRunId_SetsContextEventError()
        {
            var manager = this.BuildManager();

            await manager.Handle(null, this._dataRequestContext);

            A.CallTo(() => this._dataRequestContext.EventError(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._bmllSynchroniser = A.Fake<IBmllDataSynchroniser>();
            this._factsetSynchroniser = A.Fake<IFactsetDataSynchroniser>();
            this._markitSynchroniser = A.Fake<IMarkitDataSynchroniser>();
            this._refinitivSynchroniser = A.Fake<IRefinitivDataSynchroniser>();
            this._dataRequestContext = A.Fake<ISystemProcessOperationThirdPartyDataRequestContext>();
            this._scheduleRulePublisher = A.Fake<IScheduleRulePublisher>();
            this._repository = A.Fake<IRuleRunDataRequestRepository>();
            this._logger = A.Fake<ILogger<DataRequestManager>>();
        }

        private DataRequestManager BuildManager()
        {
            return new DataRequestManager(
                this._bmllSynchroniser,
                this._factsetSynchroniser,
                this._markitSynchroniser,
                this._refinitivSynchroniser,
                this._scheduleRulePublisher,
                this._repository,
                this._logger);
        }
    }
}