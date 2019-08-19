namespace Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    public interface IHighProfitsRuleEquitiesParameters : IFilterableRule,
                                                          IRuleParameter,
                                                          IOrganisationalFactorable,
                                                          IReferenceDataFilterable,
                                                          IMarketCapFilterable,
                                                          IVenueVolumeFilterable
    {
        decimal? HighProfitAbsoluteThreshold { get; }

        string HighProfitCurrencyConversionTargetCurrency { get; }

        decimal? HighProfitPercentageThreshold { get; }

        bool PerformHighProfitDailyAnalysis { get; }

        bool PerformHighProfitWindowAnalysis { get; }

        bool UseCurrencyConversions { get; }

        TimeWindows Windows { get; }
    }
}