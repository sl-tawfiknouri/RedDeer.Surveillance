using System.Collections.Generic;
using System.Linq;
using Domain.Core.Trading.Orders;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis
{
    public class PriceImpactClassifier : IPriceImpactClassifier
    {
        IReadOnlyCollection<PriceImpactClassification> IPriceImpactClassifier.ClassifyByTradeCount(IReadOnlyCollection<Order> orders)
        {
            if (!orders.Any())
            {
                return new[] { PriceImpactClassification.Unknown };
            }

            return new[] { PriceImpactClassification.Positive };
        }

        IReadOnlyCollection<PriceImpactClassification> IPriceImpactClassifier.ClassifyByWeightedVolume(IReadOnlyCollection<Order> orders)
        {
            if (!orders.Any())
            {
                return new[] { PriceImpactClassification.Unknown };
            }

            return new [] { PriceImpactClassification.Positive };
        }
    }
}
