namespace Surveillance.DataLayer.Aurora.Analytics
{
    public class UniverseAnalytics
    {
        // primary key
        public int Id { get; set; }

        // foreign key - must be set
        public int SystemProcessOperationId { get; set;}

        // event counts
        public int GenesisEventCount { get; set; }
        public int EschatonEventCount { get; set; }
        public int TradeReddeerCount { get; set; }
        public int TradeReddeerSubmittedCount { get; set;}
        public int StockTickReddeerCount { get; set; }
        public int StockMarketOpenCount { get; set; }
        public int StockMarketCloseCount { get; set; }
        // end event counts

        // analysis of uniqueness
        public int UniqueTradersCount { get; set; }
        public int UniqueSecuritiesCount { get; set; }
        public int UniqueMarketsTradedOnCount { get; set; }
        // end of uniqueness analysis

    }
}
