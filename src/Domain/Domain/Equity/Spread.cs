namespace Domain.Equity
{
    /// <summary>
    /// bid / ask / market clearing price
    /// </summary>
    public struct Spread
    {
        public Spread(Price bid, Price ask, Price price)
        {
            Bid = bid;
            Ask = ask;
            Price = price;
        }

        /// <summary>
        /// Last buy price
        /// </summary>
        public Price Bid { get; }

        /// <summary>
        /// Last sell price
        /// </summary>
        public Price Ask { get; }

        /// <summary>
        /// The price the market resolved to
        /// </summary>
        public Price Price { get; }
    }
}
