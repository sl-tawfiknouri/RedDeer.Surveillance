namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces
{
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces;

    public interface IRampingStrategySummary
    {
        IPriceImpactSummary PriceImpact { get; }

        RampingStrategy RampingStrategy { get; }

        TimeSegment TimeSegment { get; }

        ITimeSeriesTrendClassification TrendClassification { get; }
    }
}