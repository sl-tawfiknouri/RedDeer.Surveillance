namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade
{
    using Surveillance.Engine.Rules.Trades;

    public class PositionCluster
    {
        public PositionCluster(TradePosition buys, TradePosition sells)
        {
            this.Buys = buys;
            this.Sells = sells;
        }

        public TradePosition Buys { get; }

        public TradePosition Sells { get; }
    }
}