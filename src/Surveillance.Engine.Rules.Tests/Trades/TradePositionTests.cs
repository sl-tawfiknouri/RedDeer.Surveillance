namespace Surveillance.Engine.Rules.Tests.Trades
{
    using System;
    using System.Collections.Generic;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets;
    using Domain.Core.Trading.Orders;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Surveillance.Engine.Rules.Trades;

    [TestFixture]
    public class TradePositionTests
    {
        private ILogger _logger;

        [Test]
        public void CancellationRatioByPositionSizeIsHigh_ReturnsExpected()
        {
            var bigPosition = OrderHelper.Orders(OrderStatus.Cancelled);
            bigPosition.OrderFilledVolume = bigPosition.OrderFilledVolume * 100;
            bigPosition.OrderOrderedVolume = bigPosition.OrderFilledVolume * 100;

            var tof = new List<Order>
                          {
                              bigPosition,
                              OrderHelper.Orders(OrderStatus.Filled),
                              OrderHelper.Orders(OrderStatus.Filled),
                              OrderHelper.Orders(OrderStatus.Filled),
                              OrderHelper.Orders(OrderStatus.Filled)
                          };

            var tradePosition = new TradePositionCancellations(tof, 0.8m, null, this._logger);

            var result = tradePosition.HighCancellationRatioByPositionSize();

            Assert.AreEqual(result, true);
        }

        [Test]
        public void CancellationRatioByPositionSizeNotHigh_ReturnsExpected()
        {
            var bigPosition = OrderHelper.Orders(OrderStatus.Filled);
            bigPosition.OrderFilledVolume = bigPosition.OrderFilledVolume * 100;

            var tof = new List<Order>
                          {
                              bigPosition,
                              OrderHelper.Orders(OrderStatus.Cancelled),
                              OrderHelper.Orders(OrderStatus.Cancelled),
                              OrderHelper.Orders(OrderStatus.Filled),
                              OrderHelper.Orders(OrderStatus.Filled)
                          };

            var tradePosition = new TradePositionCancellations(tof, 0.8m, null, this._logger);

            var result = tradePosition.HighCancellationRatioByPositionSize();

            Assert.AreEqual(result, false);
        }

        [Test]
        public void CancellationRatioByTradeCount_ReturnsExpected()
        {
            var tof = new List<Order>
                          {
                              OrderHelper.Orders(OrderStatus.Cancelled),
                              OrderHelper.Orders(OrderStatus.Filled),
                              OrderHelper.Orders(OrderStatus.Filled),
                              OrderHelper.Orders(OrderStatus.Filled),
                              OrderHelper.Orders(OrderStatus.Filled)
                          };

            var tradePosition = new TradePositionCancellations(tof, null, 0.3m, this._logger);

            var result = tradePosition.CancellationRatioByTradeCount();

            Assert.AreEqual(result, 0.2m);
        }

        [Test]
        public void CancellationRatioByTradeCountIsHigh_ReturnsExpected()
        {
            var tof = new List<Order>
                          {
                              OrderHelper.Orders(OrderStatus.Cancelled),
                              OrderHelper.Orders(OrderStatus.Cancelled),
                              OrderHelper.Orders(OrderStatus.Filled),
                              OrderHelper.Orders(OrderStatus.Filled),
                              OrderHelper.Orders(OrderStatus.Filled)
                          };

            var tradePosition = new TradePositionCancellations(tof, null, 0.3m, this._logger);

            var result = tradePosition.HighCancellationRatioByTradeCount();

            Assert.AreEqual(result, true);
        }

        [Test]
        public void CancellationRatioByTradeCountNotHigh_ReturnsExpected()
        {
            var tof = new List<Order>
                          {
                              OrderHelper.Orders(OrderStatus.Cancelled),
                              OrderHelper.Orders(OrderStatus.Filled),
                              OrderHelper.Orders(OrderStatus.Filled),
                              OrderHelper.Orders(OrderStatus.Filled),
                              OrderHelper.Orders(OrderStatus.Filled)
                          };

            var tradePosition = new TradePositionCancellations(tof, null, 0.3m, this._logger);

            var result = tradePosition.HighCancellationRatioByTradeCount();

            Assert.AreEqual(result, false);
        }

        [SetUp]
        public void Setup()
        {
            this._logger = A.Fake<ILogger>();
        }

        private Order TradeFrame(OrderStatus status)
        {
            var securityIdentifiers = new InstrumentIdentifiers(
                string.Empty,
                "reddeer id",
                null,
                "client id",
                "1234567",
                "12345678912",
                "figi",
                "cusip",
                "test",
                "test lei",
                "ticker");

            var security = new FinancialInstrument(
                InstrumentTypes.Equity,
                securityIdentifiers,
                "Test Security",
                "CFI",
                "USD",
                "Issuer Identifier");

            var cancelledDate = status == OrderStatus.Cancelled ? (DateTime?)DateTime.UtcNow : null;
            var filledDate = status == OrderStatus.Filled ? (DateTime?)DateTime.UtcNow : null;

            return new Order(
                security,
                new Market("1", "XLON", "XLON", MarketTypes.STOCKEXCHANGE),
                null,
                "id1",
                DateTime.UtcNow,
                "version-1",
                "version-1",
                "version-1",
                DateTime.UtcNow,
                null,
                null,
                null,
                cancelledDate,
                filledDate,
                OrderTypes.MARKET,
                OrderDirections.BUY,
                new Currency("GBP"),
                new Currency("GBP"),
                OrderCleanDirty.NONE,
                null,
                new Money(1000, "GBP"),
                new Money(1000, "GBP"),
                1000,
                1000,
                "Trader - 1",
                "trader one",
                "Rybank Long",
                "deal-asap",
                new OrderBroker(string.Empty, string.Empty, "Mr Broker", DateTime.Now, true),
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new DealerOrder[0]);
        }
    }
}