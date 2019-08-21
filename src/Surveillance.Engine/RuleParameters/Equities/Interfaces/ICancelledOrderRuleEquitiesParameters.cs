namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    public interface ICancelledOrderRuleEquitiesParameters : IFilterableRule,
                                                             IRuleParameter,
                                                             IOrganisationalFactorable,
                                                             IReferenceDataFilterable,
                                                             IMarketCapFilterable,
                                                             IVenueVolumeFilterable
    {
        decimal? CancelledOrderCountPercentageThreshold { get; set; }

        decimal? CancelledOrderPercentagePositionThreshold { get; set; }

        int? MaximumNumberOfTradesToApplyRuleTo { get; set; }

        int MinimumNumberOfTradesToApplyRuleTo { get; set; }

        TimeWindows Windows { get; set; }
    }
}