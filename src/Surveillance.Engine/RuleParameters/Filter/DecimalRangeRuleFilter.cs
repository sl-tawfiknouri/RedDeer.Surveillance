namespace Surveillance.Engine.Rules.RuleParameters.Filter
{
    public class DecimalRangeRuleFilter
    {
        public RuleFilterType Type { get; set; }

        public decimal? Min { get; set; }
        public decimal? Max { get; set; }

        public static DecimalRangeRuleFilter None()
        {
            return new DecimalRangeRuleFilter { Type = RuleFilterType.None };
        }
    }
}
