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

        public override int GetHashCode()
        {
            return MarketId.GetHashCode() * 17
                   + MarketOpen.GetHashCode() * 19
                   + MarketClose.GetHashCode() * 23;
        }

        public override bool Equals(object obj)
        {
            if (obj == null
                || !(obj is MarketOpenClose castObj))
            {
                return false;
            }

            return string.Equals(MarketId, castObj.MarketId, StringComparison.InvariantCultureIgnoreCase)
               && MarketOpen == castObj.MarketOpen
               && MarketClose == castObj.MarketClose;
        }

        public override string ToString()
        {
            return $"Market({MarketId}) Open({MarketOpen.ToShortTimeString()}) Close({MarketClose.ToShortTimeString()})";
        }
    }
}
