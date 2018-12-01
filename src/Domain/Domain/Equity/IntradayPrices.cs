using Domain.Finance;

namespace Domain.Equity
{
    /// <summary>
    /// Daily high/lows open/close
    /// </summary>
    public class IntradayPrices
    {
        public IntradayPrices(
            CurrencyAmount? open,
            CurrencyAmount? close,
            CurrencyAmount? high,
            CurrencyAmount? low)
        {
            Open = open;
            Close = close;
            High = high;
            Low = low;
        }

        /// <summary>
        /// Opening trading price
        /// </summary>
        public CurrencyAmount? Open { get; }

        /// <summary>
        /// Closing trading price
        /// </summary>
        public CurrencyAmount? Close { get; }

        /// <summary>
        /// Intraday high trading price
        /// </summary>
        public CurrencyAmount? High { get; }

        /// <summary>
        /// Intraday low trading price
        /// </summary>
        public CurrencyAmount? Low { get; }
    }
}
