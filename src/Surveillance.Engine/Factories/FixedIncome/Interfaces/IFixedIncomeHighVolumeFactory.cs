namespace Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces
{
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance.Interfaces;

    /// <summary>
    /// The FixedIncomeHighVolumeFactory interface.
    /// </summary>
    public interface IFixedIncomeHighVolumeFactory
    {
        /// <summary>
        /// The build rule.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="operationContext">
        /// The operation context.
        /// </param>
        /// <param name="runMode">
        /// The run mode.
        /// </param>
        /// <returns>
        /// The <see cref="IFixedIncomeHighVolumeRule"/>.
        /// </returns>
        IFixedIncomeHighVolumeRule BuildRule(
            IHighVolumeIssuanceRuleFixedIncomeParameters parameters,
            ISystemProcessOperationRunRuleContext operationContext,
            RuleRunMode runMode);
    }
}