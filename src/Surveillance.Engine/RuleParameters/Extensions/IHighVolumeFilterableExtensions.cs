using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;

namespace Surveillance.Engine.Rules.RuleParameters.Extensions
{
    public static class IHighVolumeFilterableExtensions
    {
        public static bool HasVenueVolumeFilters(this IVenueVolumeFilterable filterableRule)
        {
            return filterableRule.VenueVolumeFilter?.Type != RuleFilterType.None;
        }
    }
}
