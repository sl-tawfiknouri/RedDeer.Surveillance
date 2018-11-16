using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface IWashTradeRuleFactory
    {
        IWashTradeRule Build(ISystemProcessOperationRunRuleContext ruleCtx);
        string RuleVersion { get; }
    }
}