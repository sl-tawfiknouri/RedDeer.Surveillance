using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Extensions
{
    public static class IMarketCapFilterableExtensions
    {
        public static bool HasMarketCapFilters(this IMarketCapFilterable marketCapFilterable)
        {
            return marketCapFilterable.MarketCapFilter.Type == Filter.RuleFilterType.Include;
        }
    }
}
