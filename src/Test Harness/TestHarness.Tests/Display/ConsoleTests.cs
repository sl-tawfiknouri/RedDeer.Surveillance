using Domain.Equity;
using Domain.Market;
using NUnit.Framework;
using System;
using System.Linq;
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
            var sec = new Security(new Security.SecurityId("STAN"), "Standard Chartered", "LSE");

            return new TradeOrderFrame(
                    OrderType.Limit,
                    stock,
                    sec,
                    new Price(20.2m),
                    100 * vol,
                    OrderDirection.Buy,
                    OrderStatus.Fulfilled,
                    DateTime.Now);
        }
    }
}
