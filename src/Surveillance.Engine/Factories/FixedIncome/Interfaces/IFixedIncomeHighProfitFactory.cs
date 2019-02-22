using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;

namespace Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces
{
    public interface IFixedIncomeHighProfitFactory
    {
        IFixedIncomeHighProfitsRule BuildRule(
            IHighProfitsRuleFixedIncomeParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            RuleRunMode runMode);
    }
}