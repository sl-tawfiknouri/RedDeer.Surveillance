namespace Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces
{
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
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
        /// <param name="judgementService">
        /// The judgement service.
        /// </param>
        /// <param name="dataRequestSubscriber">
        /// The data request subscriber.
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
            IFixedIncomeHighVolumeJudgementService judgementService,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode);
    }
}