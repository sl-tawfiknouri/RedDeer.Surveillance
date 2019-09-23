namespace Surveillance.Engine.Rules.RuleParameters.Filter
{
    using System;

    [Serializable]
    public class DecimalRangeRuleFilter
    {
        public decimal? Max { get; set; }

        public decimal? Min { get; set; }

        public RuleFilterType Type { get; set; }

        public static DecimalRangeRuleFilter None()
        {
            return new DecimalRangeRuleFilter { Type = RuleFilterType.None };
        }
    }
}