namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    public interface IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters : IFilterableRule,
                                                                                IRuleParameter,
                                                                                IOrganisationalFactorable,
                                                                                IReferenceDataFilterable,
                                                                                IMarketCapFilterable,
                                                                                IVenueVolumeFilterable
    {
        decimal Sigma { get; }

        TimeWindows Windows { get; }
    }
}