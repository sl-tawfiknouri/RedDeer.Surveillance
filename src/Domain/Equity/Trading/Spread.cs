namespace Domain.Equity.Trading
{
    /// <summary>
    /// Sell / Buy
    /// </summary>
    public class Spread
    {
        public Spread(Price buy, Price sell)
        {
            Buy = buy;
            Sell = sell;
        }

        /// <summary>
        /// Last buy price
        /// </summary>
        public Price Buy { get; }

        /// <summary>
        /// Last sell price
        /// </summary>
        public Price Sell { get; }
    }
}
