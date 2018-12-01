using Domain.Equity;
using Domain.Market;
using NUnit.Framework;
using System;
using System.Linq;
using Domain.Finance;
using Domain.Trades.Orders;

namespace TestHarness.Tests.Display
{
    [TestFixture]
    public class ConsoleTests
    {
        [Test]
        [Explicit]
        [Description("Used to visually confirm output for display in test output")]
        public void OutputTradeFrame_OutputsAllFive_PushedOrders()
        {
            var con = new TestHarness.Display.Console();

            var frames = Enumerable.Range(0, 20).Select(GenerateFrame).ToList();

            foreach (var frame in frames)
            {
                con.OutputTradeFrame(frame);
            }

            Assert.IsTrue(true);
        }

        private TradeOrderFrame GenerateFrame(int vol)
        {
            var stock = new StockExchange(new Market.MarketId("LSE"), "London Stock Exchange");
            var securityIdentifiers =
                new SecurityIdentifiers(
                    string.Empty,
                    "STAN",
                    "STAN",
                    "st12345",
                    "sta123456789",
                    "stan",
                    "sta12345",
                    "stan",
                    "stan lei",
                    "stan");

            var sec = new Security(
                securityIdentifiers,
                "Standard Chartered",
                "CFI",
                "Standard Chartered Bank");

            return new TradeOrderFrame(
                    null,
                    OrderType.Limit,
                    stock,
                    sec,
                    new CurrencyAmount(20.2m, "GBP"),
                    new CurrencyAmount(20.2m, "GBP"),
                    100 * vol,
                    100 * vol,
                    OrderPosition.Buy,
                    OrderStatus.Fulfilled,
                    DateTime.Now,
                    DateTime.Now,
                    "trader-1",
                    "",
                    "account-1",
                    "test",
                    "party-broker",
                    "counterParty-broker",
                    "none",
                    "unknown",
                    "GBX");
        }
    }
}
