using System;
using Domain.Core.Financial.Assets;

namespace Surveillance.Api.DataAccess.Abstractions.Entities
{
    public interface IFinancialInstrument : IUnderlyingFinancialInstrument
    {
        int Id { get; set; }
        int MarketId { get; set; }
        IMarket Market { get; set; }
        string ClientIdentifier { get; set; }
        string Sedol { get; set; }
        string Isin { get; set; }
        string Figi { get; set; }
        string Cusip { get; set; }
        string Lei { get; set; }
        string ExchangeSymbol { get; set; }
        string BloombergTicker { get; set; }
        string SecurityName { get; set; }
        string Cfi { get; set; }
        string IssuerIdentifier { get; set; }
        string SecurityCurrency { get; set; }
        string ReddeerId { get; set; }
        DateTime? Enrichment { get; set; }
        int InstrumentType { get; set; }
        InstrumentTypes InstrumentTypes { get; set; }
    }
}
