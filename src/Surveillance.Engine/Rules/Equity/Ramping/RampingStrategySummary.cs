using System;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    public class RampingStrategySummary : IRampingStrategySummary
    {
        public RampingStrategySummary(
            IPriceImpactSummary priceImpact,
            ITimeSeriesTrendClassification trendClassification,
            RampingStrategy strategy,
            TimeSegment timeSegment)
        {
            PriceImpact = priceImpact ?? throw new ArgumentNullException(nameof(priceImpact));
            TrendClassification = trendClassification ?? throw new ArgumentNullException(nameof(trendClassification));
            Strategy = strategy;
            TimeSegment = timeSegment;
        }

        public TimeSegment TimeSegment { get; }
        public IPriceImpactSummary PriceImpact { get; }
        public ITimeSeriesTrendClassification TrendClassification { get; }
        public RampingStrategy Strategy { get; }
    }
}
