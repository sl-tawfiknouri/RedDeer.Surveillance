namespace Surveillance.Engine.Rules.RuleParameters.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Filter;

    /// <summary>
    ///     Filter on (%) trading as a proportion of venue trading
    /// </summary>
    public interface IVenueVolumeFilterable
    {
        DecimalRangeRuleFilter VenueVolumeFilter { get; }

        bool HasVenueVolumeFilters();
    }
}