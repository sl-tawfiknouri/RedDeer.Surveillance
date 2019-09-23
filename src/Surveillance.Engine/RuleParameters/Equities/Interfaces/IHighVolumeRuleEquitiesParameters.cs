namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    public interface IHighVolumeRuleEquitiesParameters : IFilterableRule,
                                                         IRuleParameter,
                                                         IOrganisationalFactorable,
                                                         IReferenceDataFilterable,
                                                         IMarketCapFilterable,
                                                         IVenueVolumeFilterable
    {
        decimal? HighVolumePercentageDaily { get; }

        decimal? HighVolumePercentageMarketCap { get; }

        decimal? HighVolumePercentageWindow { get; }

        TimeWindows Windows { get; }
    }
}