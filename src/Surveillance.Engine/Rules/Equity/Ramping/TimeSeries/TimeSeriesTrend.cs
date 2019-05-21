using System.ComponentModel;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries
{
    public enum TimeSeriesTrend
    {
        [Description("Unclassified")]
        Unclassified = 0,
        [Description("Increasing")]
        Increasing,
        [Description("Decreasing")]
        Decreasing,
        [Description("Mean Reverting")]
        MeanReverting,
        [Description("Chaotic")]
        Chaotic
    }
}
