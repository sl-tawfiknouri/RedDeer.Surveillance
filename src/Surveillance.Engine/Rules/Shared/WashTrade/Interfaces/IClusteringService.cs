using System.Collections.Generic;
using Domain.Core.Trading.Orders;

namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces
{
    public interface IClusteringService
    {
        IReadOnlyCollection<PositionClusterCentroid> Cluster(IReadOnlyCollection<Order> frames);
    }
}