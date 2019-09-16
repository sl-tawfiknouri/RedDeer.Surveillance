namespace Surveillance.Engine.Rules.RuleParameters.Extensions
{
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    /// <summary>
    /// The reference data filterable extensions.
    /// </summary>
    public static class ReferenceDataFilterableExtensions
    {
        /// <summary>
        /// The has reference data filters.
        /// </summary>
        /// <param name="referenceDataFilterable">
        /// The reference data filterable.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasReferenceDataFilters(this IReferenceDataFilterable referenceDataFilterable)
        {
            return referenceDataFilterable.Sectors?.Type != RuleFilterType.None
                   || referenceDataFilterable.Industries?.Type != RuleFilterType.None
                   || referenceDataFilterable.Regions?.Type != RuleFilterType.None
                   || referenceDataFilterable.Countries?.Type != RuleFilterType.None;
        }
    }
}