namespace Surveillance.Engine.Rules.Rules.Equity.Ramping
{
    using System;

    using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;

    public class RampingStrategySummaryPanel : IRampingStrategySummaryPanel
    {
        public RampingStrategySummaryPanel(
            IRampingStrategySummary oneDayAnalysis,
            IRampingStrategySummary fiveDayAnalysis,
            IRampingStrategySummary fifteenDayAnalysis,
            IRampingStrategySummary thirtyDayAnalysis)
        {
            this.OneDayAnalysis = oneDayAnalysis ?? throw new ArgumentNullException(nameof(oneDayAnalysis));
            this.FiveDayAnalysis = fiveDayAnalysis ?? throw new ArgumentNullException(nameof(fiveDayAnalysis));
            this.FifteenDayAnalysis = fifteenDayAnalysis ?? throw new ArgumentNullException(nameof(fifteenDayAnalysis));
            this.ThirtyDayAnalysis = thirtyDayAnalysis ?? throw new ArgumentNullException(nameof(thirtyDayAnalysis));
        }

        public IRampingStrategySummary FifteenDayAnalysis { get; }

        public IRampingStrategySummary FiveDayAnalysis { get; }

        public IRampingStrategySummary OneDayAnalysis { get; }

        public IRampingStrategySummary ThirtyDayAnalysis { get; }

        public bool HasRampingStrategy()
        {
            return this.OneDayAnalysis?.RampingStrategy == RampingStrategy.Reinforcing
                   || this.FiveDayAnalysis?.RampingStrategy == RampingStrategy.Reinforcing
                   || this.FifteenDayAnalysis?.RampingStrategy == RampingStrategy.Reinforcing
                   || this.ThirtyDayAnalysis?.RampingStrategy == RampingStrategy.Reinforcing;
        }
    }
}