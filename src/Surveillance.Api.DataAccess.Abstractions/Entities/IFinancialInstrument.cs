namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    using System;

    using Domain.Core.Financial.Assets;

    public interface IFinancialInstrument : IUnderlyingFinancialInstrument
    {
        string BloombergTicker { get; set; }

        string Cfi { get; set; }

        string ClientIdentifier { get; set; }

        string Cusip { get; set; }

        DateTime? Enrichment { get; set; }

        string ExchangeSymbol { get; set; }

        string Figi { get; set; }

        int Id { get; set; }

        int InstrumentType { get; set; }

        InstrumentTypes InstrumentTypes { get; set; }

        string Isin { get; set; }

        string IssuerIdentifier { get; set; }

        string Lei { get; set; }

        IMarket Market { get; set; }

        int MarketId { get; set; }

        string ReddeerId { get; set; }

        string SecurityCurrency { get; set; }

        string SecurityName { get; set; }

        string Sedol { get; set; }
    }
}