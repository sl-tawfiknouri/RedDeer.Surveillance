using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainV2.Financial;
using DomainV2.Trading;
using FakeItEasy;
using NUnit.Framework;
using Surveillance.DataLayer.Aurora.Trade;
using Surveillance.Trades;

namespace Surveillance.Tests.Trades
{
    [TestFixture]
    public class OrdersToAllocatedOrdersProjectorTests
    {
        private IOrderAllocationRepository _repository;

        [SetUp]
        public void Setup()
        {
            _repository = A.Fake<IOrderAllocationRepository>();
        }

        [Test]
        public void Constructor_Null_Repository_Throws_Exception()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new OrdersToAllocatedOrdersProjector(null));
        }

        [Test]
        public async Task DecoratedOrders_Returns_Empty_For_Null()
        {
            var projector = Build();

            var result = await projector.DecorateOrders(null);

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task DecoratedOrders_Returns_100Percent_Allocated_WhenNotIn_Repo()
        {
            var projector = Build();
            var orders = OrderHelper.Orders(OrderStatus.Filled);

            var result = await projector.DecorateOrders(new[] {orders});

            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result.First().OrderFilledVolume, orders.OrderFilledVolume);
            Assert.AreEqual(result.First().OrderFund, string.Empty);
            Assert.AreEqual(result.First().OrderStrategy, string.Empty);
            Assert.AreEqual(result.First().OrderClientAccountAttributionId, string.Empty);
            Assert.AreEqual(result.First().OrderId, orders.OrderId);
            Assert.AreEqual(result.First().OrderFilledVolume, orders.OrderFilledVolume);
            Assert.AreEqual(result.First().OrderOrderedVolume, orders.OrderOrderedVolume);
        }

        [Test]
        public async Task DecoratedOrders_Returns_50Percent_Allocated_WhenIn_Repo()
        {
            var projector = Build();
            var orders = OrderHelper.Orders(OrderStatus.Filled);
            orders.OrderFilledVolume = 100;
            orders.OrderOrderedVolume = 1000;
            var fiftyPercentVolume = (long)(orders.OrderFilledVolume.GetValueOrDefault(0) * 0.5);

            var allocations = new List<OrderAllocation>
            {
                new OrderAllocation(
                    null,
                    orders.OrderId,
                    "test-fund-1",
                    "test-strategy-1",
                    "test-account-1",
                    fiftyPercentVolume),
                new OrderAllocation(
                    null,
                    orders.OrderId,
                    "test-fund-2",
                    "test-strategy-2",
                    "test-account-2",
                    fiftyPercentVolume)
            };

            A.CallTo(() => _repository.Get(A<IReadOnlyCollection<string>>.Ignored)).Returns(allocations);

            var result = await projector.DecorateOrders(new[] { orders });

            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result.First().OrderFilledVolume, 50);
            Assert.AreEqual(result.First().OrderOrderedVolume, 500);
            Assert.AreEqual(result.First().OrderFund, "test-fund-1");
            Assert.AreEqual(result.First().OrderStrategy, "test-strategy-1");
            Assert.AreEqual(result.First().OrderClientAccountAttributionId, "test-account-1");
            Assert.AreEqual(result.First().OrderId, orders.OrderId);

            Assert.AreEqual(result.Skip(1).First().OrderFilledVolume, 50);
            Assert.AreEqual(result.Skip(1).First().OrderOrderedVolume, 500);
            Assert.AreEqual(result.Skip(1).First().OrderFund, "test-fund-2");
            Assert.AreEqual(result.Skip(1).First().OrderStrategy, "test-strategy-2");
            Assert.AreEqual(result.Skip(1).First().OrderClientAccountAttributionId, "test-account-2");
            Assert.AreEqual(result.Skip(1).First().OrderId, orders.OrderId);
        }

        private OrdersToAllocatedOrdersProjector Build()
        {
            return new OrdersToAllocatedOrdersProjector(_repository);
        }
    }
}
