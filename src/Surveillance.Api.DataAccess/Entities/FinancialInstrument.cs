﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Domain.Core.Financial.Assets;
using Surveillance.Api.DataAccess.Abstractions.Entities;

namespace Surveillance.Api.DataAccess.Entities
{
    public class FinancialInstrument : IFinancialInstrument
    {
        [Key]
        public int Id { get; set; }
        public int MarketId { get; set; }
        [NotMapped]
        public IMarket Market { get; set; }
        public string ClientIdentifier { get; set; }
        public string Sedol { get; set; }
        public string Isin { get; set; }
        public string Figi { get; set; }
        public string Cusip { get; set; }
        public string Lei { get; set; }
        public string ExchangeSymbol { get; set; }
        public string BloombergTicker { get; set; }
        public string SecurityName { get; set; }
        public string Cfi { get; set; }
        public string IssuerIdentifier { get; set; }
        public string SecurityCurrency { get; set; }
        public string ReddeerId { get; set; }
        public DateTime? Enrichment { get; set; }

        [NotMapped]
        public string EnrichmentDate => Enrichment?.ToString(CultureInfo.GetCultureInfo("en-GB")) ?? string.Empty;

        public int InstrumentType { get; set; }

        [NotMapped]
        public InstrumentTypes InstrumentTypes { get; set; }
        public string UnderlyingCfi { get; set; }
        public string UnderlyingName { get; set; }
        public string UnderlyingSedol { get; set; }
        public string UnderlyingIsin { get; set; }
        public string UnderlyingFigi { get; set; }
        public string UnderlyingCusip { get; set; }
        public string UnderlyingLei { get; set; }
        public string UnderlyingExchangeSymbol { get; set; }
        public string UnderlyingBloombergTicker { get; set; }
        public string UnderlyingClientIdentifier { get; set; }
    }
}
