using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RedDeer.Surveillance.Api.Client.Dtos
{
    public class FinancialInstrumentDto
    {
        public int Id { get; set; }
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
        public string ReddeerId { get; set; }
    }
}
