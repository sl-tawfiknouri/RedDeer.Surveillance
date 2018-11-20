namespace Surveillance.Analytics
{
    public class UniverseAnalytics
    {

        // event counts
        public int UnknownEventCount { get; set; }
        public int GenesisEventCount { get; set; }
        public int EschatonEventCount { get; set; }
        public int TradeReddeerCount { get; set; }
        public int TradeReddeerSubmittedCount { get; set;}
        public int StockTickReddeerCount { get; set; }
        public int StockMarketOpenCount { get; set; }
        public int StockMarketCloseCount { get; set; }
        // end event counts

        // analysis of uniqueness
        public int UniqueTraders { get; set; }
        public int UniqueSecurities { get; set; }
        public int UniqueMarketsTradedOn { get; set; }
        // end of uniqueness analysis

    }
}
