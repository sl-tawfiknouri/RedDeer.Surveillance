using System.Collections.Generic;
using DomainV2.Trading;

namespace Surveillance.Engine.Rules.Rules.WashTrade.Interfaces
{
    public interface IWashTradeClustering
    {
        IReadOnlyCollection<PositionClusterCentroid> Cluster(IReadOnlyCollection<Order> frames);
    }
}