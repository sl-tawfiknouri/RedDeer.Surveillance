namespace DomainV2.Equity.TimeBars
{
    public class SecurityTickCsv
    {
        public string Timestamp { get; set; }
        public string MarketIdentifierCode { get; set; }
        public string MarketName { get; set; }


        public string SecurityName { get; set; }
        public string Currency { get; set; }

        public string Cfi { get; set; }
        public string IssuerIdentifier { get; set; }

        public string SecurityClientIdentifier { get; set; }
        public string Sedol { get; set; }
        public string Isin { get; set; }
        public string Figi { get; set; }
        public string ExchangeSymbol { get; set; }
        public string Cusip { get; set; }
        public string Lei { get; set; }
        public string BloombergTicker { get; set; }


        public string Ask { get; set; }
        public string Bid { get; set; }
        public string Price { get; set; }

        public string Open { get; set; }
        public string Close { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public string DailyVolume { get; set; }


        public string Volume { get; set; }
        public string ListedSecurities { get; set; }
        public string MarketCap { get; set; }

        public int RowId { get; set; }
    }
}