using System;
using System.Threading.Tasks;
using DataSynchroniser.Api.Bmll.Interfaces;
using DataSynchroniser.Api.Factset.Interfaces;
using DataSynchroniser.Api.Markit.Interfaces;
using DataSynchroniser.Manager;
using DataSynchroniser.Queues.Interfaces;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;

namespace DataSynchroniser.Tests.Manager
{
    [TestFixture]
    public class DataRequestManagerTests
    {
        private IBmllDataSynchroniser _bmllSynchroniser;
        private IFactsetDataSynchroniser _factsetSynchroniser;
        private IMarkitDataSynchroniser _markitSynchroniser;
        private ISystemProcessOperationThirdPartyDataRequestContext _dataRequestContext;
        private IScheduleRulePublisher _scheduleRulePublisher;
        private IRuleRunDataRequestRepository _repository;
        private ILogger<DataRequestManager> _logger;

        [SetUp]
        public void Setup()
        {
            _bmllSynchroniser = A.Fake<IBmllDataSynchroniser>();
            _factsetSynchroniser = A.Fake<IFactsetDataSynchroniser>();
            _markitSynchroniser = A.Fake<IMarkitDataSynchroniser>();
            _dataRequestContext = A.Fake<ISystemProcessOperationThirdPartyDataRequestContext>();
            _scheduleRulePublisher = A.Fake<IScheduleRulePublisher>();
            _repository = A.Fake<IRuleRunDataRequestRepository>();
            _logger = A.Fake<ILogger<DataRequestManager>>();
        }

        [Test]
        public void Constructor_Null_Repository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => 
                new DataRequestManager(
                    _bmllSynchroniser,
                    _factsetSynchroniser,
                    _markitSynchroniser,
                    _scheduleRulePublisher,
                    null, 
                    _logger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => 
                new DataRequestManager(
                    _bmllSynchroniser,
                    _factsetSynchroniser,
                    _markitSynchroniser,
                    _scheduleRulePublisher, 
                    _repository, 
                    null));
        }

        [Test]
        public async Task Handle_NullRuleRunId_SetsContextEventError()
        {
            var manager = BuildManager();

            await manager.Handle(null, _dataRequestContext);

            A
                .CallTo(() => _dataRequestContext.EventError(A<string>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Handle_FetchesDataFromRepo()
        {
            var manager = BuildManager();

            await manager.Handle("1", _dataRequestContext);

            A
                .CallTo(() => _repository.DataRequestsForSystemOperation("1"))
                .MustHaveHappenedOnceExactly();
        }

        private DataRequestManager BuildManager()
        {
            return new DataRequestManager(_bmllSynchroniser, _factsetSynchroniser, _markitSynchroniser, _scheduleRulePublisher, _repository, _logger);
        }
    }
}
