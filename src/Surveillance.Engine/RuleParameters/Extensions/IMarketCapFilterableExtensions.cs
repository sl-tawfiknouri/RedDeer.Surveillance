namespace Surveillance.Engine.Rules.RuleParameters.Extensions
{
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    public static class IMarketCapFilterableExtensions
    {
        public static bool HasMarketCapFilters(this IMarketCapFilterable marketCapFilterable)
        {
            return marketCapFilterable.MarketCapFilter.Type == RuleFilterType.Include;
        }
    }
}