using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Trade;


namespace Surveillance.DataLayer.Tests.Aurora.Trade
{
    [TestFixture]
    public class OrderAllocationRepositoryTests
    {
        private IConnectionStringFactory _connectionFactory;
        private ILogger<OrderAllocationRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _connectionFactory = A.Fake<IConnectionStringFactory>();
            _logger = A.Fake<ILogger<OrderAllocationRepository>>();
        }

        [Test]
        public void Constructor_Null_Logger_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderAllocationRepository(_connectionFactory , null));
        }

        [Test]
        public void Constructor_Null_Connection_Factory()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderAllocationRepository(null, _logger));
        }



        
    }
}
