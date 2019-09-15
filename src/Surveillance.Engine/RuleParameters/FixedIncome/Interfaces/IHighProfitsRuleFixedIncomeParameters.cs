namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    /// <summary>
    /// The HighProfitsRuleFixedIncomeParameters interface.
    /// </summary>
    public interface IHighProfitsRuleFixedIncomeParameters : IFilterableRule,
                                                             IRuleParameter,
                                                             IOrganisationalFactorable,
                                                             IReferenceDataFilterable,
                                                             IMarketCapFilterable,
                                                             IVenueVolumeFilterable
    {
        /// <summary>
        /// Gets the high profit absolute threshold.
        /// </summary>
        decimal? HighProfitAbsoluteThreshold { get; }

        /// <summary>
        /// Gets the high profit currency conversion target currency.
        /// </summary>
        string HighProfitCurrencyConversionTargetCurrency { get; }

        /// <summary>
        /// Gets the high profit percentage threshold.
        /// </summary>
        decimal? HighProfitPercentageThreshold { get; }

        /// <summary>
        /// Gets a value indicating whether perform high profit daily analysis.
        /// </summary>
        bool PerformHighProfitDailyAnalysis { get; }

        /// <summary>
        /// Gets a value indicating whether perform high profit window analysis.
        /// </summary>
        bool PerformHighProfitWindowAnalysis { get; }

        /// <summary>
        /// Gets a value indicating whether use currency conversions.
        /// </summary>
        bool UseCurrencyConversions { get; }

        /// <summary>
        /// Gets the windows.
        /// </summary>
        TimeWindows Windows { get; }
    }
}