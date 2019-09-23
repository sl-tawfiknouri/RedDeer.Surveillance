namespace Domain.Core.Markets.Timebars
{
    using Domain.Core.Financial.Money;

    /// <summary>
    ///     Daily high/lows open/close
    /// </summary>
    public class IntradayPrices
    {
        public IntradayPrices(Money? open, Money? close, Money? high, Money? low)
        {
            this.Open = open;
            this.Close = close;
            this.High = high;
            this.Low = low;
        }

        /// <summary>
        ///     Closing trading price
        /// </summary>
        public Money? Close { get; }

        /// <summary>
        ///     Intraday high trading price
        /// </summary>
        public Money? High { get; }

        /// <summary>
        ///     Intraday low trading price
        /// </summary>
        public Money? Low { get; }

        /// <summary>
        ///     Opening trading price
        /// </summary>
        public Money? Open { get; }
    }
}