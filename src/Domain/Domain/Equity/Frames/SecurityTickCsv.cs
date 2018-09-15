namespace Domain.Equity.Frames
{
    public class SecurityTickCsv
    {
        public string Timestamp { get; set; }
        public string MarketIdentifierCode { get; set; }
        public string MarketName { get; set; }


        public string SecurityName { get; set; }
        public string SecurityCurrency { get; set; }

        public string SecurityClientIdentifier { get; set; }
        public string SecuritySedol { get; set; }
        public string SecurityIsin { get; set; }
        public string SecurityFigi { get; set; }
        public string SecurityCfi { get; set; }
        public string SecurityExchangeSymbol { get; set; }
        public string SecurityCusip { get; set; }

        public string SpreadAsk { get; set; }
        public string SpreadBid { get; set; }
        public string SpreadPrice { get; set; }

        public string OpenPrice { get; set; }
        public string ClosePrice { get; set; }
        public string HighPrice { get; set; }
        public string LowPrice { get; set; }

        public string VolumeTraded { get; set; }
        public string ListedSecurities { get; set; }
        public string MarketCap { get; set; }
    }
}