namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces
{
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;

    public interface IPriceImpactSummary
    {
        PriceImpactClassification Classification { get; }

        TimeSegment TimeSegment { get; }
    }
}