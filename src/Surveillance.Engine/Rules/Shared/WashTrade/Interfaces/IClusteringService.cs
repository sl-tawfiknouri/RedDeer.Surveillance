namespace Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces
{
    using System.Collections.Generic;

    using Domain.Core.Trading.Orders;

    public interface IClusteringService
    {
        IReadOnlyCollection<PositionClusterCentroid> Cluster(IReadOnlyCollection<Order> frames);
    }
}