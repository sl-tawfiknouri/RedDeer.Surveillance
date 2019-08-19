using System;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Assets.Interfaces;
using Domain.Core.Financial.Money;
using Domain.Core.Trading.Orders;
using FakeItEasy;
using NUnit.Framework;

namespace Domain.Core.Tests.Trading.Orders
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

        /// <summary>
        /// This is a DDD value object
        /// </summary>
        [Test]
        public void Ctor_AssignsVariables_Correctly()
        {
            var placedDate = new DateTime(2019, 1, 1, 1, 1, 1);
            var bookedDate = new DateTime(2019, 2, 1, 1, 1, 1);
            var amendedDate = new DateTime(2019, 3, 1, 1, 1, 1);
            var rejectedDate = new DateTime(2019, 4, 1, 1, 1, 1);
            var cancelledDate = new DateTime(2019, 5, 1, 1, 1, 1);
            var filledDate = new DateTime(2019, 6, 1, 1, 1, 1);
            var createdDate = new DateTime(2019, 7, 1, 1, 1, 1);

            var fi = new FinancialInstrument();
            var redOrderId = "red-order-id";
            var orderId = "order-id";

            var traderId = "trader-id";
            var dealerName = "mr.dealer";
            var notes = "my notes";
            var tradeCounterParty = "trade counter party";

            var orderType = OrderTypes.LIMIT;
            var orderDirection = OrderDirections.SHORT;
            var currency = new Currency("GBP");
            var settlementCurrency = new Currency("USD");

            var cleanDirty = OrderCleanDirty.DIRTY;
            var accumInterest = 1231.12312m;
            var dealerOrderVersion = "do-version-1";
            var dealerOrderVersionLink = "do-version-1-link-id";
            var dealerOrderGroupId = "group-90";
            var limitPrice = new Money(100, "GBP");
            var averageFillPrice = new Money(99, "USD");
            var orderedVolume = 1002;
            var filledVolume = 1000;
            var optionStrikePrice = 99.9m;
            var optionExpirationDate = new DateTime(2019, 08, 1, 1, 1, 1);
            var optionEuropeanAmerican = OptionEuropeanAmerican.AMERICAN;

            var dO = new DealerOrder(
                fi,
                redOrderId,
                orderId,
                placedDate,
                bookedDate,
                amendedDate,
                rejectedDate,
                cancelledDate,
                filledDate,
                createdDate,
                traderId,
                dealerName,
                notes,
                tradeCounterParty,
                orderType,
                orderDirection,
                currency,
                settlementCurrency,
                cleanDirty,
                accumInterest,
                dealerOrderVersion,
                dealerOrderVersionLink,
                dealerOrderGroupId,
                limitPrice,
                averageFillPrice,
                orderedVolume,
                filledVolume,
                optionStrikePrice,
                optionExpirationDate,
                optionEuropeanAmerican);

            Assert.AreEqual(fi, dO.Instrument);
            Assert.AreEqual(redOrderId, dO.ReddeerDealerOrderId);
            Assert.AreEqual(orderId, dO.DealerOrderId);
            Assert.AreEqual(createdDate, dO.CreatedDate);
            Assert.AreEqual(traderId, dO.DealerId);
            Assert.AreEqual(dealerName, dO.DealerName);
            Assert.AreEqual(notes, dO.Notes);
            Assert.AreEqual(tradeCounterParty, dO.DealerCounterParty);
            Assert.AreEqual(orderType, dO.OrderType);
            Assert.AreEqual(orderDirection, dO.OrderDirection);
            Assert.AreEqual(currency, dO.Currency);
            Assert.AreEqual(settlementCurrency, dO.SettlementCurrency);
            Assert.AreEqual(cleanDirty, dO.CleanDirty);
            Assert.AreEqual(accumInterest, dO.AccumulatedInterest);
            Assert.AreEqual(dealerOrderVersion, dO.DealerOrderVersion);
            Assert.AreEqual(dealerOrderVersionLink, dO.DealerOrderVersionLinkId);
            Assert.AreEqual(dealerOrderGroupId, dO.DealerOrderGroupId);
            Assert.AreEqual(limitPrice, dO.LimitPrice);
            Assert.AreEqual(averageFillPrice, dO.AverageFillPrice);
            Assert.AreEqual(orderedVolume, dO.OrderedVolume);
            Assert.AreEqual(filledVolume, dO.FilledVolume);
            Assert.AreEqual(optionStrikePrice, dO.OptionStrikePrice);
            Assert.AreEqual(optionExpirationDate, dO.OptionExpirationDate);
            Assert.AreEqual(optionEuropeanAmerican, dO.OptionEuropeanAmerican);
        }
    }
}
