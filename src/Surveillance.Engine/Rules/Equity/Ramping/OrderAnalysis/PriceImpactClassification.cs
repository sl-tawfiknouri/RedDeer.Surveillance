using System.ComponentModel;

namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis
{
    public enum PriceImpactClassification
    {
        [Description("Unclassified")]
        Unknown,
        [Description("Positive")]
        Positive,
        [Description("Negative")]
        Negative,
    }
}
