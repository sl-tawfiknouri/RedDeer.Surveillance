using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Trade;


namespace Surveillance.DataLayer.Tests.Aurora.Trade
{
    [TestFixture]
    public class OrderAttributionRepositoryTests
    {
        private IConnectionStringFactory _connectionFactory;
        private ILogger<OrderAttributionRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _connectionFactory = A.Fake<IConnectionStringFactory>();
            _logger = A.Fake<ILogger<OrderAttributionRepository>>();
        }

        [Test]
        public void Constructor_Null_Logger_Is_Exceptional()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderAttributionRepository(_connectionFactory , null));
        }

        [Test]
        public void Constructor_Null_Connection_Factory()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderAttributionRepository(null, _logger));
        }


        
    }
}
