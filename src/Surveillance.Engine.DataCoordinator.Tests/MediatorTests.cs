using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Engine.DataCoordinator.Queues.Interfaces;

namespace Surveillance.Engine.DataCoordinator.Tests
{
    [TestFixture]
    public class MediatorTests
    {
        private IQueueSubscriber _queueSubscriber;
        private ILogger<Mediator> _logger;

        [SetUp]
        public void Setup()
        {
            _queueSubscriber = A.Fake<IQueueSubscriber>();
            _logger = A.Fake<ILogger<Mediator>>();
        }

        [Test]
        public void Constructor_QueueSubscriber_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(null, _logger));
        }

        [Test]
        public void Constructor_Logger_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new Mediator(_queueSubscriber, null));
        }

        [Test]
        public void Initiate()
        {
            var mediator = new Mediator(_queueSubscriber, _logger);

            mediator.Initiate();

            A.CallTo(() => _queueSubscriber.Initiate()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Terminate()
        {
            var mediator = new Mediator(_queueSubscriber, _logger);

            mediator.Terminate();

            A.CallTo(() => _queueSubscriber.Terminate()).MustHaveHappenedOnceExactly();
        }
    }
}
