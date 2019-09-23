namespace Surveillance.Engine.Rules.Tests.Markets
{
    using System;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.Markets;

    [TestFixture]
    public class TradingHoursTests
    {
        [Test]
        public void ClosingInUtcForDay_Returns_ClosingOnDay_If_Time_ComponentIsAfterMarketOpen()
        {
            var tradingHours = this.Xlon();

            var closing = tradingHours.ClosingInUtcForDay(DateTime.UtcNow.Date.Add(TimeSpan.FromHours(9)));

            Assert.AreEqual(closing, DateTime.UtcNow.Date.AddHours(16));
        }

        [Test]
        public void ClosingInUtcForDay_Returns_ClosingOnDay_If_Time_ComponentIsOnMarketOpen()
        {
            var tradingHours = this.Xlon();

            var closing = tradingHours.ClosingInUtcForDay(DateTime.UtcNow.Date.Add(TimeSpan.FromHours(8)));

            Assert.AreEqual(closing, DateTime.UtcNow.Date.AddHours(16));
        }

        [Test]
        public void ClosingInUtcForDay_Returns_ClosingOnDay_If_Time_ComponentIsThreeHoursBeforeMarketOpen()
        {
            var tradingHours = this.Xlon();

            var closing = tradingHours.ClosingInUtcForDay(DateTime.UtcNow.Date.Add(TimeSpan.FromHours(5)));

            Assert.AreEqual(closing, DateTime.UtcNow.Date.AddHours(16));
        }

        [Test]
        public void ClosingInUtcForDay_Returns_ClosingOnPreviousDay_If_Time_ComponentIsFiveHoursBeforeMarketOpen()
        {
            var tradingHours = this.Xlon();

            var closing = tradingHours.ClosingInUtcForDay(DateTime.UtcNow.Date.Add(TimeSpan.FromHours(3)));

            Assert.AreEqual(closing, DateTime.UtcNow.Date.AddDays(-1).AddHours(16));
        }

        [Test]
        public void ClosingInUtcOrUniverseForDay_Returns_ClosingOnDay_If_Time_IsSetToAfterMarketClose()
        {
            var tradingHours = this.Xlon();

            var closing =
                tradingHours.MinimumOfCloseInUtcForDayOrUniverse(DateTime.UtcNow.Date.Add(TimeSpan.FromHours(18)));

            Assert.AreEqual(closing, DateTime.UtcNow.Date.AddHours(16));
        }

        [Test]
        public void ClosingInUtcOrUniverseForDay_Returns_UniverseTime_If_Time_IsSetToBeforeMarketClose()
        {
            var tradingHours = this.Xlon();

            var closing =
                tradingHours.MinimumOfCloseInUtcForDayOrUniverse(DateTime.UtcNow.Date.Add(TimeSpan.FromHours(10)));

            Assert.AreEqual(closing, DateTime.UtcNow.Date.AddHours(10));
        }

        [Test]
        public void OpeningInUtcForDay_Returns_OpeningOnDay_If_Time_ComponentIsAfterMarketOpen()
        {
            var tradingHours = this.Xlon();

            var opening = tradingHours.OpeningInUtcForDay(DateTime.UtcNow.Date.Add(TimeSpan.FromHours(9)));

            Assert.AreEqual(opening.Date, DateTime.UtcNow.Date);
        }

        [Test]
        public void OpeningInUtcForDay_Returns_OpeningOnDay_If_Time_ComponentIsSameAsMarketOpen()
        {
            var tradingHours = this.Xlon();

            var opening = tradingHours.OpeningInUtcForDay(DateTime.UtcNow.Date.Add(TimeSpan.FromHours(8)));

            Assert.AreEqual(opening.Date, DateTime.UtcNow.Date);
        }

        [Test]
        public void OpeningInUtcForDay_Returns_OpeningOnDay_If_Time_ComponentIsThreeHoursBeforeMarketOpen()
        {
            var tradingHours = this.Xlon();

            var opening = tradingHours.OpeningInUtcForDay(DateTime.UtcNow.Date.Add(TimeSpan.FromHours(5)));

            Assert.AreEqual(opening.Date, DateTime.UtcNow.Date);
        }

        [Test]
        public void OpeningInUtcForDay_Returns_OpeningOnPreviousDay_If_Time_ComponentIsFiveHoursBeforeMarketOpen()
        {
            var tradingHours = this.Xlon();

            var opening = tradingHours.OpeningInUtcForDay(DateTime.UtcNow.Date.Add(TimeSpan.FromHours(3)));

            Assert.AreEqual(opening.Date, DateTime.UtcNow.Date.AddDays(-1));
        }

        private TradingHours Xlon()
        {
            var tradingHours = new TradingHours
                                   {
                                       OpenOffsetInUtc = TimeSpan.FromHours(8),
                                       CloseOffsetInUtc = TimeSpan.FromHours(16),
                                       Mic = "XLON"
                                   };

            return tradingHours;
        }
    }
}