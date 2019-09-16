namespace Surveillance.Engine.Rules.RuleParameters.Extensions
{
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    /// <summary>
    /// The high volume filterable extensions.
    /// </summary>
    public static class HighVolumeFilterableExtensions
    {
        /// <summary>
        /// The has venue volume filters.
        /// </summary>
        /// <param name="filterableRule">
        /// The filterable rule.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasVenueVolumeFilters(this IVenueVolumeFilterable filterableRule)
        {
            return filterableRule.VenueVolumeFilter?.Type != RuleFilterType.None;
        }
    }
}