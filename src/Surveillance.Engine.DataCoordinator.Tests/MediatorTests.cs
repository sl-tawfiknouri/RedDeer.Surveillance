namespace Surveillance.Engine.DataCoordinator.Tests
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Engine.DataCoordinator.Queues.Interfaces;
    using Surveillance.Engine.DataCoordinator.Scheduler.Interfaces;

    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// The mediator tests.
    /// </summary>
    [TestFixture]
    public class MediatorTests
    {
        /// <summary>
        /// The data scheduler.
        /// </summary>
        private IDataCoordinatorScheduler dataScheduler;

        /// <summary>
        /// The queue subscriber.
        /// </summary>
        private IQueueSubscriber queueSubscriber;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<Mediator> logger;

        /// <summary>
        /// The constructor logger null throws exception.
        /// </summary>
        [Test]
        public void ConstructorLoggerNullThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new Mediator(this.dataScheduler, this.queueSubscriber, null));
        }

        /// <summary>
        /// The constructor queue subscriber null throws exception.
        /// </summary>
        [Test]
        public void ConstructorQueueSubscriberNullThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new Mediator(this.dataScheduler, null, this.logger));
        }

        /// <summary>
        /// The initiate.
        /// </summary>
        [Test]
        public void Initiate()
        {
            var mediator = new Mediator(this.dataScheduler, this.queueSubscriber, this.logger);

            mediator.Initiate();

            A.CallTo(() => this.queueSubscriber.Initiate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.dataScheduler.Initialise()).MustHaveHappenedOnceExactly();
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.dataScheduler = A.Fake<IDataCoordinatorScheduler>();
            this.queueSubscriber = A.Fake<IQueueSubscriber>();
            this.logger = A.Fake<ILogger<Mediator>>();
        }

        /// <summary>
        /// The terminate.
        /// </summary>
        [Test]
        public void Terminate()
        {
            var mediator = new Mediator(this.dataScheduler, this.queueSubscriber, this.logger);

            mediator.Terminate();

            A.CallTo(() => this.queueSubscriber.Terminate()).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.dataScheduler.Terminate()).MustHaveHappenedOnceExactly();
        }
    }
}