namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade
{
    using Domain.Core.Trading;

    /// <summary>
    /// The position cluster centroid.
    /// </summary>
    public class PositionClusterCentroid : PositionCluster
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionClusterCentroid"/> class.
        /// </summary>
        /// <param name="centroidPrice">
        /// The centroid price.
        /// </param>
        /// <param name="buys">
        /// The buys.
        /// </param>
        /// <param name="sells">
        /// The sells.
        /// </param>
        public PositionClusterCentroid(decimal centroidPrice, TradePosition buys, TradePosition sells)
            : base(buys, sells)
        {
            this.CentroidPrice = centroidPrice;
        }

        /// <summary>
        /// Gets the centroid price.
        /// </summary>
        public decimal CentroidPrice { get; }
    }
}