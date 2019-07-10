using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Extensions
{
    public static class IFilterableRuleExtensions
    {
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
