namespace Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces
{
    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolumeIssuance.Interfaces;

    public interface IFixedIncomeHighVolumeFactory
    {
        IFixedIncomeHighVolumeRule BuildRule(
            IHighVolumeIssuanceRuleFixedIncomeParameters parameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode);
    }
}