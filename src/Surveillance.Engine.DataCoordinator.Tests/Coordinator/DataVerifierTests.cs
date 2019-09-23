namespace Surveillance.Engine.DataCoordinator.Tests.Coordinator
{
    using System;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using Surveillance.DataLayer.Aurora.Orders.Interfaces;
    using Surveillance.Engine.DataCoordinator.Coordinator;

    [TestFixture]
    public class DataVerifierTests
    {
        private ILogger<DataVerifier> _logger;

        private IOrderAllocationRepository _orderAllocationRepository;

        private IOrdersRepository _ordersRepository;

        [Test]
        public void AnalyseFileId_Null_Message_Returns()
        {
            var coordinator = new DataVerifier(this._ordersRepository, this._orderAllocationRepository, this._logger);

            Assert.DoesNotThrow(() => coordinator.Scan().Wait());
        }

        [Test]
        public void Constructor_Logger_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new DataVerifier(this._ordersRepository, this._orderAllocationRepository, null));
        }

        [Test]
        public void Constructor_OrderAllocationsRepository_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataVerifier(this._ordersRepository, null, this._logger));
        }

        [Test]
        public void Constructor_OrdersRepository_Null_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new DataVerifier(null, this._orderAllocationRepository, this._logger));
        }

        [SetUp]
        public void Setup()
        {
            this._ordersRepository = A.Fake<IOrdersRepository>();
            this._orderAllocationRepository = A.Fake<IOrderAllocationRepository>();
            this._logger = new NullLogger<DataVerifier>();
        }
    }
}