using System.Collections.Generic;
using Domain.Trading;
using Surveillance.Engine.Rules.Rules.Shared;
using Surveillance.Engine.Rules.Rules.Shared.WashTrade;

namespace Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces
{
    public interface IWashTradeClustering
    {
        IReadOnlyCollection<PositionClusterCentroid> Cluster(IReadOnlyCollection<Order> frames);
    }
}