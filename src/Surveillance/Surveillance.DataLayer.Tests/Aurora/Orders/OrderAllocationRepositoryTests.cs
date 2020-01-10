namespace Surveillance.DataLayer.Tests.Aurora.Orders
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Core.Trading.Orders;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.DataLayer.Aurora;
    using Surveillance.DataLayer.Aurora.Interfaces;
    using Surveillance.DataLayer.Aurora.Orders;
    using Surveillance.DataLayer.Configuration.Interfaces;
    using Surveillance.DataLayer.Tests.Helpers;

    [TestFixture]
    public class OrderAllocationRepositoryTests
    {
        private IDataLayerConfiguration _configuration;

        private IConnectionStringFactory _connectionFactory;

        private ILogger<OrderAllocationRepository> _logger;

        [Test]
        [Explicit]
        public async Task Bulk_Then_Get_Rows_Returns_Expected()
        {
            var factory = new ConnectionStringFactory(this._configuration);
            var repo = new OrderAllocationRepository(factory, this._logger);
            var orderAllocation1 = new OrderAllocation(
                null,
                "order-1",
                "my-fund",
                "my-strategy",
                "my-account",
                "my-allocation",
                1000,
                DateTime.UtcNow);
            var orderAllocation2 = new OrderAllocation(
                null,
                "order-2",
                "my-fund",
                "my-strategy",
                "my-account",
                "my-allocation",
                1000,
                DateTime.UtcNow);
            var allocations = new List<OrderAllocation> { orderAllocation1, orderAllocation2 };

            await repo.Create(allocations);

            var orderId = new List<string> { "order-1", "order-2" };
            var result = await repo.Get(orderId);

            Assert.AreEqual(result.Count, 2);
        }

        [Test]
        public void Constructor_Null_Connection_Factory()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderAllocationRepository(null, this._logger));
        }

        [Test]
        public void Constructor_Null_Logger_Throws_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderAllocationRepository(this._connectionFactory, null));
        }

        [Test]
        [Explicit]
        public async Task Create_Inserts_New_Row()
        {
            var factory = new ConnectionStringFactory(this._configuration);
            var repo = new OrderAllocationRepository(factory, this._logger);
            var orderAllocation = new OrderAllocation(
                null,
                "order-1",
                "my-fund",
                "my-strategy",
                "my-account",
                "my-allocation",
                1000,
                DateTime.UtcNow);

            await repo.Create(orderAllocation);
        }

        [Test]
        [Explicit]
        public async Task Get_Rows_Returns_Expected()
        {
            var factory = new ConnectionStringFactory(this._configuration);
            var repo = new OrderAllocationRepository(factory, this._logger);
            var orderAllocation = new OrderAllocation(
                null,
                "order-1",
                "my-fund",
                "my-strategy",
                "my-account",
                "my-allocation",
                1000,
                DateTime.UtcNow);

            await repo.Create(orderAllocation);

            var orderId = new List<string> { "order-1", "order-2" };
            var result = await repo.Get(orderId);

            Assert.AreEqual(result.Count, 1);
        }

        [SetUp]
        public void Setup()
        {
            this._configuration = TestHelpers.Config();
            this._connectionFactory = A.Fake<IConnectionStringFactory>();
            this._logger = A.Fake<ILogger<OrderAllocationRepository>>();
        }
    }
}