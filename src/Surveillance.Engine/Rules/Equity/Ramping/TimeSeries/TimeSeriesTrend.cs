namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries
{
    using System.ComponentModel;

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