using Surveillance.Engine.Rules.Trades;

namespace Surveillance.Engine.Rules.Rules.Equity.WashTrade
{
    public class PositionCluster
    {
        public PositionCluster(TradePosition buys, TradePosition sells)
        {
            Buys = buys;
            Sells = sells;
        }

        public TradePosition Buys { get; }
        public TradePosition Sells { get; }
    }
}
