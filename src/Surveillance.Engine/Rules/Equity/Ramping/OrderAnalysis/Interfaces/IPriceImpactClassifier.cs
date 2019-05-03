using System.Collections.Generic;
using Domain.Core.Trading.Orders;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces
{
    public interface IPriceImpactClassifier
    {
        IReadOnlyCollection<IPriceImpactSummary> ClassifyByTradeCount(IReadOnlyCollection<Order> orders);
        IReadOnlyCollection<IPriceImpactSummary> ClassifyByWeightedVolume(IReadOnlyCollection<Order> orders);
    }
}