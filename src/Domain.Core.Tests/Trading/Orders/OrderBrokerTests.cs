namespace Domain.Core.Tests.Trading.Orders
{
    using System;
    using Domain.Core.Trading.Orders;
    using NUnit.Framework;

    [TestFixture]
    public class OrderBrokerTests
    {
        [Test]
        public void Ctor_AssignsVariables_Correctly()
        {
            var id = "broker-id";
            var reddeerId = "reddeer-id";
            var name = "broker-name";
            var createdOn = DateTime.UtcNow;
            var live = true;

            var broker = new OrderBroker(id, reddeerId, name, createdOn, live);

            Assert.AreEqual(id, broker.Id);
            Assert.AreEqual(reddeerId, broker.ReddeerId);
            Assert.AreEqual(name, broker.Name);
            Assert.AreEqual(createdOn, broker.CreatedOn);
            Assert.AreEqual(live, broker.Live);
        }
    }
}
