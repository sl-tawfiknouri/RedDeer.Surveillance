using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora.BMLL.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Manager;

namespace ThirdPartySurveillanceDataSynchroniser.Tests.Manager
{
    [TestFixture]
    public class DataRequestManagerTests
    {
        private ISystemProcessOperationThirdPartyDataRequestContext _dataRequestContext;
        private IBmllDataRequestRepository _repository;
        private ILogger<DataRequestManager> _logger;

        [SetUp]
        public void Setup()
        {
            _dataRequestContext = A.Fake<ISystemProcessOperationThirdPartyDataRequestContext>();
            _repository = A.Fake<IBmllDataRequestRepository>();
            _logger = A.Fake<ILogger<DataRequestManager>>();
        }

        [Test]
        public void Constructor_Null_Repository_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataRequestManager(null, _logger));
        }

        [Test]
        public void Constructor_Null_Logger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataRequestManager(_repository, null));
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


        private DataRequestManager BuildManager()
        {
            return new DataRequestManager(_repository, _logger);
        }
    }
}
