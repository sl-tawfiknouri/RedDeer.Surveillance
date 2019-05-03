using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Trading.Orders;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis
{
    public class RampingAnalyser : IRampingAnalyser
    {
        private readonly ITimeSeriesTrendClassifier _trendClassifier;
        private readonly IPriceImpactClassifier _priceImpactClassifier;

        public RampingAnalyser(
            ITimeSeriesTrendClassifier trendClassifier,
            IPriceImpactClassifier priceImpactClassifier)
        {
            _trendClassifier = trendClassifier ?? throw new ArgumentNullException(nameof(trendClassifier));
            _priceImpactClassifier = priceImpactClassifier ?? throw new ArgumentNullException(nameof(priceImpactClassifier));
        }

        /// <summary>
        /// Oldest order at head, youngest (root) at tail
        /// </summary>
        public IRampingStrategySummaryPanel Analyse(IReadOnlyCollection<Order> orderSegment)
        {
            if (!orderSegment.Any())
            {
                return null;
            }

            var orderedOrders = orderSegment.Reverse();

            var head = orderedOrders.First();
            var to = head.FilledDate.Value;

            var fromOneDay = to - TimeSpan.FromDays(1);
            var fromFiveDay = to - TimeSpan.FromDays(5);
            var fromThirtyDay = to - TimeSpan.FromDays(30);

            var dayOne = ClassifyPriceImpactByDate(head, orderSegment, TimeSpan.FromDays(1), TimeSegment.OneDay);
            var segmentPriceTrendOne = _trendClassifier.Classify(head.Instrument, fromOneDay, to);
            var dayOneSummary = IdentifyRampingStrategy(dayOne, segmentPriceTrendOne, TimeSegment.OneDay);

            var dayFive = ClassifyPriceImpactByDate(head, orderSegment, TimeSpan.FromDays(5), TimeSegment.FiveDay);
            var segmentPriceTrendFive = _trendClassifier.Classify(head.Instrument, fromFiveDay, to);
            var dayFiveSummary = IdentifyRampingStrategy(dayFive, segmentPriceTrendFive, TimeSegment.FiveDay);

            var dayThirty = ClassifyPriceImpactByDate(head, orderSegment, TimeSpan.FromDays(30), TimeSegment.ThirtyDay);
            var segmentPriceTrendThirty = _trendClassifier.Classify(head.Instrument, fromThirtyDay, to);
            var dayThirtySummary = IdentifyRampingStrategy(dayThirty, segmentPriceTrendThirty, TimeSegment.ThirtyDay);

            return new RampingStrategySummaryPanel(dayOneSummary, dayFiveSummary, dayThirtySummary);
        }

        public IRampingStrategySummary IdentifyRampingStrategy(
            IPriceImpactSummary priceImpactSummary,
            ITimeSeriesTrendClassification trendClassification,
            TimeSegment timeSegment)
        {
            var strategy = RampingStrategy.Unknown;

            return new RampingStrategySummary(
                    priceImpactSummary,
                    trendClassification,
                    strategy,
                    timeSegment);
        }

        private IPriceImpactSummary ClassifyPriceImpactByDate(
            Order root,
            IReadOnlyCollection<Order> orderSegment,
            TimeSpan span,
            TimeSegment segment)
        {
            var filteredOrders = FilterOrderByDate(root, orderSegment, span);
            var segmentTradeTrend = _priceImpactClassifier.ClassifyByTradeCount(filteredOrders, segment);

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
