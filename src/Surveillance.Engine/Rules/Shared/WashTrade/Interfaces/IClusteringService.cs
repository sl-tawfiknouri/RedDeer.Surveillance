using System.Collections.Generic;
using Domain.Trading;

namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces
{
    public interface IClusteringService
    {
        IReadOnlyCollection<PositionClusterCentroid> Cluster(IReadOnlyCollection<Order> frames);
    }
}