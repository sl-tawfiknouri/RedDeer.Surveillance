﻿using System;
using System.Collections.Generic;
using Domain.Equity;
using Domain.Market;
using Domain.Trades.Orders;
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
            var tof = new List<TradeOrderFrame>
            {
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled)
            };

            var tradePosition = new TradePosition(tof, null, 0.3m, _logger);

            var result = tradePosition.CancellationRatioByTradeCount();

            Assert.AreEqual(result, 0.2m);
        }

        [Test]
        public void CancellationRatioByTradeCountNotHigh_ReturnsExpected()
        {
            var tof = new List<TradeOrderFrame>
            {
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled)
            };

            var tradePosition = new TradePosition(tof, null, 0.3m, _logger);

            var result = tradePosition.HighCancellationRatioByTradeCount();

            Assert.AreEqual(result, false);
        }

        [Test]
        public void CancellationRatioByTradeCountIsHigh_ReturnsExpected()
        {
            var tof = new List<TradeOrderFrame>
            {
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled)
            };

            var tradePosition = new TradePosition(tof, null, 0.3m, _logger);

            var result = tradePosition.HighCancellationRatioByTradeCount();

            Assert.AreEqual(result, true);
        }

        [Test]
        public void CancellationRatioByPositionSizeIsHigh_ReturnsExpected()
        {
            var bigPosition = TradeFrame(OrderStatus.Cancelled);
            bigPosition.Volume = bigPosition.Volume * 100;

            var tof = new List<TradeOrderFrame>
            {
                bigPosition,
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled)
            };

            var tradePosition = new TradePosition(tof, 0.8m, null, _logger);

            var result = tradePosition.HighCancellationRatioByPositionSize();

            Assert.AreEqual(result, true);
        }

        [Test]
        public void CancellationRatioByPositionSizeNotHigh_ReturnsExpected()
        {
            var bigPosition = TradeFrame(OrderStatus.Fulfilled);
            bigPosition.Volume = bigPosition.Volume * 100;

            var tof = new List<TradeOrderFrame>
            {
                bigPosition,
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Cancelled),
                TradeFrame(OrderStatus.Fulfilled),
                TradeFrame(OrderStatus.Fulfilled)
            };

            var tradePosition = new TradePosition(tof, 0.8m, null, _logger);

            var result = tradePosition.HighCancellationRatioByPositionSize();

            Assert.AreEqual(result, false);
        }


        private TradeOrderFrame TradeFrame(OrderStatus status)
        {
            return new TradeOrderFrame(
                OrderType.Market,
                new StockExchange(new Market.MarketId("XLON"), "XLON"),
                new Security(
                    new SecurityIdentifiers("client id", "1234567", "12345678912", "figi", "cusip", "test"),
                    "Test Security",
                    "CFI"),
                null,
                1000,
                OrderPosition.BuyLong,
                status,
                DateTime.Now,
                DateTime.Now,
                "trader-1",
                "client-attribution-id",
                "party-broker",
                "counter party");
        }
    }
}
