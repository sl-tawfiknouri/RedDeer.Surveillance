namespace Domain.Core.Tests.Trading.Orders
{
    using System;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets;
    using Domain.Core.Trading.Orders;

    using FakeItEasy;

    using NUnit.Framework;

    public class OrderTests
    {
        [Test]
        public void Ctor_AssignsVariables_Correctly()
        {
            var placedDate = new DateTime(2019, 1, 1, 1, 1, 1);
            var bookedDate = new DateTime(2019, 1, 1, 1, 1, 1);
            var amendedDate = new DateTime(2019, 1, 1, 1, 1, 1);
            var rejectedDate = new DateTime(2019, 1, 1, 1, 1, 1);
            var cancelledDate = new DateTime(2019, 1, 1, 1, 1, 1);
            var filledDate = new DateTime(2019, 1, 1, 1, 1, 1);

            var fi = new FinancialInstrument();
            var market = new Market("market-id", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
            var reddeerOrderId = 124;
            var orderId = "order-id";
            var createdDate = new DateTime(2019, 09, 19, 1, 1, 1);
            var orderVersion = "version-1";
            var orderVersionLinkId = "version-link-id-1";
            var orderGroupId = "order-group-id";
            var orderTypes = OrderTypes.MARKET;
            var orderDirection = OrderDirections.SHORT;
            var currency = new Currency("GBP");
            var settlementCurrency = new Currency("USD");
            var cleanDirty = OrderCleanDirty.DIRTY;
            var accumInterest = 1241.3m;
            var limitPrice = new Money(99, "GBP");
            var averageFillPrice = new Money(101, "GBP");
            var orderedVolume = 1234m;
            var filledVolume = 4321m;
            var traderId = "abcdef";
            var traderName = "Mr Trader";
            var clearingAgent = "Goldman";
            var dealingInstruction = "ASAP";
            var broker = new OrderBroker("broker-1", "2", "Brokerage Inc", DateTime.UtcNow, true);
            var optionStrikePrice = new Money(30, "USD");
            var optionExpirationDate = new DateTime(2019, 08, 19, 1, 1, 1);
            var optionEurAmer = OptionEuropeanAmerican.AMERICAN;
            var trades = new DealerOrder[0];

            var order = new Order(
                fi,
                market,
                reddeerOrderId,
                orderId,
                createdDate,
                orderVersion,
                orderVersionLinkId,
                orderGroupId,
                placedDate,
                bookedDate,
                amendedDate,
                rejectedDate,
                cancelledDate,
                filledDate,
                orderTypes,
                orderDirection,
                currency,
                settlementCurrency,
                cleanDirty,
                accumInterest,
                limitPrice,
                averageFillPrice,
                orderedVolume,
                filledVolume,
                traderId,
                traderName,
                clearingAgent,
                dealingInstruction,
                broker,
                optionStrikePrice,
                optionExpirationDate,
                optionEurAmer,
                trades);

            Assert.AreEqual(fi, order.Instrument);
            Assert.AreEqual(market, order.Market);
            Assert.AreEqual(reddeerOrderId, order.ReddeerOrderId);
            Assert.AreEqual(orderId, order.OrderId);
            Assert.AreEqual(orderVersion, order.OrderVersion);
            Assert.AreEqual(orderVersionLinkId, order.OrderVersionLinkId);
            Assert.AreEqual(orderGroupId, order.OrderGroupId);
            Assert.AreEqual(optionStrikePrice, order.OrderOptionStrikePrice);
            Assert.AreEqual(optionExpirationDate, order.OrderOptionExpirationDate);
            Assert.AreEqual(optionEurAmer, order.OrderOptionEuropeanAmerican);
            Assert.AreEqual(createdDate, order.CreatedDate);
            Assert.AreEqual(orderTypes, order.OrderType);
            Assert.AreEqual(orderDirection, order.OrderDirection);
            Assert.AreEqual(currency, order.OrderCurrency);
            Assert.AreEqual(settlementCurrency, order.OrderSettlementCurrency);
            Assert.AreEqual(cleanDirty, order.OrderCleanDirty);
            Assert.AreEqual(accumInterest, order.OrderAccumulatedInterest);
            Assert.AreEqual(limitPrice, order.OrderLimitPrice);
            Assert.AreEqual(averageFillPrice, order.OrderAverageFillPrice);
            Assert.AreEqual(traderId, order.OrderTraderId);
            Assert.AreEqual(traderName, order.OrderTraderName);
            Assert.AreEqual(clearingAgent, order.OrderClearingAgent);
            Assert.AreEqual(dealingInstruction, order.OrderDealingInstructions);
            Assert.AreEqual(broker, order.OrderBroker);
            Assert.AreEqual(trades, order.DealerOrders);
            Assert.AreEqual(orderedVolume, order.OrderOrderedVolume);
            Assert.AreEqual(filledVolume, order.OrderFilledVolume);
            Assert.AreEqual(string.Empty, order.OrderFund);
            Assert.AreEqual(string.Empty, order.OrderStrategy);
            Assert.AreEqual(string.Empty, order.OrderClientAccountAttributionId);
            Assert.AreEqual(false, order.IsInputBatch);
            Assert.AreEqual(0, order.BatchSize);
            Assert.AreEqual(null, order.InputBatchId);
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
                new OrderBroker(string.Empty, string.Empty, "Mr Broker", DateTime.Now, true),
                null,
                null,
                OptionEuropeanAmerican.NONE,
                null);

            // act
            var actual = underTest.MostRecentDateEvent();

            // assert
            Assert.AreEqual(bookedDate, actual);
        }

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
                new OrderBroker(string.Empty, string.Empty, "Mr Broker", DateTime.Now, true),
                null,
                null,
                OptionEuropeanAmerican.NONE,
                null);

            // act
            var before = DateTime.UtcNow;
            var actual = underTest.MostRecentDateEvent();
            var after = DateTime.UtcNow;

            // assert
            Assert.True(before <= actual); // allows for time to pass in test execution
            Assert.True(after >= actual);

            Assert.AreEqual(DateTimeKind.Utc, actual.Kind);
        }

        [Test]
        public void ToString_YieldsExpected_Value()
        {
            var placedDate = new DateTime(2019, 1, 1, 1, 1, 1);
            var bookedDate = new DateTime(2019, 1, 1, 1, 1, 1);
            var amendedDate = new DateTime(2019, 1, 1, 1, 1, 1);
            var rejectedDate = new DateTime(2019, 1, 1, 1, 1, 1);
            var cancelledDate = new DateTime(2019, 1, 1, 1, 1, 1);
            var filledDate = new DateTime(2019, 1, 1, 1, 1, 1);

            var fi = new FinancialInstrument();
            var market = new Market("market-id", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
            var reddeerOrderId = 124;
            var orderId = "order-id";
            var createdDate = new DateTime(2019, 09, 19, 1, 1, 1);
            var orderVersion = "version-1";
            var orderVersionLinkId = "version-link-id-1";
            var orderGroupId = "order-group-id";
            var orderTypes = OrderTypes.MARKET;
            var orderDirection = OrderDirections.SHORT;
            var currency = new Currency("GBP");
            var settlementCurrency = new Currency("USD");
            var cleanDirty = OrderCleanDirty.DIRTY;
            var accumInterest = 1241.3m;
            var limitPrice = new Money(99, "GBP");
            var averageFillPrice = new Money(101, "GBP");
            var orderedVolume = 1234m;
            var filledVolume = 4321m;
            var traderId = "abcdef";
            var traderName = "Mr Trader";
            var clearingAgent = "Goldman";
            var dealingInstruction = "ASAP";
            var broker = new OrderBroker("broker-1", "2", "Brokerage Inc", DateTime.UtcNow, true);
            var optionStrikePrice = new Money(30, "USD");
            var optionExpirationDate = new DateTime(2019, 08, 19, 1, 1, 1);
            var optionEurAmer = OptionEuropeanAmerican.AMERICAN;
            var trades = new DealerOrder[0];

            var order = new Order(
                fi,
                market,
                reddeerOrderId,
                orderId,
                createdDate,
                orderVersion,
                orderVersionLinkId,
                orderGroupId,
                placedDate,
                bookedDate,
                amendedDate,
                rejectedDate,
                cancelledDate,
                filledDate,
                orderTypes,
                orderDirection,
                currency,
                settlementCurrency,
                cleanDirty,
                accumInterest,
                limitPrice,
                averageFillPrice,
                orderedVolume,
                filledVolume,
                traderId,
                traderName,
                clearingAgent,
                dealingInstruction,
                broker,
                optionStrikePrice,
                optionExpirationDate,
                optionEurAmer,
                trades);

            var result = order.ToString();

            Assert.AreEqual(" |London Stock Exchange | Cancelled | ordered-1234 | filled-4321 | (GBP) 101", result);
        }
    }
}