using System;
using Domain.Core.Financial;
using Domain.Core.Financial.Assets;
using Domain.Core.Financial.Money;
using Domain.Core.Markets;
using Domain.Core.Trading.Orders;

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
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new DealerOrder[0]);

            return order;
        }
    }
}
