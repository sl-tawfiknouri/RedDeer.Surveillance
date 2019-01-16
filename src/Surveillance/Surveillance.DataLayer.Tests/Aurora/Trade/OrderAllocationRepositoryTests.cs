﻿using System;
using System.Threading.Tasks;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.DataLayer.Aurora.Trade;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.DataLayer.Tests.Helpers;

namespace Surveillance.DataLayer.Tests.Aurora.Trade
{
    [TestFixture]
    public class OrderAllocationRepositoryTests
    {
        private IDataLayerConfiguration _configuration;
        private IConnectionStringFactory _connectionFactory;
        private ILogger<OrderAllocationRepository> _logger;

        [SetUp]
        public void Setup()
        {
            _configuration = TestHelpers.Config();
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

        [Test]
        [Explicit]
        public async Task Create_Inserts_New_Row()
        {
            var factory = new ConnectionStringFactory(_configuration);
            var repo = new OrderAllocationRepository(factory, _logger);
            var orderAllocation = new OrderAllocation(null, "order-1", "my-fund", "my-strategy", "my-account", 1000);

            await repo.Create(orderAllocation);
        }


    }
}
