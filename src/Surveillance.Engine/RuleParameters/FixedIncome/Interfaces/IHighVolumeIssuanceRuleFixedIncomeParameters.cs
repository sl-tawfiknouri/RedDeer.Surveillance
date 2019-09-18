namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    /// <summary>
    /// The HighVolumeIssuanceRuleFixedIncomeParameters interface.
    /// </summary>
    public interface IHighVolumeIssuanceRuleFixedIncomeParameters : IFilterableRule,
                                                                    IRuleParameter,
                                                                    IOrganisationalFactorable
    {
        /// <summary>
        /// Gets the windows.
        /// </summary>
        TimeWindows Windows { get; }

        /// <summary>
        /// Gets the fixed income high volume percentage daily.
        /// </summary>
        decimal? FixedIncomeHighVolumePercentageDaily { get; }

        /// <summary>
        /// Gets the fixed income high volume percentage window.
        /// </summary>
        decimal? FixedIncomeHighVolumePercentageWindow { get; }
    }
}