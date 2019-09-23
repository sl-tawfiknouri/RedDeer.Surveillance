namespace Domain.Core.Tests.Markets.Timebars
{
    using System;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Markets;
    using Domain.Core.Markets.Timebars;

    using NUnit.Framework;

    [TestFixture]
    public class EquityInstrumentInterDayTimeBarTests
    {
        [Test]
        public void Ctor_AssignsVariables_Correctly()
        {
            var fi = new FinancialInstrument();
            var prices = new IntradayPrices(null, null, null, null);
            var dates = DateTime.UtcNow;

            var dailyTb = new DailySummaryTimeBar(100, "USD", prices, 123, new Volume(123), dates);

            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.DealerBooks);
            var equityTimeBar = new EquityInstrumentInterDayTimeBar(fi, dailyTb, dates, market);

            Assert.AreEqual(fi, equityTimeBar.Security);
            Assert.AreEqual(dailyTb, equityTimeBar.DailySummaryTimeBar);
            Assert.AreEqual(dates, equityTimeBar.TimeStamp);
            Assert.AreEqual(market, equityTimeBar.Market);
        }
    }
}