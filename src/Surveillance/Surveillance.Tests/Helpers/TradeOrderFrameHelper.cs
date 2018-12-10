using System;
using DomainV2.Financial;
using DomainV2.Trading;

namespace Surveillance.Tests.Helpers
{
    public static class TradeOrderFrameHelper
    {
        public static Order Random(this Order frame, int? price = 20)
        {
            return new Order(
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    new InstrumentIdentifiers(string.Empty, "reddeer-id", "client-identifier", "sedol", "isin", "figi",
                        "cusip", "xlon", "lei",
                        "bloomberg"),
                    "random-security",
                    "ENTSPB",
                    "Random Inc"),
                new Market("1", "XLON", "London Stock Exchange", MarketTypes.STOCKEXCHANGE),
                null,
                "order-1",
                DateTime.Now,
                DateTime.Now,
                null,
                null,
                null,
                null,
                OrderTypes.MARKET,
                OrderPositions.BUY,
                new DomainV2.Financial.Currency("GBP"),
                new CurrencyAmount(price.GetValueOrDefault(20), "GBP"),
                new CurrencyAmount(price.GetValueOrDefault(20), "GBP"),
                1000,
                1000,
                "mr portfolio manager",
                "trader id",
                "executing broker",
                "clearing agent",
                "dealer-instructions",
                "asap strategy",
                "rationale",
                "mega fund",
                "client-id",
                new Trade[0]);
        }
    }
}
