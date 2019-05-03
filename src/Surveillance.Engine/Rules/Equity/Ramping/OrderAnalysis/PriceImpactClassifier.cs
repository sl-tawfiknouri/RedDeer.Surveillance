using System.Collections.Generic;
using System.Linq;
using Domain.Core.Trading.Orders;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis
{
    public class PriceImpactClassifier : IPriceImpactClassifier
    {
        IReadOnlyCollection<IPriceImpactSummary> IPriceImpactClassifier.ClassifyByTradeCount(IReadOnlyCollection<Order> orders)
        {
            if (!orders.Any())
            {
                return new[]
                {
                    new PriceImpactSummary(PriceImpactClassification.Unknown, TimeSegment.ThirtyDay)
                };
            }

            return new[]
            {
                new PriceImpactSummary(PriceImpactClassification.Positive, TimeSegment.ThirtyDay)
            };
        }

        IReadOnlyCollection<IPriceImpactSummary> IPriceImpactClassifier.ClassifyByWeightedVolume(IReadOnlyCollection<Order> orders)
        {
            if (!orders.Any())
            {
                return new[]
                {
                    new PriceImpactSummary(PriceImpactClassification.Unknown, TimeSegment.ThirtyDay)
                };
            }

            return new []
            {
                new PriceImpactSummary(PriceImpactClassification.Positive, TimeSegment.ThirtyDay)
            };
        }
    }
}
