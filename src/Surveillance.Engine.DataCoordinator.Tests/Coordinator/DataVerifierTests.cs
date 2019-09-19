namespace Surveillance.Engine.DataCoordinator.Tests.Coordinator
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.DataLayer.Aurora.Orders.Interfaces;
    using Surveillance.Engine.DataCoordinator.Coordinator;
    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// The data verifier tests.
    /// </summary>
    [TestFixture]
    public class DataVerifierTests
    {
        /// <summary>
        /// The order allocation repository.
        /// </summary>
        private IOrderAllocationRepository orderAllocationRepository;

        /// <summary>
        /// The orders repository.
        /// </summary>
        private IOrdersRepository ordersRepository;

        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<DataVerifier> logger;

        /// <summary>
        /// The analyze file id null message returns.
        /// </summary>
        [Test]
        public void AnalyseFileIdNullMessageReturns()
        {
            var coordinator = new DataVerifier(this.ordersRepository, this.orderAllocationRepository, this.logger);

            Assert.DoesNotThrowAsync(async () => await coordinator.Scan().ConfigureAwait(false));
        }

        /// <summary>
        /// The constructor logger null throws exception.
        /// </summary>
        [Test]
        public void ConstructorLoggerNullThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new DataVerifier(this.ordersRepository, this.orderAllocationRepository, null));
        }

        /// <summary>
        /// The constructor order allocations repository null throws exception.
        /// </summary>
        [Test]
        public void ConstructorOrderAllocationsRepositoryNullThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new DataVerifier(this.ordersRepository, null, this.logger));
        }

        /// <summary>
        /// The constructor orders repository null throws exception.
        /// </summary>
        [Test]
        public void ConstructorOrdersRepositoryNullThrowsException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new DataVerifier(null, this.orderAllocationRepository, this.logger));
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.ordersRepository = A.Fake<IOrdersRepository>();
            this.orderAllocationRepository = A.Fake<IOrderAllocationRepository>();
            this.logger = new NullLogger<DataVerifier>();
        }
    }
}