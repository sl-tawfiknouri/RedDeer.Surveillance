using Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces
{
    public interface IRampingStrategySummary
    {
        TimeSegment TimeSegment { get; }
        IPriceImpactSummary PriceImpact { get; }
        RampingStrategy Strategy { get; }
        ITimeSeriesTrendClassification TrendClassification { get; }
    }
}