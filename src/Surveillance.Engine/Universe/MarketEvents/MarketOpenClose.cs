namespace Surveillance.Engine.Rules.Universe.MarketEvents
{
    using System;

    public class MarketOpenClose
    {
        public MarketOpenClose(string marketId, DateTime marketOpen, DateTime marketClose)
        {
            this.MarketId = marketId ?? string.Empty;
            this.MarketOpen = marketOpen;
            this.MarketClose = marketClose;
        }

        public DateTime MarketClose { get; }

        public string MarketId { get; }

        public DateTime MarketOpen { get; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MarketOpenClose castObj)) return false;

            return string.Equals(this.MarketId, castObj.MarketId, StringComparison.InvariantCultureIgnoreCase)
                   && this.MarketOpen == castObj.MarketOpen && this.MarketClose == castObj.MarketClose;
        }

        public override int GetHashCode()
        {
            return this.MarketId.GetHashCode() * 17 + this.MarketOpen.GetHashCode() * 19
                                                    + this.MarketClose.GetHashCode() * 23;
        }

        public override string ToString()
        {
            return
                $"Market({this.MarketId}) Open({this.MarketOpen.ToShortTimeString()}) Close({this.MarketClose.ToShortTimeString()})";
        }
    }
}