namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    public interface ILayeringRuleEquitiesParameters : IFilterableRule,
                                                       IRuleParameter,
                                                       IOrganisationalFactorable,
                                                       IReferenceDataFilterable,
                                                       IMarketCapFilterable,
                                                       IVenueVolumeFilterable
    {
        bool? CheckForCorrespondingPriceMovement { get; }

        decimal? PercentageOfMarketDailyVolume { get; }

        decimal? PercentageOfMarketWindowVolume { get; }

        TimeWindows Windows { get; }
    }
}