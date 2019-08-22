namespace TestHelpers
{
    using System;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets;
    using Domain.Core.Trading.Orders;

    public static class OrderTestHelper
    {
        public static Order Random(this Order frame, int? price = 20)
        {
            var fi = new FinancialInstrument(
                InstrumentTypes.Equity,
                new InstrumentIdentifiers(
                    string.Empty,
                    "reddeer-id",
                    null,
                    "client-identifier",
                    "sedol",
                    "isin",
                    "figi",
                    "cusip",
                    "xlon",
                    "lei",
                    "bloomberg"),
                "random-security",
                "ENTSPB",
                "USD",
                "Random Inc");

            var market = new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE);

            return new Order(
                fi,
                market,
                null,
                "order-1",
                DateTime.UtcNow.Date,
                "version-1",
                "version-1",
                "version-1",
                DateTime.UtcNow.Date,
                DateTime.UtcNow.Date,
                null,
                null,
                null,
                null,
                OrderTypes.MARKET,
                OrderDirections.BUY,
                new Domain.Core.Financial.Money.Currency("GBP"),
                new Domain.Core.Financial.Money.Currency("GBP"),
                OrderCleanDirty.NONE,
                null,
                new Money(price.GetValueOrDefault(20), "GBP"),
                new Money(price.GetValueOrDefault(20), "GBP"),
                1000,
                1000,
                "trader id",
                "trader one",
                "clearing agent",
                "dealer-instructions",
                new OrderBroker(string.Empty, string.Empty, "Mr Broker", DateTime.Now, true), 
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new DealerOrder[0]);
        }
    }
}
