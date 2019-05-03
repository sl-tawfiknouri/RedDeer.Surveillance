namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces
{
    public interface IRampingStrategySummaryPanel
    {
        IRampingStrategySummary OneDayAnalysis { get; }
        IRampingStrategySummary FiveDayAnalysis { get; }
        IRampingStrategySummary FifteenDayAnalysis { get; }
        IRampingStrategySummary ThirtyDayAnalysis { get; }
        bool HasRampingStrategy();
    }
}