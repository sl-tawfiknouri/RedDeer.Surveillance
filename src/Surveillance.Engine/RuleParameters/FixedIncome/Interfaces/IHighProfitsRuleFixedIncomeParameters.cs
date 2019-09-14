namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    public interface IHighProfitsRuleFixedIncomeParameters : IFilterableRule,
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