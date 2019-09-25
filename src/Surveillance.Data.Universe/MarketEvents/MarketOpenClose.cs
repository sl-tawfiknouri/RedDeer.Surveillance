namespace Surveillance.Data.Universe.MarketEvents
{
    using System;

    /// <summary>
    /// The market open close.
    /// </summary>
    public class MarketOpenClose
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketOpenClose"/> class.
        /// </summary>
        /// <param name="marketId">
        /// The market id.
        /// </param>
        /// <param name="marketOpen">
        /// The market open.
        /// </param>
        /// <param name="marketClose">
        /// The market close.
        /// </param>
        public MarketOpenClose(string marketId, DateTime marketOpen, DateTime marketClose)
        {
            this.MarketId = marketId ?? string.Empty;
            this.MarketOpen = marketOpen;
            this.MarketClose = marketClose;
        }

        /// <summary>
        /// Gets the market close.
        /// </summary>
        public DateTime MarketClose { get; }

        /// <summary>
        /// Gets the market id.
        /// </summary>
        public string MarketId { get; }

        /// <summary>
        /// Gets the market open.
        /// </summary>
        public DateTime MarketOpen { get; }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MarketOpenClose castObj))
            {
                return false;
            }

            return string.Equals(this.MarketId, castObj.MarketId, StringComparison.InvariantCultureIgnoreCase)
                   && this.MarketOpen == castObj.MarketOpen && this.MarketClose == castObj.MarketClose;
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.MarketId.GetHashCode() * 17 + this.MarketOpen.GetHashCode() * 19
                                                    + this.MarketClose.GetHashCode() * 23;
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return
                $"Market({this.MarketId}) Open({this.MarketOpen.ToShortTimeString()}) Close({this.MarketClose.ToShortTimeString()})";
        }
    }
}