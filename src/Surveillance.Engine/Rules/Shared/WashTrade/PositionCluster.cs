namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade
{
    using Domain.Core.Trading;

    /// <summary>
    /// The position cluster.
    /// </summary>
    public class PositionCluster
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionCluster"/> class.
        /// </summary>
        /// <param name="buys">
        /// The buys.
        /// </param>
        /// <param name="sells">
        /// The sells.
        /// </param>
        public PositionCluster(TradePosition buys, TradePosition sells)
        {
            this.Buys = buys;
            this.Sells = sells;
        }

        /// <summary>
        /// Gets the buys.
        /// </summary>
        public TradePosition Buys { get; }

        /// <summary>
        /// Gets the sells.
        /// </summary>
        public TradePosition Sells { get; }
    }
}