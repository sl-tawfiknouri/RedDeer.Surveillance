namespace Domain.Core.Markets.Timebars
{
    using System;

    public class DailySummaryTimeBar
    {
        public DailySummaryTimeBar(
            decimal? marketCap,
            IntradayPrices intradayPrices,
            long? listedSecurities,
            Volume dailyVolume,
            DateTime timeStamp)
        {
            this.MarketCapCents = marketCap;
            this.IntradayPrices = intradayPrices;
            this.ListedSecurities = listedSecurities;
            this.DailyVolume = dailyVolume;
            this.TimeStamp = timeStamp;
        }

        /// <summary>
        ///     The daily volume traded of the security on the exchange
        /// </summary>
        public Volume DailyVolume { get; }

        public IntradayPrices IntradayPrices { get; }

        /// <summary>
        ///     The number of the listed security on the exchange
        /// </summary>
        public long? ListedSecurities { get; }

        /// <summary>
        ///     Valuation of the security in full USD
        /// </summary>
        public decimal? MarketCap =>
            this.MarketCapCents.GetValueOrDefault(0) > 0 ? this.MarketCapCents / 100 : this.MarketCapCents;

        public decimal? MarketCapCents { get; }

        /// <summary>
        ///     The time point at which the data was canonical
        /// </summary>
        public DateTime TimeStamp { get; }
    }
}