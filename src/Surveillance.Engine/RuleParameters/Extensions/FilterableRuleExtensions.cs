namespace Surveillance.Engine.Rules.RuleParameters.Extensions
{
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    /// <summary>
    /// The i filterable rule extensions.
    /// </summary>
    public static class FilterableRuleExtensions
    {
        /// <summary>
        /// The has internal filters.
        /// </summary>
        /// <param name="filterableRule">
        /// The filterable rule.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasInternalFilters(this IFilterableRule filterableRule)
        {
            return filterableRule.Accounts?.Type != RuleFilterType.None
                   || filterableRule.Traders?.Type != RuleFilterType.None
                   || filterableRule.Markets?.Type != RuleFilterType.None
                   || filterableRule.Funds?.Type != RuleFilterType.None
                   || filterableRule.Strategies?.Type != RuleFilterType.None;
        }
    }
}