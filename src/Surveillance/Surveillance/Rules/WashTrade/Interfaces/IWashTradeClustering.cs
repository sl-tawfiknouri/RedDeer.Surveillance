using System.Collections.Generic;
using DomainV2.Trading;

namespace Surveillance.Rules.WashTrade.Interfaces
{
    public interface IWashTradeClustering
    {
        IReadOnlyCollection<PositionClusterCentroid> Cluster(IReadOnlyCollection<Order> frames);
    }
}