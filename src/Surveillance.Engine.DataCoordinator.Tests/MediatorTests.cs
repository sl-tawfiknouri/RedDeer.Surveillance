namespace Surveillance.Engine.DataCoordinator.Tests
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Engine.DataCoordinator.Queues.Interfaces;
    using Surveillance.Engine.DataCoordinator.Scheduler.Interfaces;

    [TestFixture]
    public class MediatorTests
    {
        private IDataCoordinatorScheduler _dataScheduler;

        private ILogger<Mediator> _logger;

        private IQueueSubscriber _queueSubscriber;

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(this._dataScheduler, this._queueSubscriber, null));
        }

        [Test]
        public void Constructor_QueueSubscriber_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(this._dataScheduler, null, this._logger));
        }

        [Test]
        public void Initiate()
        {
            var mediator = new Mediator(this._dataScheduler, this._queueSubscriber, this._logger);

            mediator.Initiate();

            A.CallTo(() => this._queueSubscriber.Initiate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._dataScheduler.Initialise()).MustHaveHappenedOnceExactly();
        }

        [SetUp]
        public void Setup()
        {
            this._dataScheduler = A.Fake<IDataCoordinatorScheduler>();
            this._queueSubscriber = A.Fake<IQueueSubscriber>();
            this._logger = A.Fake<ILogger<Mediator>>();
        }

        [Test]
        public void Terminate()
        {
            var mediator = new Mediator(this._dataScheduler, this._queueSubscriber, this._logger);

            mediator.Terminate();

            A.CallTo(() => this._queueSubscriber.Terminate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this._dataScheduler.Terminate()).MustHaveHappenedOnceExactly();
        }
    }
}