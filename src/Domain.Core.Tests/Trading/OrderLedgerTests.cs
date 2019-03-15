using Domain.Core.Trading;
using FakeItEasy;
using NUnit.Framework;
using System;
using Domain.Core.Trading.Orders;

namespace Domain.Core.Tests.Trading
{
    [TestFixture]
    public class OrderLedgerTests
    {
        [Test]
        public void Add_ThenFullLedger_ReturnsOrder()
        {
            var ledger = BuildLedger();
            var order = A.Fake<Order>();

            ledger.Add(order);

            var fullLedger = ledger.FullLedger();

            Assert.AreEqual(fullLedger.Count, 1);
        }

        [Test]
        public void FullLedger_NoOrders_EmptyList()
        {
            var ledger = BuildLedger();

            var orderLedger = ledger.FullLedger();

            Assert.AreEqual(orderLedger.Count, 0);
        }

        [Test]
        public void LedgerEntries_OutOfRange_ReturnEmpty()
        {
            var startDate = new DateTime(2018, 01, 01);
            var ledger = BuildLedger();
            var order = A.Fake<Order>();
            order.PlacedDate = startDate;

            ledger.Add(order);
            var orderLedger = ledger.LedgerEntries(startDate.AddMinutes(2), TimeSpan.FromMinutes(1));

            Assert.AreEqual(orderLedger.Count, 0);
        }


        [Test]
        public void LedgerEntries_InRange_ReturnOneOrder()
        {
            var startDate = new DateTime(2018, 01, 01);
            var ledger = BuildLedger();
            var order = A.Fake<Order>();
            order.PlacedDate = startDate.AddSeconds(30);

            ledger.Add(order);
            var orderLedger = ledger.LedgerEntries(startDate, TimeSpan.FromMinutes(1));

            Assert.AreEqual(orderLedger.Count, 1);
        }

        [Test]
        public void LedgerEntries_InRange_ReturnTwoOrders()
        {
            var startDate = new DateTime(2018, 01, 01);
            var ledger = BuildLedger();
            var order = A.Fake<Order>();
            order.PlacedDate = startDate.AddSeconds(30);
            var order2 = A.Fake<Order>();
            order2.PlacedDate = startDate.AddSeconds(45);

            ledger.Add(order);
            ledger.Add(order2);

            var orderLedger = ledger.LedgerEntries(startDate, TimeSpan.FromMinutes(1));

            Assert.AreEqual(orderLedger.Count, 2);
        }

        [Test]
        public void VolumeInLedger_ReturnsVolume_ForZeroOrders()
        {
            var ledger = BuildLedger();

            var result = ledger.VolumeInLedger();

            Assert.AreEqual(0, result);
        }

        [Test]
        public void VolumeInLedger_ReturnsVolume_ForOneFilledOrders()
        {
            var ledger = BuildLedger();
            var order = A.Fake<Order>();
            order.OrderFilledVolume = 100;
            ledger.Add(order);

            var result = ledger.VolumeInLedger();

            Assert.AreEqual(100, result);
        }

        [Test]
        public void VolumeInLedger_ReturnsVolume_ForOneUnfilledOrders()
        {
            var ledger = BuildLedger();
            var order = A.Fake<Order>();
            order.OrderOrderedVolume = 100;
            ledger.Add(order);

            var result = ledger.VolumeInLedger();

            Assert.AreEqual(100, result);
        }

        [Test]
        public void VolumeInLedger_ReturnsFilledVolume_ForOneOrderWithOrderedAndFilledVolumes()
        {
            var ledger = BuildLedger();
            var order = A.Fake<Order>();
            order.OrderOrderedVolume = 100;
            order.OrderFilledVolume = 50;
            ledger.Add(order);

            var result = ledger.VolumeInLedger();

            Assert.AreEqual(50, result);
        }

        [Test]
        public void VolumeInLedgerWithStatus_ReturnsSum_InStatusCancelled()
        {
            var ledger = BuildLedger();
            var order1 = A.Fake<Order>();
            order1.OrderOrderedVolume = 1000;
            order1.CancelledDate = DateTime.UtcNow;

            var order2 = A.Fake<Order>();
            order2.OrderOrderedVolume = 100;
            order2.OrderFilledVolume = 50;
            order2.RejectedDate = DateTime.UtcNow;

            var order3 = A.Fake<Order>();
            order3.OrderOrderedVolume = 100;
            order3.RejectedDate = DateTime.UtcNow;

            ledger.Add(order1);
            ledger.Add(order2);
            ledger.Add(order3);

            var result = ledger.VolumeInLedgerWithStatus(OrderStatus.Rejected);

            Assert.AreEqual(150, result);
        }

        [Test]
        public void PercentageInStatusByOrder_Returns0_ForNoOrders()
        {
            var ledger = BuildLedger();

            var result = ledger.PercentageInStatusByOrder(OrderStatus.Rejected);

            Assert.AreEqual(0, result);
        }

        [Test]
        public void PercentageInStatusByOrder_Returns0_ForNoOrdersInStatus()
        {
            var ledger = BuildLedger();
            var order1 = A.Fake<Order>();
            order1.OrderOrderedVolume = 1000;
            order1.CancelledDate = DateTime.UtcNow;
            ledger.Add(order1);

            var result = ledger.PercentageInStatusByOrder(OrderStatus.Rejected);

            Assert.AreEqual(0, result);
        }

        [Test]
        public void PercentageInStatusByOrder_ReturnsFiftfyPercent_ForOrdersInStatus()
        {
            var ledger = BuildLedger();
            var order1 = A.Fake<Order>();
            order1.OrderOrderedVolume = 1000;
            order1.CancelledDate = DateTime.UtcNow;

            var order2 = A.Fake<Order>();
            order2.OrderOrderedVolume = 50;
            order2.RejectedDate = DateTime.UtcNow;

            ledger.Add(order1);
            ledger.Add(order2);

            var result = ledger.PercentageInStatusByOrder(OrderStatus.Rejected);

            Assert.AreEqual(0.5m, result);
        }

        [Test]
        public void PercentageInStatusByOrder_ReturnsFiftfyPercent_ForVolumeInStatus()
        {
            var ledger = BuildLedger();
            var order1 = A.Fake<Order>();
            order1.OrderOrderedVolume = 1000;
            order1.CancelledDate = DateTime.UtcNow;

            var order2 = A.Fake<Order>();
            order2.OrderOrderedVolume = 1000;
            order2.RejectedDate = DateTime.UtcNow;

            ledger.Add(order1);
            ledger.Add(order2);

            var result = ledger.PercentageInStatusByVolume(OrderStatus.Rejected);

            Assert.AreEqual(0.5m, result);
        }

        private OrderLedger BuildLedger()
        {
            return new OrderLedger();
        }
    }
}
