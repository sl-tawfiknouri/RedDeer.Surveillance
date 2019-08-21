namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade
{
    using Surveillance.Engine.Rules.Trades;

    public class PositionClusterCentroid : PositionCluster
    {
        public PositionClusterCentroid(decimal centroidPrice, TradePosition buys, TradePosition sells)
            : base(buys, sells)
        {
            this.CentroidPrice = centroidPrice;
        }

        public decimal CentroidPrice { get; }
    }
}