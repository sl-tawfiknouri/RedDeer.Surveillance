namespace Surveillance.Engine.Rules.RuleParameters.Filter
{
    using System;

    /// <summary>
    /// The decimal range rule filter.
    /// </summary>
    [Serializable]
    public class DecimalRangeRuleFilter
    {
        /// <summary>
        /// Gets or sets the max.
        /// </summary>
        public decimal? Max { get; set; }

        /// <summary>
        /// Gets or sets the min.
        /// </summary>
        public decimal? Min { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public RuleFilterType Type { get; set; }

        /// <summary>
        /// The none decimal range rule filter.
        /// </summary>
        /// <returns>
        /// The <see cref="DecimalRangeRuleFilter"/>.
        /// </returns>
        public static DecimalRangeRuleFilter None()
        {
            return new DecimalRangeRuleFilter { Type = RuleFilterType.None };
        }
    }
}