namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    using System;

    using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces;

    public class RampingStrategySummary : IRampingStrategySummary
    {
        public RampingStrategySummary(
            IPriceImpactSummary priceImpact,
            ITimeSeriesTrendClassification trendClassification,
            RampingStrategy rampingStrategy,
            TimeSegment timeSegment)
        {
            this.PriceImpact = priceImpact ?? throw new ArgumentNullException(nameof(priceImpact));
            this.TrendClassification =
                trendClassification ?? throw new ArgumentNullException(nameof(trendClassification));
            this.RampingStrategy = rampingStrategy;
            this.TimeSegment = timeSegment;
        }

        public IPriceImpactSummary PriceImpact { get; }

        public RampingStrategy RampingStrategy { get; }

        public TimeSegment TimeSegment { get; }

        public ITimeSeriesTrendClassification TrendClassification { get; }
    }
}