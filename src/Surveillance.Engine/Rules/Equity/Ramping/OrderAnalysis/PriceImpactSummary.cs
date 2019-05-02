using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis
{
    public class PriceImpactSummary : IPriceImpactSummary
    {
        public PriceImpactSummary(
            PriceImpactClassification classification,
            TimeSegmentLength timeSegment)
        {
            Classification = classification;
            TimeSegment = timeSegment;
        }

        public PriceImpactClassification Classification { get; }
        public TimeSegmentLength TimeSegment { get; }
    }
}
