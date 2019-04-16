using Domain.Core.Trading.Orders;
using NUnit.Framework;
using SharedKernel.Files.Orders;
using System;
using System.Linq;

namespace Domain.Tests.Orders
{
    [TestFixture]
    public class OmsVersionerTests
    {
        [Test]
        public void ProjectOmsVersion_NullOrders_YieldsEmptyCollection()
        {
            var versioner = Versioner();

            var result = versioner.ProjectOmsVersion(null);

            Assert.IsEmpty(result);
        }

        [Test]
        public void ProjectOmsVersion_NoOmsVersioning_YieldsInputCollection()
        {
            var orderZero = new Order() { OrderVersionLinkId = null };
            var orderOne = new Order() { OrderVersionLinkId = "1" };
            var orderTwo = new Order() { OrderVersionLinkId = "2"};
            var orderCollection = new[] { orderZero, orderOne, orderTwo };
            var versioner = Versioner();

            var orders = versioner.ProjectOmsVersion(orderCollection);

            Assert.AreEqual(orders.Count, orderCollection.Length);
        }

        [Test]
        public void ProjectOmsVersion_OneOmsVersioning_YieldsInputCollection()
        {
            var orderZero = new Order() { OrderVersionLinkId = null };
            var orderOne1 = new Order() { OrderVersionLinkId = "1", OrderFilledVolume = 100 };
            var orderOne2 = new Order() { OrderVersionLinkId = "1", OrderFilledVolume = 200 };
            var orderTwo = new Order() { OrderVersionLinkId = "2" };
            var orderCollection = new[] { orderZero, orderOne1, orderOne2, orderTwo };
            var versioner = Versioner();

            var orders = versioner.ProjectOmsVersion(orderCollection);

            Assert.AreEqual(orders.Count, orderCollection.Length - 1);
        }

        [Test]
        public void ProjectOmsVersion_OneOmsVersioning_YieldsWithHigherOrderVersionCollection()
        {
            var orderZero = new Order() { OrderVersionLinkId = null };
            var orderOne1 = new Order() { OrderVersionLinkId = "1", OrderVersion = "2" };
            var orderOne2 = new Order() { OrderVersionLinkId = "1", OrderVersion = "1" };
            var orderTwo = new Order() { OrderVersionLinkId = "2" };
            var orderCollection = new[] { orderZero, orderOne1, orderOne2, orderTwo };
            var versioner = Versioner();

            var orders = versioner.ProjectOmsVersion(orderCollection);

            Assert.IsTrue(orders.Any(i => i.OrderVersion == "2"));
            Assert.IsTrue(!orders.Any(i => i.OrderVersion == "1"));
            Assert.AreEqual(orders.Count, orderCollection.Length - 1);
        }

        [Test]
        public void ProjectOmsVersion_OneOmsVersioning_YieldsWithHigherOrderVersionWithMonotonicButNotContinousVersioningCollection()
        {
            var orderZero = new Order() { OrderVersionLinkId = null };
            var orderOne1 = new Order() { OrderVersionLinkId = "1", OrderVersion = "2" };
            var orderOne2 = new Order() { OrderVersionLinkId = "1", OrderVersion = "1" };
            var orderTwo1 = new Order() { OrderVersionLinkId = "2", OrderVersion = "1" };
            var orderTwo2 = new Order() { OrderVersionLinkId = "2", OrderVersion = "3" };
            var orderThree = new Order() { OrderVersionLinkId = "3" };
            var orderCollection = new[] { orderZero, orderOne1, orderOne2, orderTwo1, orderTwo2, orderThree };
            var versioner = Versioner();

            var orders = versioner.ProjectOmsVersion(orderCollection);

            Assert.IsTrue(orders.Any(i => i.OrderVersionLinkId == "1" && i.OrderVersion == "2"));
            Assert.IsTrue(!orders.Any(i => i.OrderVersionLinkId == "1" && i.OrderVersion == "1"));
            Assert.IsTrue(orders.Any(i => i.OrderVersionLinkId == "2" && i.OrderVersion == "3"));
            Assert.IsTrue(!orders.Any(i => i.OrderVersionLinkId == "2" && i.OrderVersion == "1"));
            Assert.IsTrue(orders.Any(i => i.OrderVersionLinkId == "3"));
            Assert.AreEqual(orders.Count, orderCollection.Length - 2);
        }

        [Test]
        public void ProjectOmsVersion_OneOmsVersioning_CompressesNonDuplicateData()
        {
            var orderOne1 = new Order() { OrderVersionLinkId = "1", OrderVersion = "1", CancelledDate = DateTime.UtcNow };
            var orderOne2 = new Order() { OrderVersionLinkId = "1", OrderVersion = "2", CreatedDate = DateTime.UtcNow };
            var orderOne3 = new Order() { OrderVersionLinkId = "1", OrderVersion = "3", FilledDate = DateTime.UtcNow };
            var orderOne4 = new Order() { OrderVersionLinkId = "1", OrderVersion = "6", RejectedDate = DateTime.UtcNow };
            var orderCollection = new[] { orderOne1, orderOne2, orderOne3, orderOne4 };
            var versioner = Versioner();

            var orders = versioner.ProjectOmsVersion(orderCollection);

            Assert.IsTrue(
                orders.Any(i => 
                i.OrderVersionLinkId == "1"
                && i.OrderVersion == "6" 
                && i.CancelledDate != null 
                && i.CreatedDate != null 
                && i.FilledDate != null 
                && i.RejectedDate != null));

            Assert.IsTrue(!orders.Any(i => i.OrderVersionLinkId == "1" && i.OrderVersion == "3"));
            Assert.IsTrue(!orders.Any(i => i.OrderVersionLinkId == "1" && i.OrderVersion == "2"));
            Assert.IsTrue(!orders.Any(i => i.OrderVersionLinkId == "1" && i.OrderVersion == "1"));
        }

        private OmsVersioner Versioner()
        {
            return new OmsVersioner(new OmsOrderFieldCompression());
        }
    }
}
