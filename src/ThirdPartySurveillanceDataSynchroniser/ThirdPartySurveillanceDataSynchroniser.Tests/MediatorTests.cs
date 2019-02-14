using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Utilities.Interfaces;
using ThirdPartySurveillanceDataSynchroniser.Services.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Tests
{
    [TestFixture]
    public class MediatorTests
    {
        private ILogger<Mediator> _logger;
        private IDataRequestsService _dataRequestService;
        private IApplicationHeartbeatService _applicationHeartbeatService;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<Mediator>>();
            _dataRequestService = A.Fake<IDataRequestsService>();
            _applicationHeartbeatService = A.Fake<IApplicationHeartbeatService>();
        }
        
        [Test]
        public void Constructor_NullDataRequestsService_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(null, _applicationHeartbeatService, _logger));
        }

        [Test]
        public void Constructor_NullApplicationHeartbeatService_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(_dataRequestService, null, _logger));
        }

        [Test]
        public void Constructor_NullLogger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(_dataRequestService, _applicationHeartbeatService, null));
        }

        [Test]
        public void Initiate_Calls_DataRequestServiceInitiate()
        {
            var mediator = new Mediator(_dataRequestService, _applicationHeartbeatService, _logger);

            mediator.Initiate();

            A.CallTo(() => _dataRequestService.Initiate()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Initiate_Calls_ApplicationHeartbeatServiceInitiate()
        {
            var mediator = new Mediator(_dataRequestService, _applicationHeartbeatService, _logger);

            mediator.Initiate();

            A.CallTo(() => _applicationHeartbeatService.Initialise()).MustHaveHappenedOnceExactly();
        }

    }
}
