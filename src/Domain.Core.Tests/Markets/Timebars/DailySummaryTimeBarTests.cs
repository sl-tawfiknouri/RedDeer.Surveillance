namespace Domain.Core.Tests.Markets.Timebars
{
    using System;

    using Domain.Core.Markets.Timebars;

    using NUnit.Framework;

    [TestFixture]
    public class DailySummaryTimeBarTests
    {
        [Test]
        public void Ctor_AssignsVariables_Correctly()
        {
            var date = DateTime.UtcNow;

            var timeBar = new DailySummaryTimeBar(
                100,
                "USD",
                new IntradayPrices(null, null, null, null),
                123,
                new Volume(101),
                date);

            Assert.AreEqual(100, timeBar.MarketCap.Value.Value);
            Assert.AreEqual("USD", timeBar.MarketCap.Value.Currency.Code);
            Assert.AreEqual(101, timeBar.DailyVolume.Traded);
            Assert.AreEqual(123, timeBar.ListedSecurities);
            Assert.AreEqual(date, timeBar.TimeStamp);
        }
    }
}