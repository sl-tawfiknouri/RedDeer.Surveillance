using System.Collections.Generic;
using Domain.Core.Trading.Orders;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces
{
    public interface IPriceImpactClassifier
    {
        IReadOnlyCollection<PriceImpactClassification> ClassifyByTradeCount(IReadOnlyCollection<Order> orders);
        IReadOnlyCollection<PriceImpactClassification> ClassifyByWeightedVolume(IReadOnlyCollection<Order> orders);
    }
}