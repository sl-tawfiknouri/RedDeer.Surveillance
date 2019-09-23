namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces
{
    public interface IRampingStrategySummaryPanel
    {
        IRampingStrategySummary FifteenDayAnalysis { get; }

        IRampingStrategySummary FiveDayAnalysis { get; }

        IRampingStrategySummary OneDayAnalysis { get; }

        IRampingStrategySummary ThirtyDayAnalysis { get; }

        bool HasRampingStrategy();
    }
}