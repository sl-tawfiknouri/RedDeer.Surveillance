namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries
{
    using System.ComponentModel;

    public enum TimeSegment
    {
        [Description("One day")]
        OneDay,

        [Description("Five day")]
        FiveDay,

        [Description("Fifteen day")]
        FifteenDay,

        [Description("Thirty day")]
        ThirtyDay
    }
}