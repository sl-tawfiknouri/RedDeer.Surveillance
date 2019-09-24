using Surveillance.Engine.Rules.Rules.Shared.HighProfits.Calculators;

namespace Surveillance.Engine.Rules.Tests.Rules.Equities.HighProfits.Calculators
{
    using System.Collections.Generic;

    using Domain.Core.Financial.Money;
    using Domain.Core.Trading;
    using Domain.Core.Trading.Orders;

    using NUnit.Framework;

    using TestHelpers;

    [TestFixture]
    public class ExchangeRateProfitBreakdownTests
    {
        [Test]
        public void ProfitBreakCalculatesCorrectPercentageDueToWer()
        {
            var costOne = new Order().Random();
            var costTwo = new Order().Random();
            var costPosition = new TradePosition(new List<Order> { costOne, costTwo });

            var revenueOne = new Order().Random();
            var revenueTwo = new Order().Random();
            var revenuePosition = new TradePosition(new List<Order> { revenueOne, revenueTwo });

            var breakdown = new ExchangeRateProfitBreakdown(
                costPosition,
                revenuePosition,
                2,
                1.5m,
                new Currency("USD"),
                new Currency("GBP"));

            var x = breakdown.AbsoluteAmountDueToWer();
            var y = breakdown.RelativePercentageDueToWer();

            Assert.AreEqual(x, -1000);
            Assert.AreEqual(y, -0.25m);
        }
    }
}