using Surveillance.Trades;

namespace Surveillance.Rules.WashTrade
{
    public class PositionClusterCentroid : PositionCluster
    {
        public PositionClusterCentroid(
            decimal centroidPrice,
            TradePosition buys,
            TradePosition sells)
            : base(buys, sells)
        {
            CentroidPrice = centroidPrice;
        }

        public decimal CentroidPrice { get; }
    }
}
