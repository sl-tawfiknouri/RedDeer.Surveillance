using System.Collections.Generic;
using Domain.Trades.Orders;
using NUnit.Framework;
using Surveillance.Rules.HighProfits.Calculators;
using Surveillance.Tests.Helpers;
using Surveillance.Trades;

namespace Surveillance.Tests.Rules.HighProfits.Calculators
{
    [TestFixture]
    public class ExchangeRateProfitBreakdownTests
    {
        [Test]
        public void ProfitBreakCalculatesCorrectPercentageDueToWer()
        {
            var costOne = (new TradeOrderFrame()).Random();
            var costTwo = (new TradeOrderFrame()).Random();
            var costPosition = new TradePosition(new List<TradeOrderFrame> { costOne, costTwo });

            var revenueOne = (new TradeOrderFrame()).Random();
            var revenueTwo = (new TradeOrderFrame()).Random();
            var revenuePosition = new TradePosition(new List<TradeOrderFrame> { revenueOne, revenueTwo });

            var breakdown = new ExchangeRateProfitBreakdown(costPosition, revenuePosition, 2, 1.5m);

            var x = breakdown.AbsoluteAmountDueToWer();
            var y = breakdown.RelativePercentageDueToWer();

            Assert.AreEqual(x, -1000);
            Assert.AreEqual(y, -0.25m);
        }
    }
}
