using System;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Assets.Interfaces;
using Domain.Core.Financial.Money;
using Domain.Core.Trading.Orders;
using FakeItEasy;
using NUnit.Framework;

namespace Domain.Tests.Trading
{
    public class DealerOrderTests
    {
        [Test]
        public void MostRecentDateEvent_When_NoDomainDates_Then_ReturnsUtcDate()
        {
            // arrange
            var underTest = new DealerOrder(
                A.Fake<IFinancialInstrument>(),
                string.Empty,
                string.Empty,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                OrderTypes.LIMIT,
                OrderDirections.BUY,
                new Currency("GBP"),
                new Currency("USD"),
                OrderCleanDirty.NONE,
                null,
                string.Empty,
                string.Empty,
                string.Empty,
                null,
                null,
                null,
                null,
                null,
                null,                
                OptionEuropeanAmerican.NONE);

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
        public void MostRecentDateEvent_When_PlacedDateOnly_Then_ReturnsPlacedDate()
        {
            // arrange
            var placedDate = new DateTime(2019, 1, 1, 1, 1, 1);
            var underTest = new DealerOrder(
                A.Fake<IFinancialInstrument>(),
                string.Empty,
                string.Empty,
                placedDate,
                null,
                null,
                null,
                null,
                null,
                null,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                OrderTypes.LIMIT,
                OrderDirections.BUY,
                new Currency("GBP"),
                new Currency("USD"),
                OrderCleanDirty.NONE,
                null,
                string.Empty,
                string.Empty,
                string.Empty,
                null,
                null,
                null,
                null,
                null,
                null,
                OptionEuropeanAmerican.NONE);

            // act
            var actual = underTest.MostRecentDateEvent();
            
            // assert
            Assert.AreEqual(placedDate, actual);
            
        }
    }
}
