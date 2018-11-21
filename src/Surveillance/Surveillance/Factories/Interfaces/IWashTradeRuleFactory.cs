using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface IWashTradeRuleFactory
    {
        IWashTradeRule Build(IWashTradeRuleParameters parameters, ISystemProcessOperationRunRuleContext ruleCtx);
    }
}