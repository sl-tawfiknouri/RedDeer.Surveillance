using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Trading.Orders;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Analysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces;
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
                return new RampingStrategySummaryPanel();
            }

            var orderedOrders = orderSegment.Reverse();

            var head = orderedOrders.First();
            var tail = orderedOrders.Last();

            // initiation
            var from = tail.FilledDate.Value;

            // termination
            var to = head.FilledDate.Value;

            var span = to - from;

            // price trend in this segment
            var segmentPriceTrend = _trendClassifier.Classify(head.Instrument, span, from);

            var dayOne = ClassifyPriceImpactByDate(head, orderSegment, TimeSpan.FromDays(1));
            var dayFive = ClassifyPriceImpactByDate(head, orderSegment, TimeSpan.FromDays(5));
            var dayThirty = ClassifyPriceImpactByDate(head, orderSegment, TimeSpan.FromDays(30));
            
            return new RampingStrategySummaryPanel();
        }

        private IReadOnlyCollection<IPriceImpactSummary> ClassifyPriceImpactByDate(
            Order root,
            IReadOnlyCollection<Order> orderSegment,
            TimeSpan span)
        {
            var filteredOrders = FilterOrderByDate(root, orderSegment, span);
            var segmentTradeTrend = _priceImpactClassifier.ClassifyByTradeCount(filteredOrders);

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
