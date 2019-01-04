using System;
using DomainV2.Financial;

namespace DomainV2.Equity.TimeBars
{
    /// <summary>
    /// Market update for financial instrument trading data
    /// </summary>
    public class FinancialInstrumentTimeBar
    {
        public FinancialInstrumentTimeBar(
            FinancialInstrument security,
            Spread spread,
            Volume volume,
            Volume dailyVolume,
            DateTime timeStamp,
            decimal? marketCap,
            IntradayPrices intradayPrices,
            long? listedSecurities,
            Market market)
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
        public FinancialInstrument Security { get; }

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
        public DomainV2.Financial.Market Market { get; }
    }
}