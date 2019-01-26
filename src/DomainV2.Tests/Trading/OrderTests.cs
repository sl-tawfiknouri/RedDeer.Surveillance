using System;
using DomainV2.Trading;
using NUnit.Framework;
using FakeItEasy;
using DomainV2.Financial;

namespace DomainV2.Tests.Trading
{
    public class OrderTests
    {
        [Test]
        public void MostRecentDateEvent_When_NoDomainDates_Then_ReturnsUtcDate()
        {
            // arrange
            var underTest = new Order(
                A.Fake<FinancialInstrument>(),
                A.Fake<Market>(),
                null,
                string.Empty,
                null,
                string.Empty,
                string.Empty,
                null,
                null,
                null,
                null,
                null,
                null,
                null,               
                OrderTypes.LIMIT,
                OrderDirections.BUY,
                new Currency("GBP"),
                new Currency("USD"),
                OrderCleanDirty.NONE,
                null,
                null,
                null, 
                null,
                null,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                null,
                null,
                OptionEuropeanAmerican.NONE,
                null);

            // act
            DateTime before = DateTime.UtcNow;
            var actual = underTest.MostRecentDateEvent();
            DateTime after = DateTime.UtcNow;

            // assert
            Assert.True(before <= actual); // allows for time to pass in test execution
            Assert.True(after >= actual);

            Assert.AreEqual(DateTimeKind.Utc, actual.Kind);
        }

        [Test]
        public void MostRecentDateEvent_When_BookedDateOnly_Then_ReturnsBookedDate()
        {
            // arrange
            var bookedDate = new DateTime(2019, 1, 1, 1, 1, 1);
            var underTest = new Order(
                A.Fake<FinancialInstrument>(),
                A.Fake<Market>(),
                null,
                string.Empty,
                null,
                string.Empty,
                string.Empty,
                null,
                null,
                bookedDate,
                null,
                null,
                null,
                null,
                OrderTypes.LIMIT,
                OrderDirections.BUY,
                new Currency("GBP"),
                new Currency("USD"),
                OrderCleanDirty.NONE,
                null,
                null,
                null,
                null,
                null,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                null,
                null,
                OptionEuropeanAmerican.NONE,
                null);

            // act
            var actual = underTest.MostRecentDateEvent();
            
            // assert
            Assert.AreEqual(bookedDate, actual);
            
        }
    }
}
