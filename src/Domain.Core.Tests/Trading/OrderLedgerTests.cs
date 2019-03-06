using Domain.Core.Trading;
using Domain.Trading;
using FakeItEasy;
using NUnit.Framework;

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

        private OrderLedger BuildLedger()
        {
            return new OrderLedger();
        }
    }
}
