using System;
using Domain.Financial;
using Domain.Trading;

namespace Surveillance.Engine.Rules.Tests.Trades
{
    public static class OrderHelper
    {
        public static Order Orders(OrderStatus status)
        {
            var securityIdentifiers =
                new InstrumentIdentifiers(
                    string.Empty,
                    "reddeer id",
                    null,
                    "client id",
                    "1234567",
                    "12345678912",
                    "figi",
                    "cusip",
                    "test",
                    "test lei",
                    "ticker");

            var security =
                new FinancialInstrument(
                    InstrumentTypes.Equity,
                    securityIdentifiers,
                    "Test Security",
                    "CFI",
                    "USD",
                    "Issuer Identifier");

            var cancelledDate = status == OrderStatus.Cancelled ? (DateTime?)DateTime.UtcNow : null;
            var filledDate = status == OrderStatus.Filled ? (DateTime?)DateTime.UtcNow : null;

            return new Order(
                security,
                new Market("1", "XLON", "XLON", MarketTypes.STOCKEXCHANGE),
                null,
                "id1",
                DateTime.UtcNow,
                "version-1",
                "version-1",
                "version-1",
                DateTime.UtcNow,
                null,
                null,
                null,
                cancelledDate,
                filledDate,
                OrderTypes.MARKET,
                OrderDirections.BUY,
                new Domain.Financial.Currency("GBP"),
                new Domain.Financial.Currency("GBP"),
                OrderCleanDirty.NONE,
                null,
                new CurrencyAmount(1000, "GBP"),
                new CurrencyAmount(1000, "GBP"),
                1000,
                1000,
                "Trader - 1",
                "trader one",
                "Rybank Long",
                "deal-asap",
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new DealerOrder[0]);
            }
        }
    }