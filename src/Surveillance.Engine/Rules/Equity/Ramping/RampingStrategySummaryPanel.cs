using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    public class RampingStrategySummaryPanel : IRampingStrategySummaryPanel
    {
        public IRampingStrategySummary OneDayAnalysis()
        {
            return (IRampingStrategySummary)new object();
        }

        public IRampingStrategySummary FiveDayAnalysis()
        {
            return (IRampingStrategySummary)new object();
        }

        public IRampingStrategySummary ThirtyDayAnalysis()
        {
            return (IRampingStrategySummary)new object();
        }
    }
}
