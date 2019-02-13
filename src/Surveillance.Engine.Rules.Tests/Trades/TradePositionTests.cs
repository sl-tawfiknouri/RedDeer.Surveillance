using System;
using System.Collections.Generic;
using DomainV2.Financial;
using DomainV2.Trading;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Surveillance.Tests.Trades
{
    [TestFixture]
    public class TradePositionTests
    {
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = A.Fake<ILogger>();
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

            var tradePosition = new TradePositionCancellations(tof, null, 0.3m, _logger);

            var result = tradePosition.CancellationRatioByTradeCount();

            Assert.AreEqual(result, 0.2m);
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

            var tradePosition = new TradePositionCancellations(tof, null, 0.3m, _logger);

            var result = tradePosition.HighCancellationRatioByTradeCount();

            Assert.AreEqual(result, false);
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

            var tradePosition = new TradePositionCancellations(tof, null, 0.3m, _logger);

            var result = tradePosition.HighCancellationRatioByTradeCount();

            Assert.AreEqual(result, true);
        }

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

            var tradePosition = new TradePositionCancellations(tof, 0.8m, null, _logger);

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

            var tradePosition = new TradePositionCancellations(tof, 0.8m, null, _logger);

            var result = tradePosition.HighCancellationRatioByPositionSize();

            Assert.AreEqual(result, false);
        }

        private Order TradeFrame(OrderStatus status)
        {
            var securityIdentifiers =
                new InstrumentIdentifiers(
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

            var security =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    securityIdentifiers,
                    "Test Security",
                    "CFI",
                    "USD",
                    "Issuer Identifier");

            var cancelledDate = status == OrderStatus.Cancelled ? (DateTime?)DateTime.UtcNow : null;
            var filledDate = status == OrderStatus.Filled ? (DateTime?) DateTime.UtcNow : null;

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
                new DomainV2.Financial.Currency("GBP"), 
                new DomainV2.Financial.Currency("GBP"),
                OrderCleanDirty.NONE,
                null,
                new CurrencyAmount(1000, "GBP"),
                new CurrencyAmount(1000, "GBP"),
                1000,
                1000,
                "Trader - 1",
                "trader one",
                "Rybank Long",
                "deal-asap",
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new DealerOrder[0]);
        }
    }
}