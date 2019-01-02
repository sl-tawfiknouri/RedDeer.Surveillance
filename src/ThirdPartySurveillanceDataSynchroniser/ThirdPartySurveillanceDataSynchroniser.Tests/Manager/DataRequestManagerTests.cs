using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.DataSources.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Manager;
using ThirdPartySurveillanceDataSynchroniser.Manager.Bmll.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Tests.Manager
{
    [TestFixture]
    public class DataRequestManagerTests
    {
        private IDataSourceClassifier _dataSourceClassifier;
        private ISystemProcessOperationThirdPartyDataRequestContext _dataRequestContext;
        private IBmllDataRequestManager _dataRequestManager;
        private IBmllDataRequestRepository _repository;
        private ILogger<DataRequestManager> _logger;

        [SetUp]
        public void Setup()
        {
            _dataSourceClassifier = A.Fake<IDataSourceClassifier>();
            _dataRequestContext = A.Fake<ISystemProcessOperationThirdPartyDataRequestContext>();
            _dataRequestManager = A.Fake<IBmllDataRequestManager>();
            _repository = A.Fake<IBmllDataRequestRepository>();
            _logger = A.Fake<ILogger<DataRequestManager>>();
        }

        [Test]
        public void Constructor_Null_Repository_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataRequestManager(_dataSourceClassifier, null, _dataRequestManager, _logger));
        }

        [Test]
        public void Constructor_Null_Logger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataRequestManager(_dataSourceClassifier, _repository, _dataRequestManager, null));
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
                .CallTo(() => _repository.DataRequestsForRuleRun("1"))
                .MustHaveHappenedOnceExactly();
        }

        private DataRequestManager BuildManager()
        {
            return new DataRequestManager(_dataSourceClassifier, _repository, _dataRequestManager, _logger);
        }
    }
}
