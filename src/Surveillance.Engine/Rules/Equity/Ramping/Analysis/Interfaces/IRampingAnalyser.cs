﻿using System.Collections.Generic;
using Domain.Core.Trading.Orders;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis.Interfaces
{
    public interface IRampingAnalyser
    {
        IRampingStrategySummaryPanel Analyse(IReadOnlyCollection<Order> orderSegment);
    }
}