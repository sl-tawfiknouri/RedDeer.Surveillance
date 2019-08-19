namespace Surveillance.Engine.Rules.RuleParameters.Extensions
{
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    public static class IReferenceDataFilterableExtensions
    {
        public static bool HasReferenceDataFilters(this IReferenceDataFilterable referenceDataFilterable)
        {
            return referenceDataFilterable.Sectors?.Type != RuleFilterType.None
                   || referenceDataFilterable.Industries?.Type != RuleFilterType.None
                   || referenceDataFilterable.Regions?.Type != RuleFilterType.None
                   || referenceDataFilterable.Countries?.Type != RuleFilterType.None;
        }
    }
}