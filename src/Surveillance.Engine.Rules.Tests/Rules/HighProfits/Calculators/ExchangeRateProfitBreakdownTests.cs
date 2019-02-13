using System.Collections.Generic;
using DomainV2.Trading;
using NUnit.Framework;
using Surveillance.Tests.Helpers;

namespace Surveillance.Tests.Rules.HighProfits.Calculators
{
    [TestFixture]
    public class ExchangeRateProfitBreakdownTests
    {
        [Test]
        public void ProfitBreakCalculatesCorrectPercentageDueToWer()
        {
            var costOne = (new Order()).Random();
            var costTwo = (new Order()).Random();
            var costPosition = new TradePosition(new List<Order> { costOne, costTwo });

            var revenueOne = (new Order()).Random();
            var revenueTwo = (new Order()).Random();
            var revenuePosition = new TradePosition(new List<Order> { revenueOne, revenueTwo });

            var breakdown = new ExchangeRateProfitBreakdown(
                costPosition,
                revenuePosition,
                2, 
                1.5m,
                new DomainV2.Financial.Currency("USD"),
                new DomainV2.Financial.Currency("GBP"));

            var x = breakdown.AbsoluteAmountDueToWer();
            var y = breakdown.RelativePercentageDueToWer();

            Assert.AreEqual(x, -1000);
            Assert.AreEqual(y, -0.25m);
        }
    }
}
