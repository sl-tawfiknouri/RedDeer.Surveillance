using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Markets.Timebars;
using Domain.Core.Trading.Orders;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis
{
    public class RampingAnalyser : IRampingAnalyser
    {
        private readonly ITimeSeriesTrendClassifier _trendClassifier;
        private readonly IOrderPriceImpactClassifier _orderPriceImpactClassifier;

        public RampingAnalyser(
            ITimeSeriesTrendClassifier trendClassifier,
            IOrderPriceImpactClassifier orderPriceImpactClassifier)
        {
            _trendClassifier = trendClassifier ?? throw new ArgumentNullException(nameof(trendClassifier));
            _orderPriceImpactClassifier = orderPriceImpactClassifier ?? throw new ArgumentNullException(nameof(orderPriceImpactClassifier));
        }

        /// <summary>
        /// Oldest order at head, youngest (root) at tail
        /// </summary>
        public IRampingStrategySummaryPanel Analyse(
            IReadOnlyCollection<Order> orderSegment,
            List<EquityInstrumentIntraDayTimeBar> timeBars)
        {
            if (!orderSegment.Any())
            {
                return null;
            }

            var orderedOrders = orderSegment.Reverse();
            var head = orderedOrders.First();

            var summary01 = ClassifyRampingStrategy(head, orderSegment, TimeSpan.FromDays(1), TimeSegment.OneDay, timeBars);
            var summary05 = ClassifyRampingStrategy(head, orderSegment, TimeSpan.FromDays(5), TimeSegment.FiveDay, timeBars);
            var summary15 = ClassifyRampingStrategy(head, orderSegment, TimeSpan.FromDays(15), TimeSegment.FifteenDay, timeBars);
            var summary30 = ClassifyRampingStrategy(head, orderSegment, TimeSpan.FromDays(30), TimeSegment.ThirtyDay, timeBars);

            return new RampingStrategySummaryPanel(summary01, summary05, summary15, summary30);
        }

        private IRampingStrategySummary ClassifyRampingStrategy(
            Order head,
            IReadOnlyCollection<Order> orderSegment,
            TimeSpan span,
            TimeSegment segment,
            List<EquityInstrumentIntraDayTimeBar> timeBars)
        {
            var to = head.FilledDate.Value;
            var fromAdjusted = to - span;

            var weightedPriceImpact = ClassifyOrderPriceImpactByWeightedVolume(head, orderSegment, span, segment);
            var segmentPriceTrend = _trendClassifier.Classify(timeBars, head.Instrument, fromAdjusted, to, segment);
            var timeSegmentSummary = IdentifyRampingStrategy(weightedPriceImpact, segmentPriceTrend, segment);

            return timeSegmentSummary;
        }

        public IRampingStrategySummary IdentifyRampingStrategy(
            IPriceImpactSummary priceImpactSummary,
            ITimeSeriesTrendClassification trendClassification,
            TimeSegment timeSegment)
        {
            if (priceImpactSummary.Classification == PriceImpactClassification.Unknown)
            {
                return new RampingStrategySummary(
                    priceImpactSummary,
                    trendClassification,
                    RampingStrategy.Unknown,
                    timeSegment);
            }

            if (priceImpactSummary.Classification == PriceImpactClassification.Positive)
            {
                var rampingStrategy =
                    trendClassification.Trend == TimeSeriesTrend.Increasing
                        ? RampingStrategy.Reinforcing
                        : RampingStrategy.Unknown;

                return new RampingStrategySummary(
                    priceImpactSummary,
                    trendClassification,
                    rampingStrategy,
                    timeSegment);
            }

            if (priceImpactSummary.Classification == PriceImpactClassification.Negative)
            {
                var rampingStrategy =
                    trendClassification.Trend == TimeSeriesTrend.Decreasing
                        ? RampingStrategy.Reinforcing
                        : RampingStrategy.Unknown;

                return new RampingStrategySummary(
                    priceImpactSummary,
                    trendClassification,
                    rampingStrategy,
                    timeSegment);
            }

            return new RampingStrategySummary(
                priceImpactSummary,
                trendClassification,
                RampingStrategy.Unknown,
                timeSegment);
        }

        private IPriceImpactSummary ClassifyOrderPriceImpactByWeightedVolume(
            Order root,
            IReadOnlyCollection<Order> orderSegment,
            TimeSpan span,
            TimeSegment segment)
        {
            var filteredOrders = FilterOrderByDate(root, orderSegment, span);
            var segmentTradeTrend = _orderPriceImpactClassifier.ClassifyByWeightedVolume(filteredOrders, segment);

            return segmentTradeTrend;
        }

        private IReadOnlyCollection<Order> FilterOrderByDate(
            Order root, 
            IReadOnlyCollection<Order> order, 
            TimeSpan date)
        {
            if (root.FilledDate == null
                || order == null
                || !order.Any())
            {
                return new Order[0];
            }

            var oldestDate = root.FilledDate.Value - date;

            var filteredOrders =
                order
                    .Where(_ => _.FilledDate >= oldestDate)
                    .ToList();

            return filteredOrders;
        }
    }
}
