using System;
using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    public class RampingStrategySummaryPanel : IRampingStrategySummaryPanel
    {
        public RampingStrategySummaryPanel(
            IRampingStrategySummary oneDayAnalysis,
            IRampingStrategySummary fiveDayAnalysis,
            IRampingStrategySummary fifteenDayAnalysis,
            IRampingStrategySummary thirtyDayAnalysis)
        {
            OneDayAnalysis = oneDayAnalysis ?? throw new ArgumentNullException(nameof(oneDayAnalysis));
            FiveDayAnalysis = fiveDayAnalysis ?? throw new ArgumentNullException(nameof(fiveDayAnalysis));
            FifteenDayAnalysis = fifteenDayAnalysis ?? throw new ArgumentNullException(nameof(fifteenDayAnalysis));
            ThirtyDayAnalysis = thirtyDayAnalysis ?? throw new ArgumentNullException(nameof(thirtyDayAnalysis));
        }

        public IRampingStrategySummary OneDayAnalysis { get; }
        public IRampingStrategySummary FiveDayAnalysis { get; }
        public IRampingStrategySummary FifteenDayAnalysis { get; }
        public IRampingStrategySummary ThirtyDayAnalysis { get; }

        public bool HasRampingStrategy()
        {
            return OneDayAnalysis?.Strategy == RampingStrategy.Reinforcing
                   || FiveDayAnalysis?.Strategy == RampingStrategy.Reinforcing
                   || FifteenDayAnalysis?.Strategy == RampingStrategy.Reinforcing
                   || ThirtyDayAnalysis?.Strategy == RampingStrategy.Reinforcing;
        }
    }
}
