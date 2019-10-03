namespace Domain.Core.Tests.Trading.Execution
{
    using System;

    using Domain.Core.Trading.Execution;
    using Domain.Core.Trading.Orders;

    using NUnit.Framework;

    using TestHelpers;

    [TestFixture]
    public class OrderAnalysisTests
    {
        [Test]
        public void Ctor_AssignsVariables_Correctly()
        {
            var order = OrderTestHelper.Random(new Order(), 30);
            var orderAnalysis = new OrderAnalysis(order, PriceSentiment.Negative);

            Assert.AreEqual(PriceSentiment.Negative, orderAnalysis.Sentiment);
            Assert.AreEqual(order, orderAnalysis.Order);
        }

        [Test]
        public void Ctor_ThrowsForNull_Order()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderAnalysis(null, PriceSentiment.Mixed));
        }
    }
}