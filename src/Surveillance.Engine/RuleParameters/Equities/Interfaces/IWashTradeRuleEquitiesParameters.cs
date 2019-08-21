namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    public interface IWashTradeRuleEquitiesParameters : IFilterableRule,
                                                        IRuleParameter,
                                                        IOrganisationalFactorable,
                                                        IWashTradeRuleParameters,
                                                        IReferenceDataFilterable,
                                                        IMarketCapFilterable,
                                                        IVenueVolumeFilterable
    {
        TimeWindows Windows { get; }
    }
}