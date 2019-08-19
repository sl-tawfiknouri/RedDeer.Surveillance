namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis
{
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;

    public class PriceImpactSummary : IPriceImpactSummary
    {
        public PriceImpactSummary(PriceImpactClassification classification, TimeSegment timeSegment)
        {
            this.Classification = classification;
            this.TimeSegment = timeSegment;
        }

        public PriceImpactClassification Classification { get; }

        public TimeSegment TimeSegment { get; }
    }
}