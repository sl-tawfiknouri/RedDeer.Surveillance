namespace Surveillance.Engine.Rules.RuleParameters.Extensions
{
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    public static class IHighVolumeFilterableExtensions
    {
        public static bool HasVenueVolumeFilters(this IVenueVolumeFilterable filterableRule)
        {
            return filterableRule.VenueVolumeFilter?.Type != RuleFilterType.None;
        }
    }
}