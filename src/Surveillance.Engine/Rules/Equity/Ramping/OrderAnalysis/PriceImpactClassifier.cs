using System.Collections.Generic;
using System.Linq;
using Domain.Core.Trading.Orders;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis
{
    public class PriceImpactClassifier : IPriceImpactClassifier
    {
        IPriceImpactSummary IPriceImpactClassifier.ClassifyByTradeCount(IReadOnlyCollection<Order> orders, TimeSegment segment)
        {
            if (!orders.Any())
            {
                return new PriceImpactSummary(PriceImpactClassification.Unknown, segment);
            }

            return new PriceImpactSummary(PriceImpactClassification.Positive, segment);
        }

        IPriceImpactSummary IPriceImpactClassifier.ClassifyByWeightedVolume(IReadOnlyCollection<Order> orders, TimeSegment segment)
        {
            if (!orders.Any())
            {
                return new PriceImpactSummary(PriceImpactClassification.Unknown, segment);
            }

            return new PriceImpactSummary(PriceImpactClassification.Positive, segment);
        }
    }
}
