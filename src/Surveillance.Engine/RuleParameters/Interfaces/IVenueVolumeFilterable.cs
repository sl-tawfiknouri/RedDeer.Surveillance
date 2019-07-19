using Surveillance.Engine.Rules.RuleParameters.Filter;

namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    /// <summary>
    /// Filter on (%) trading as a proportion of venue trading
    /// </summary>
    public interface IVenueVolumeFilterable
    {
        DecimalRangeRuleFilter VenueVolumeFilter { get; }

        bool HasVenueVolumeFilters();
    }
}
