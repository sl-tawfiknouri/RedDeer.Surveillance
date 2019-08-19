namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    public interface ISpoofingRuleEquitiesParameters : IFilterableRule,
                                                       IRuleParameter,
                                                       IOrganisationalFactorable,
                                                       IReferenceDataFilterable,
                                                       IMarketCapFilterable,
                                                       IVenueVolumeFilterable
    {
        decimal CancellationThreshold { get; }

        decimal RelativeSizeMultipleForSpoofExceedingReal { get; }

        TimeWindows Windows { get; }
    }
}