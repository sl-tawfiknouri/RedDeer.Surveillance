using System;

namespace Surveillance.DataLayer.Stub
{
    public class MarketOpenClose
    {
        public MarketOpenClose(
            string marketId,
            DateTime marketOpen,
            DateTime marketClose)
        {
            MarketId = marketId ?? string.Empty;
            MarketOpen = marketOpen;
            MarketClose = marketClose;
        }

        public string MarketId { get; }
        public DateTime MarketOpen { get; }
        public DateTime MarketClose { get; }
    }
}
