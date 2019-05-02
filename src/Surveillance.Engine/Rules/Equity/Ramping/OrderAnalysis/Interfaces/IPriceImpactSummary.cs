using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces
{
    public interface IPriceImpactSummary
    {
        PriceImpactClassification Classification { get; }
        TimeSegmentLength TimeSegment { get; }
    }
}