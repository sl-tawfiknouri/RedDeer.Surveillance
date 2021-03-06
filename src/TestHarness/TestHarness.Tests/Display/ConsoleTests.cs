﻿namespace TestHarness.Tests.Display
{
    using System;
    using System.Linq;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets;
    using Domain.Core.Trading.Orders;

    using NUnit.Framework;

    using Console = TestHarness.Display.Console;

    [TestFixture]
    public class ConsoleTests
    {
        [Test]
        [Explicit]
        [Description("Used to visually confirm output for display in test output")]
        public void OutputTradeFrame_OutputsAllFive_PushedOrders()
        {
            var con = new Console();

            var frames = Enumerable.Range(0, 20).Select(this.GenerateFrame).ToList();

            foreach (var frame in frames) con.OutputTradeFrame(frame);

            Assert.IsTrue(true);
        }

        private Order GenerateFrame(int vol)
        {
            var stock = new Market("1", "LSE", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
            var securityIdentifiers = new InstrumentIdentifiers(
                string.Empty,
                "STAN",
                null,
                "STAN",
                "st12345",
                "sta123456789",
                "stan",
                "sta12345",
                "stan",
                "stan lei",
                "stan",
                "stanRic");

            var sec = new FinancialInstrument(
                InstrumentTypes.Equity,
                securityIdentifiers,
                "Standard Chartered",
                "CFI",
                "USD",
                "Standard Chartered Bank");

            var order = new Order(
                sec,
                stock,
                null,
                Guid.NewGuid().ToString(),
                DateTime.UtcNow,
                "version-1",
                "version-1",
                "version-1",
                DateTime.UtcNow,
                DateTime.UtcNow,
                null,
                null,
                null,
                DateTime.UtcNow,
                OrderTypes.MARKET,
                OrderDirections.BUY,
                new Currency("GBP"),
                new Currency("GBP"),
                OrderCleanDirty.NONE,
                null,
                new Money(20.2m, "GBP"),
                new Money(20.2m, "GBP"),
                100 * vol,
                100 * vol,
                "trader-1",
                "trader one",
                "clearing-bank",
                "deal asap",
                new OrderBroker(string.Empty, string.Empty, "Mr Broker", DateTime.UtcNow, true),
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new DealerOrder[0]);

            return order;
        }
    }
}