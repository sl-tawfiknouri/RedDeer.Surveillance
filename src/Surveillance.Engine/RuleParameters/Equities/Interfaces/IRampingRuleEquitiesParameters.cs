namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    public interface IRampingRuleEquitiesParameters : IFilterableRule,
                                                      IRuleParameter,
                                                      IOrganisationalFactorable,
                                                      IReferenceDataFilterable,
                                                      IMarketCapFilterable,
                                                      IVenueVolumeFilterable
    {
        decimal AutoCorrelationCoefficient { get; }

        // optional noise reduction
        int? ThresholdOrdersExecutedInWindow { get; }

        decimal? ThresholdVolumePercentageWindow { get; }

        TimeWindows Windows { get; }
    }
}