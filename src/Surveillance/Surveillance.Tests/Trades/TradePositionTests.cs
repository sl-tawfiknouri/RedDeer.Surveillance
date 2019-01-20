using System;
using System.Collections.Generic;
using DomainV2.Financial;
using DomainV2.Trading;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Trades;

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
    }
}