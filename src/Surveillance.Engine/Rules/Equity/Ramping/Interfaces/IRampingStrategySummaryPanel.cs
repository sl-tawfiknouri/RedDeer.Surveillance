namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces
{
    public interface IRampingStrategySummaryPanel
    {
        IRampingStrategySummary FiveDayAnalysis();
        IRampingStrategySummary OneDayAnalysis();
        IRampingStrategySummary ThirtyDayAnalysis();
    }
}