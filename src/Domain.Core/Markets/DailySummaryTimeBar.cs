using System;

namespace Domain.Equity.TimeBars
{
    public class DailySummaryTimeBar
    {
        public DailySummaryTimeBar(
            decimal? marketCap,
            IntradayPrices intradayPrices, 
            long? listedSecurities,
            Volume dailyVolume, 
            DateTime timeStamp)
        {
            MarketCap = marketCap;
            IntradayPrices = intradayPrices;
            ListedSecurities = listedSecurities;
            DailyVolume = dailyVolume;
            TimeStamp = timeStamp;
        }

        /// <summary>
        /// Valuation of the security
        /// </summary>
        public decimal? MarketCap { get; }

        public IntradayPrices IntradayPrices { get; }

        /// <summary>
        /// The number of the listed security on the exchange
        /// </summary>
        public long? ListedSecurities { get; }

        /// <summary>
        /// The daily volume traded of the security on the exchange
        /// </summary>
        public Volume DailyVolume { get; }

        /// <summary>
        /// The time point at which the data was canonical
        /// </summary>
        public DateTime TimeStamp { get; }
    }
}
