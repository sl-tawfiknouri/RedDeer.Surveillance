using System.Collections.Generic;
using Domain.Core.Trading.Orders;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces
{
    public interface IPriceImpactClassifier
    {
        IPriceImpactSummary ClassifyByTradeCount(IReadOnlyCollection<Order> orders, TimeSegment segment);
        IPriceImpactSummary ClassifyByWeightedVolume(IReadOnlyCollection<Order> orders, TimeSegment segment);
    }
}