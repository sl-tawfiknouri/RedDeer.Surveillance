using System;
using Domain.Trading;

namespace Surveillance.Engine.Rules.Tests.Helpers
{
    public static class TradeOrderFrameHelper
    {
        public static Order Random(this Order frame, int? price = 20)
        {
            var fi = new FinancialInstrument(
                InstrumentTypes.Equity,
                new InstrumentIdentifiers(string.Empty, "reddeer-id", null, "client-identifier", "sedol", "isin",
                    "figi",
                    "cusip", "xlon", "lei",
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
                new Domain.Financial.Currency("GBP"),
                new Domain.Financial.Currency("GBP"),
                OrderCleanDirty.NONE,
                null,
                new CurrencyAmount(price.GetValueOrDefault(20), "GBP"),
                new CurrencyAmount(price.GetValueOrDefault(20), "GBP"),
                1000,
                1000,
                "trader id",
                "trader one",
                "clearing agent",
                "dealer-instructions",
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new DealerOrder[0]);
        }
    }
}
