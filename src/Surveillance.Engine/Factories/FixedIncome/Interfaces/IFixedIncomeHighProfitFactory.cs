namespace Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces
{
    using Domain.Surveillance.Scheduling;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Judgements.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;

    /// <summary>
    /// The FixedIncomeHighProfitFactory interface.
    /// Implementations have internal constructions of the stream  / market close variants -> aggregated high level fixed income rule
    /// </summary>
    public interface IFixedIncomeHighProfitFactory
    {
        /// <summary>
        /// The build rule.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="ruleContext">
        /// The rule context.
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
        /// <param name="scheduledExecution">
        /// The scheduled execution.
        /// </param>
        /// <returns>
        /// The <see cref="IFixedIncomeHighProfitsRule"/>.
        /// </returns>
        IFixedIncomeHighProfitsRule BuildRule(
            IHighProfitsRuleFixedIncomeParameters parameters,
            ISystemProcessOperationRunRuleContext ruleContext,
            IFixedIncomeHighProfitJudgementService judgementService,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            RuleRunMode runMode,
            ScheduledExecution scheduledExecution);
    }
}