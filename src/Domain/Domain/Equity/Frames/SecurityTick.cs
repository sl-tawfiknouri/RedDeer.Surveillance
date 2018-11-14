using System;
using Domain.Market;

namespace Domain.Equity.Frames
{
    /// <summary>
    /// Market update for a security trading data
    /// </summary>
    public class SecurityTick
    {
        public SecurityTick(
            Security security,
            Spread spread,
            Volume volume,
            Volume dailyVolume,
            DateTime timeStamp,
            decimal? marketCap,
            IntradayPrices intradayPrices,
            long? listedSecurities,
            StockExchange market)
        {
            Security = security;
            Spread = spread;
            Volume = volume;
            TimeStamp = timeStamp;
            MarketCap = marketCap;
            IntradayPrices = intradayPrices;
            ListedSecurities = listedSecurities;
            Market = market;
            DailyVolume = dailyVolume;
        }

        /// <summary>
        /// The security the tick data was related to
        /// </summary>
        public Security Security { get; }

        /// <summary>
        /// Valuation of the security
        /// </summary>
        public decimal? MarketCap { get; }

        /// <summary>
        /// Price spread at the tick point
        /// </summary>
        public Spread Spread { get; }

        public IntradayPrices IntradayPrices { get; }

        /// <summary>
        /// The number of the listed security on the exchange
        /// </summary>
        public long? ListedSecurities { get; }

        /// <summary>
        /// The volume of the security traded since the last tick
        /// </summary>
        public Volume Volume { get; }

        /// <summary>
        /// The daily volume traded of the security on the exchange
        /// </summary>
        public Volume DailyVolume { get; }

        /// <summary>
        /// The time point at which the data was canonical
        /// </summary>
        public DateTime TimeStamp { get; }

        /// <summary>
        /// The market the security is traded on
        /// </summary>
        public StockExchange Market { get; }
    }
}