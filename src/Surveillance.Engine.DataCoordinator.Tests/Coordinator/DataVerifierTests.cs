using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora.Orders.Interfaces;
using Surveillance.Engine.DataCoordinator.Coordinator;

namespace Surveillance.Engine.DataCoordinator.Tests.Coordinator
{
    [TestFixture]
    public class DataVerifierTests
    {
        private IOrdersRepository _ordersRepository;
        private IOrderAllocationRepository _orderAllocationRepository;
        private ILogger<DataVerifier> _logger;

        [SetUp]
        public void Setup()
        {
            _ordersRepository = A.Fake<IOrdersRepository>();
            _orderAllocationRepository = A.Fake<IOrderAllocationRepository>();
            _logger = new NullLogger<DataVerifier>();
        }

        [Test]
        public void Constructor_OrdersRepository_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataVerifier(null, _orderAllocationRepository, _logger));
        }

        [Test]
        public void Constructor_OrderAllocationsRepository_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataVerifier(_ordersRepository, null, _logger));
        }

        [Test]
        public void Constructor_Logger_Null_Is_Exceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DataVerifier(_ordersRepository, _orderAllocationRepository, null));
        }

        [Test]
        public void AnalyseFileId_Null_Message_Returns()
        {
            var coordinator = new DataVerifier(_ordersRepository, _orderAllocationRepository, _logger);

            Assert.DoesNotThrow(() => coordinator.Scan().Wait());
        }
    }
}
