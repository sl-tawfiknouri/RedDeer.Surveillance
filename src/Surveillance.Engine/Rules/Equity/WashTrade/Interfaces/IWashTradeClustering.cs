using System.Collections.Generic;
using Domain.Core.Trading.Orders;

namespace Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces
{
    public interface IWashTradeClustering
    {
        IReadOnlyCollection<PositionClusterCentroid> Cluster(IReadOnlyCollection<Order> frames);
    }
}