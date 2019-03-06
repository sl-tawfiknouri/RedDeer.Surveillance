using Domain.Core.Trading;
using Domain.Trading;
using FakeItEasy;
using NUnit.Framework;
using System;

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

        private OrderLedger BuildLedger()
        {
            return new OrderLedger();
        }
    }
}
