namespace Domain.Equity.TimeBars
{
    /// <summary>
    /// bid / ask / market clearing price
    /// </summary>
    public struct SpreadTimeBar
    {
        public SpreadTimeBar(
            CurrencyAmount bid,
            CurrencyAmount ask,
            CurrencyAmount price,
            Volume volume)
        {
            Bid = bid;
            Ask = ask;
            Price = price;
            Volume = volume;
        }

        /// <summary>
        /// Last buy price
        /// </summary>
        public CurrencyAmount Bid { get; }

        /// <summary>
        /// Last sell price
        /// </summary>
        public CurrencyAmount Ask { get; }

        /// <summary>
        /// The price the market resolved to
        /// </summary>
        public CurrencyAmount Price { get; }

        /// <summary>
        /// The volume of the security traded since the last time bar
        /// </summary>
        public Volume Volume { get; }
    }
}
