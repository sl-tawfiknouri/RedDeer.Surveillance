using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade.Interfaces;

namespace Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces
{
    public interface IFixedIncomeWashTradeFactory
    {
        IFixedIncomeWashTradeRule BuildRule(
            IWashTradeRuleFixedIncomeParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx,
            IUniverseAlertStream alertStream,
            RuleRunMode runMode);
    }
}