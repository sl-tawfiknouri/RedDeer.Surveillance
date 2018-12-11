using System;
using DomainV2.Financial;
using DomainV2.Trading;

namespace TestHarness.Tests.Helpers
{
    public static class OrderHelper
    {
        public static Order GetOrder(int? vol = 1000)
        {
            var stock = new Market("1", "LSE", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);
            var securityIdentifiers =
                new InstrumentIdentifiers(
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

            var sec = new FinancialInstrument(
                InstrumentTypes.Equity,
                securityIdentifiers,
                "Standard Chartered",
                "CFI",
                "Standard Chartered Bank");

            var order = new Order(
                sec,
                stock,
                null,
                Guid.NewGuid().ToString(),
                DateTime.UtcNow,
                DateTime.UtcNow,
                null,
                null,
                null,
                DateTime.UtcNow,
                OrderTypes.MARKET,
                OrderPositions.BUY,
                new Currency("GBP"),
                new CurrencyAmount(20.2m, "GBP"),
                new CurrencyAmount(20.2m, "GBP"),
                100 * vol,
                100 * vol,
                "portfolio-manager",
                "trader-1",
                "executing-broker",
                "clearing-bank",
                "deal asap",
                "long/short equities",
                "low liquidity",
                "fast-fund",
                "acc-1",
                new Trade[0]);

            return order;
        }
    }
}
