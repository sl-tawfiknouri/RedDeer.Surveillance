namespace DataSynchroniser.Tests
{
    using System;

    using DataSynchroniser.Queues.Interfaces;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Auditing.Utilities.Interfaces;

    [TestFixture]
    public class MediatorTests
    {
        private IApplicationHeartbeatService _applicationHeartbeatService;

        private IDataRequestSubscriber _dataRequestService;

        private ILogger<Mediator> _logger;

        [Test]
        public void Constructor_NullApplicationHeartbeatService_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(this._dataRequestService, null, this._logger));
        }

        [Test]
        public void Constructor_NullDataRequestsService_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new Mediator(null, this._applicationHeartbeatService, this._logger));
        }

        [Test]
        public void Constructor_NullLogger_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new Mediator(this._dataRequestService, this._applicationHeartbeatService, null));
        }

        [Test]
        public void Initiate_Calls_ApplicationHeartbeatServiceInitiate()
        {
            var mediator = new Mediator(this._dataRequestService, this._applicationHeartbeatService, this._logger);

            mediator.Initiate();

            A.CallTo(() => this._applicationHeartbeatService.Initialise()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Initiate_Calls_DataRequestServiceInitiate()
        {
            var mediator = new Mediator(this._dataRequestService, this._applicationHeartbeatService, this._logger);

            mediator.Initiate();

            A.CallTo(() => this._dataRequestService.Initiate()).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._logger = A.Fake<ILogger<Mediator>>();
            this._dataRequestService = A.Fake<IDataRequestSubscriber>();
            this._applicationHeartbeatService = A.Fake<IApplicationHeartbeatService>();
        }
    }
}