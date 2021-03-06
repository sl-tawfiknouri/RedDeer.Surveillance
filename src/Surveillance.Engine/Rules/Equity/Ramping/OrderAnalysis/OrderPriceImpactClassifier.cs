﻿namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis
{
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Core.Trading.Orders;

    using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;

    public class OrderPriceImpactClassifier : IOrderPriceImpactClassifier
    {
        public IPriceImpactSummary ClassifyByTradeCount(IReadOnlyCollection<Order> orders, TimeSegment segment)
        {
            if (!orders.Any()) return new PriceImpactSummary(PriceImpactClassification.Unknown, segment);

            var buySide = orders.Count(
                _ => _.OrderDirection == OrderDirections.BUY || _.OrderDirection == OrderDirections.COVER);

            if (buySide == 0) return new PriceImpactSummary(PriceImpactClassification.Negative, segment);

            var proportionBuySide = buySide / (decimal)orders.Count;

            if (proportionBuySide >= 0.66m) return new PriceImpactSummary(PriceImpactClassification.Positive, segment);
            if (proportionBuySide <= 0.33m) return new PriceImpactSummary(PriceImpactClassification.Negative, segment);
            return new PriceImpactSummary(PriceImpactClassification.Unknown, segment);
        }

        public IPriceImpactSummary ClassifyByWeightedVolume(IReadOnlyCollection<Order> orders, TimeSegment segment)
        {
            if (!orders.Any()) return new PriceImpactSummary(PriceImpactClassification.Unknown, segment);

            var buySide = orders
                .Where(_ => _.OrderDirection == OrderDirections.BUY || _.OrderDirection == OrderDirections.COVER)
                .Sum(_ => _.OrderFilledVolume ?? 0);

            var sellSide = orders
                .Where(_ => _.OrderDirection == OrderDirections.SELL || _.OrderDirection == OrderDirections.SHORT)
                .Sum(_ => _.OrderFilledVolume ?? 0);

            if (buySide == 0)
            {
                var impact = sellSide == 0 ? PriceImpactClassification.Unknown : PriceImpactClassification.Negative;

                return new PriceImpactSummary(impact, segment);
            }

            var totalVolume = orders.Sum(_ => _.OrderFilledVolume ?? 0);

            if (totalVolume == 0) return new PriceImpactSummary(PriceImpactClassification.Unknown, segment);

            var proportionBuySide = buySide / totalVolume;

            if (proportionBuySide >= 0.66m) return new PriceImpactSummary(PriceImpactClassification.Positive, segment);
            if (proportionBuySide <= 0.33m) return new PriceImpactSummary(PriceImpactClassification.Negative, segment);
            return new PriceImpactSummary(PriceImpactClassification.Unknown, segment);
        }
    }
}