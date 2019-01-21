﻿using System;
using DomainV2.Financial;
using DomainV2.Trading;
using NUnit.Framework;

namespace DomainV2.Tests.Trading
{
    [TestFixture]
    public class OrderAllocationDecoratorTests
    {
        [Test]
        public void Decorator_Handles_Zero_OrderFill()
        {
            var order = BuildOrder();
            order.OrderFilledVolume = 0;
            order.OrderOrderedVolume = 100;

            var orderAllocation = new OrderAllocation("id", "order-1", "allocation-fund", "allocation-strategy", "allocation-account", 100);
            var decorator = new OrderAllocationDecorator(order, orderAllocation);

            Assert.AreEqual(decorator.OrderFilledVolume, 100);
            Assert.AreEqual(decorator.OrderOrderedVolume, 100);
        }

        [Test]
        public void Decorator_Handles_ZeroAllocatedFill()
        {
            var order = BuildOrder();
            order.OrderFilledVolume = 100;

            var orderAllocation = new OrderAllocation("id", "order-1", "allocation-fund", "allocation-strategy", "allocation-account", 0);
            var decorator = new OrderAllocationDecorator(order, orderAllocation);

            Assert.AreEqual(decorator.OrderFilledVolume, 0);
            Assert.AreEqual(decorator.OrderOrderedVolume, 0);
        }

        [Test]
        public void Decorator_Handles_Zero_CorrectFill()
        {
            var order = BuildOrder();
            order.OrderOrderedVolume = 1000;
            order.OrderFilledVolume = 1000;

            var orderAllocation = new OrderAllocation("id", "order-1", "allocation-fund", "allocation-strategy", "allocation-account", 100);
            var decorator = new OrderAllocationDecorator(order, orderAllocation);

            Assert.AreEqual(decorator.OrderFilledVolume, 100);
            Assert.AreEqual(decorator.OrderOrderedVolume, 100);
        }

        [Test]
        public void Decorator_Handles_Zero_CorrectAccountingEntities()
        {
            var order = BuildOrder();
            order.OrderFilledVolume = 1000;

            var orderAllocation = new OrderAllocation("id", "order-1", "allocation-fund", "allocation-strategy", "allocation-account", 100);
            var decorator = new OrderAllocationDecorator(order, orderAllocation);

            Assert.AreEqual(decorator.OrderClientAccountAttributionId, "allocation-account");
            Assert.AreEqual(decorator.OrderFund, "allocation-fund");
            Assert.AreEqual(decorator.OrderStrategy, "allocation-strategy");
        }

        [Test]
        public void Decorator_Handles_OrderVolumeAdjustment()
        {
            var order = BuildOrder();
            order.OrderOrderedVolume = 100;
            order.OrderFilledVolume = 1000;

            var orderAllocation = new OrderAllocation("id", "order-1", "allocation-fund", "allocation-strategy", "allocation-account", 100);
            var decorator = new OrderAllocationDecorator(order, orderAllocation);

            Assert.AreEqual(decorator.OrderOrderedVolume, 10);
        }

        private Order BuildOrder()
        {
            var instrument =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    new InstrumentIdentifiers("1", "1", "1", "1", "abcdefg", "123456789012", "123456789012", "123456", "abc", null, null),
                    "mock-instrument",
                    "entspb",
                    "GBP",
                    "mock-bank");

            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);

            return new Order(
                instrument,
                market,
                null,
                "order-1",
                DateTime.UtcNow,
                "order-v1",
                "order-v1-link",
                "order-group-1",
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow,
                null,
                null,
                DateTime.UtcNow,
                OrderTypes.MARKET,
                OrderDirections.BUY,
                new Currency("GBP"),
                null,
                OrderCleanDirty.NONE,
                null,
                null,
                new CurrencyAmount(100, "GBP"),
                0,
                0,
                "trader-one",
                "trader-1",
                "clearing-agent",
                "deal asap",
                null,
                null,
                OptionEuropeanAmerican.NONE,
                null);
        }
    }
}