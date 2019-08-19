namespace Domain.Core.Tests.Trading.Execution
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Tests.Helpers;
    using Domain.Core.Trading.Execution;
    using Domain.Core.Trading.Orders;

    using NUnit.Framework;

    [TestFixture]
    public class OrderAnalysisSentimentServiceTests
    {
        [Test]
        public void OpposingSentiment_HasFullCoverage_OfAllPriceSentiments()
        {
            var service = this.Service();
            var rawValues = Enum.GetValues(typeof(PriceSentiment));

            var order1 = new Order().Random();
            order1.OrderDirection = OrderDirections.BUY;
            var orderAnalysis1 = new OrderAnalysis(order1, PriceSentiment.Positive);
            var order2 = new Order().Random();
            order2.OrderDirection = OrderDirections.SELL;
            var orderAnalysis2 = new OrderAnalysis(order2, PriceSentiment.Negative);
            var orders = new[] { orderAnalysis1, orderAnalysis2 };

            foreach (var val in rawValues)
                Assert.DoesNotThrow(() => service.OpposingSentiment(orders, (PriceSentiment)val));
        }

        [Test]
        public void ResolveSentiment_BuyOrder_IsPositive()
        {
            var service = this.Service();
            var order = new Order().Random();
            order.OrderDirection = OrderDirections.BUY;

            var sentiment = service.ResolveSentiment(new[] { order });

            Assert.AreEqual(sentiment, PriceSentiment.Positive);
        }

        [Test]
        public void ResolveSentiment_BuyThenSell_IsMixed()
        {
            var service = this.Service();
            var order1 = new Order().Random();
            order1.OrderDirection = OrderDirections.BUY;
            var order2 = new Order().Random();
            order2.OrderDirection = OrderDirections.SELL;

            var sentiment = service.ResolveSentiment(new[] { order1, order2 });

            Assert.AreEqual(sentiment, PriceSentiment.Mixed);
        }

        [Test]
        public void ResolveSentiment_CoverOrder_IsPositive()
        {
            var service = this.Service();
            var order = new Order().Random();
            order.OrderDirection = OrderDirections.COVER;

            var sentiment = service.ResolveSentiment(new[] { order });

            Assert.AreEqual(sentiment, PriceSentiment.Positive);
        }

        [Test]
        public void ResolveSentiment_EmptyOrders_IsNeutral()
        {
            var service = this.Service();

            var sentiment = service.ResolveSentiment(new Order[0]);

            Assert.AreEqual(sentiment, PriceSentiment.Neutral);
        }

        [Test]
        public void ResolveSentiment_NullOrders_IsNeutral()
        {
            var service = this.Service();

            var sentiment = service.ResolveSentiment((List<Order>)null);

            Assert.AreEqual(sentiment, PriceSentiment.Neutral);
        }

        [Test]
        public void ResolveSentiment_SellOrder_IsNegative()
        {
            var service = this.Service();
            var order = new Order().Random();
            order.OrderDirection = OrderDirections.SELL;

            var sentiment = service.ResolveSentiment(new[] { order });

            Assert.AreEqual(sentiment, PriceSentiment.Negative);
        }

        [Test]
        public void ResolveSentiment_ShortOrder_IsNegative()
        {
            var service = this.Service();
            var order = new Order().Random();
            order.OrderDirection = OrderDirections.SHORT;

            var sentiment = service.ResolveSentiment(new[] { order });

            Assert.AreEqual(sentiment, PriceSentiment.Negative);
        }

        private OrderAnalysisService Service()
        {
            return new OrderAnalysisService();
        }
    }
}