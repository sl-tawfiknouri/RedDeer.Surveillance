﻿using System.Collections.Generic;
using Domain.Trading;

namespace Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces
{
    public interface IWashTradeClustering
    {
        IReadOnlyCollection<PositionClusterCentroid> Cluster(IReadOnlyCollection<Order> frames);
    }
}