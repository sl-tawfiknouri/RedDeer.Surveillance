namespace Surveillance.Engine.Rules.Rules.Equity.Ramping.OrderAnalysis
{
    using System.ComponentModel;

    public enum PriceImpactClassification
    {
        [Description("Unclassified")]
        Unknown,

        [Description("Positive")]
        Positive,

        [Description("Negative")]
        Negative
    }
}