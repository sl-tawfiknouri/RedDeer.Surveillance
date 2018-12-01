using Domain.Finance;

namespace Domain.Equity
{
    /// <summary>
    /// bid / ask / market clearing price
    /// </summary>
    public struct Spread
    {
        public Spread(CurrencyAmount bid, CurrencyAmount ask, CurrencyAmount price)
        {
            Bid = bid;
            Ask = ask;
            Price = price;
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
    }
}
