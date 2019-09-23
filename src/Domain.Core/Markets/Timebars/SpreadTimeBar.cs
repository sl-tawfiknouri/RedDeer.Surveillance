namespace Domain.Core.Markets.Timebars
{
    using Domain.Core.Financial.Money;

    /// <summary>
    ///     bid / ask / market clearing price
    /// </summary>
    public struct SpreadTimeBar
    {
        public SpreadTimeBar(Money bid, Money ask, Money price, Volume volume)
        {
            this.Bid = bid;
            this.Ask = ask;
            this.Price = price;
            this.Volume = volume;
        }

        /// <summary>
        ///     Last buy price
        /// </summary>
        public Money Bid { get; }

        /// <summary>
        ///     Last sell price
        /// </summary>
        public Money Ask { get; }

        /// <summary>
        ///     The price the market resolved to
        /// </summary>
        public Money Price { get; }

        /// <summary>
        ///     The volume of the security traded since the last time bar
        /// </summary>
        public Volume Volume { get; }
    }
}