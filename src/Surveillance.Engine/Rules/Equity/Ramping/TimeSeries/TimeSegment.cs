using System.ComponentModel;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.TimeSeries
{
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
