namespace Domain.Core.Tests.Trading.Orders
{
    using System;

    using Domain.Core.Trading.Orders;

    using NUnit.Framework;

    using TestHelpers;

    [TestFixture]
    public class OrderAllocationTests
    {
        [Test]
        public void Ctor_AssignsVariables_Correctly()
        {
            var id = "id-1";
            var orderId = "order-id";
            var fund = "fund-a";
            var strategy = "strategy-b";
            var clientAccount = "client-account";
            var allocationId = "allocation-id";
            var orderFilledVolume = 1002;
            var createdDate = new DateTime(2019, 1, 1, 1, 1, 1);

            var alloc = new OrderAllocation(id, orderId, fund, strategy, clientAccount, allocationId, orderFilledVolume, createdDate);

            Assert.AreEqual(id, alloc.Id);
            Assert.AreEqual(orderId, alloc.OrderId);
            Assert.AreEqual(fund, alloc.Fund);
            Assert.AreEqual(strategy, alloc.Strategy);
            Assert.AreEqual(orderFilledVolume, alloc.OrderFilledVolume);
            Assert.AreEqual(createdDate, alloc.CreatedDate);
        }

        [Test]
        public void CtorOrder_AssignsVariables_Correctly()
        {
            var orderId = "order-id";
            var fund = "fund-a";
            var strategy = "strategy-b";
            var clientAccount = "client-account";
            var orderFilledVolume = 1002;

            var order = OrderTestHelper.Random(new Order());

            order.OrderId = orderId;
            order.OrderFund = fund;
            order.OrderStrategy = strategy;
            order.OrderClientAccountAttributionId = clientAccount;
            order.OrderFilledVolume = orderFilledVolume;

            var alloc = new OrderAllocation(order);

            Assert.AreEqual(string.Empty, alloc.Id);
            Assert.AreEqual(orderId, alloc.OrderId);
            Assert.AreEqual(fund, alloc.Fund);
            Assert.AreEqual(strategy, alloc.Strategy);
            Assert.AreEqual(orderFilledVolume, alloc.OrderFilledVolume);
            Assert.IsNull(alloc.CreatedDate);
        }

        [Test]
        public void IsValid_False_WhenNoAssociatedOrderId()
        {
            var alloc = new OrderAllocation(
                "some-primary-key",
                null,
                "fund-a",
                "strategy-b",
                "client-c",
                "allocation-id",
                100,
                DateTime.UtcNow);

            var result = alloc.IsValid();

            Assert.IsFalse(result);
        }
    }
}