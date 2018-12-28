using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using ThirdPartySurveillanceDataSynchroniser.Services.Interfaces;

namespace ThirdPartySurveillanceDataSynchroniser.Tests
{
    [TestFixture]
    public class MediatorTests
    {
        private ILogger<Mediator> _logger;
        private IDataRequestsService _dataRequestService;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger<Mediator>>();
            _dataRequestService = A.Fake<IDataRequestsService>();
        }
        
        [Test]
        public void Constructor_NullDataRequestsService_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(null, _logger));
        }

        [Test]
        public void Constructor_NullLogger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(_dataRequestService, null));
        }

        [Test]
        public void Initiate_Calls_DataRequestServiceInitiate()
        {
            var mediator = new Mediator(_dataRequestService, _logger);

            mediator.Initiate();

            A.CallTo(() => _dataRequestService.Initiate()).MustHaveHappenedOnceExactly();
        }
    }
}
