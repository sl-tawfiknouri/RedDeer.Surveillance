using System;

namespace Domain.Equity.Frames
{
    /// <summary>
    /// Market update for a security trading data
    /// </summary>
    public class SecurityTick
    {
        public SecurityTick(
            Security security,
            string cfiCode,
            string tickerSymbol,
            Spread spread,
            Volume volume,
            DateTime timeStamp,
            decimal? marketCap)
        {
            Security = security;
            CfiCode = cfiCode;
            TickerSymbol = tickerSymbol;
            Spread = spread;
            Volume = volume;
            TimeStamp = timeStamp;
            MarketCap = marketCap;
        }

        /// <summary>
        /// The security the tick data was related to
        /// </summary>
        public Security Security { get; }

        /// <summary>
        /// Classification of Financial Instruments codes (norm ISO 10962:2015)
        /// This describes an asset in terms such as BOND|EQUITY VOTING STOCK
        /// https://en.wikipedia.org/wiki/ISO_10962
        /// </summary>
        public string CfiCode { get; }

        /// <summary>
        /// Ticker symbol for describing the security
        /// </summary>
        public string TickerSymbol { get; }

        public decimal? MarketCap { get; }

        /// <summary>
        /// Price spread at the tick point
        /// </summary>
        public Spread Spread { get; }

        /// <summary>
        /// The volume of the security traded since the last tick
        /// </summary>
        public Volume Volume { get; }

        /// <summary>
        /// The time point at which the data was canonical
        /// </summary>
        public DateTime TimeStamp { get; }
    }
}