﻿using System;
using Domain.Equity;
using Domain.Market;
using Domain.Trades.Orders;

namespace Surveillance.Tests.Helpers
{
    public static class TradeOrderFrameHelper
    {
        public static TradeOrderFrame Random(this TradeOrderFrame frame)
        {
            return new TradeOrderFrame(
                OrderType.Market,
                new StockExchange(new Market.MarketId("XLON"), "London Stock Exchange"),
                new Security(
                    new SecurityIdentifiers("client-identifier", "sedol", "isin", "figi", "cusip", "xlon", "lei",
                        "bloomberg"),
                    "random-security",
                    "ENTSPB",
                    "Random Inc"),
                null,
                null,
                1000,
                1000,
                OrderPosition.Buy,
                OrderStatus.Booked,
                new DateTime(2018, 01, 01),
                new DateTime(2018, 02, 01),
                "trader-id",
                "trade-client-attribution-id",
                "account-id",
                "dealer-instructions",
                "party-broker-id",
                "counter-party-broker-id",
                "trade-rationale",
                "trade-strategy",
                "GBP");
        }
    }
}