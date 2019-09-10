namespace Domain.Core.Markets.Timebars
{
    using System;

    using Domain.Core.Financial.Money;

    public class DailySummaryTimeBar
    {
        public DailySummaryTimeBar(
            decimal? marketCap,
            string currency,
            IntradayPrices intradayPrices, 
            long? listedSecurities,
            Volume dailyVolume, 
            DateTime timeStamp)
        {
            this.MarketCap =
                marketCap != null 
                    ? (Money?)new Money(marketCap, currency) 
                    : null; 
            this.IntradayPrices = intradayPrices;
            this.ListedSecurities = listedSecurities;
            this.DailyVolume = dailyVolume;
            this.TimeStamp = timeStamp;
        }

        public Money? MarketCap { get; }

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
