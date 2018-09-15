namespace Domain.Equity
{
    /// <summary>
    /// Daily high/lows open/close
    /// </summary>
    public class IntradayPrices
    {
        public IntradayPrices(
            Price? open,
            Price? close,
            Price? high,
            Price? low)
        {
            Open = open;
            Close = close;
            High = high;
            Low = low;
        }

        /// <summary>
        /// Opening trading price
        /// </summary>
        public Price? Open { get; }

        /// <summary>
        /// Closing trading price
        /// </summary>
        public Price? Close { get; }

        /// <summary>
        /// Intraday high trading price
        /// </summary>
        public Price? High { get; }

        /// <summary>
        /// Intraday low trading price
        /// </summary>
        public Price? Low { get; }
    }
}
