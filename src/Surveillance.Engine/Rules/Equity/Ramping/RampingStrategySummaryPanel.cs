using System;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    public class RampingStrategySummaryPanel : IRampingStrategySummaryPanel
    {
        public RampingStrategySummaryPanel(
            IRampingStrategySummary oneDayAnalysis,
            IRampingStrategySummary fiveDayAnalysis,
            IRampingStrategySummary thirtyDayAnalysis)
        {
            OneDayAnalysis = oneDayAnalysis ?? throw new ArgumentNullException(nameof(oneDayAnalysis));
            FiveDayAnalysis = fiveDayAnalysis ?? throw new ArgumentNullException(nameof(fiveDayAnalysis));
            ThirtyDayAnalysis = thirtyDayAnalysis ?? throw new ArgumentNullException(nameof(thirtyDayAnalysis));
        }

        public IRampingStrategySummary OneDayAnalysis { get; }
        public IRampingStrategySummary FiveDayAnalysis { get; }
        public IRampingStrategySummary ThirtyDayAnalysis { get; }
    }
}
