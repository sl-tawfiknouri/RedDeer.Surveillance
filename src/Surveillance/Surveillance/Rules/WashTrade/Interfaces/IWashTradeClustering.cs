using System.Collections.Generic;
using Domain.Trades.Orders;

namespace Surveillance.Rules.WashTrade.Interfaces
{
    public interface IWashTradeClustering
    {
        IReadOnlyCollection<PositionClusterCentroid> Cluster(IReadOnlyCollection<TradeOrderFrame> frames);
    }
}