using Domain.Core.Financial.Money;

namespace Domain.Core.Markets.Timebars
{
    /// <summary>
    /// Daily high/lows open/close
    /// </summary>
    public class IntradayPrices
    {
        public IntradayPrices(
            Money? open,
            Money? close,
            Money? high,
            Money? low)
        {
            Open = open;
            Close = close;
            High = high;
            Low = low;
        }

        /// <summary>
        /// Opening trading price
        /// </summary>
        public Money? Open { get; }

        /// <summary>
        /// Closing trading price
        /// </summary>
        public Money? Close { get; }

        /// <summary>
        /// Intraday high trading price
        /// </summary>
        public Money? High { get; }

        /// <summary>
        /// Intraday low trading price
        /// </summary>
        public Money? Low { get; }
    }
}
