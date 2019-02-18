using Surveillance.Engine.Rules.Trades;

namespace Surveillance.Engine.Rules.Rules.WashTrade
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
