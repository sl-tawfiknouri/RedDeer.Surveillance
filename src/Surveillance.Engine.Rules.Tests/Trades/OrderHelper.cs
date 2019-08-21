namespace Surveillance.Engine.Rules.Tests.Trades
{
    using System;

    using Domain.Core.Financial.Assets;
    using Domain.Core.Financial.Money;
    using Domain.Core.Markets;
    using Domain.Core.Trading.Orders;

    public static class OrderHelper
    {
        public static Order Orders(OrderStatus status)
        {
            var securityIdentifiers = new InstrumentIdentifiers(
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

            var security = new FinancialInstrument(
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
                new Currency("GBP"),
                new Currency("GBP"),
                OrderCleanDirty.NONE,
                null,
                new Money(1000, "GBP"),
                new Money(1000, "GBP"),
                1000,
                1000,
                "Trader - 1",
                "trader one",
                "Rybank Long",
                "deal-asap",
                new OrderBroker(string.Empty, string.Empty, "Mr Broker", DateTime.UtcNow, true),
                null,
                null,
                OptionEuropeanAmerican.NONE,
                new DealerOrder[0]);
        }
    }
}