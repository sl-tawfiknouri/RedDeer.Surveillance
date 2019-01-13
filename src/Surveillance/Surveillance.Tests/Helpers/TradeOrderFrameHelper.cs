﻿using System;
using DomainV2.Financial;
using DomainV2.Trading;

namespace Surveillance.Tests.Helpers
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
                "version-1",
                "version-1",
                "version-1",
                DateTime.Now,
                DateTime.Now,
                null,
                null,
                null,
                null,
                OrderTypes.MARKET,
                OrderDirections.BUY,
                new DomainV2.Financial.Currency("GBP"),
                new DomainV2.Financial.Currency("GBP"),
                OrderCleanDirty.None,
                null,
                new CurrencyAmount(price.GetValueOrDefault(20), "GBP"),
                new CurrencyAmount(price.GetValueOrDefault(20), "GBP"),
                1000,
                1000,
                "trader id",
                "clearing agent",
                "dealer-instructions",
                null,
                null,
                OptionEuropeanAmerican.None,
                new DealerOrder[0]);
        }
    }
}
