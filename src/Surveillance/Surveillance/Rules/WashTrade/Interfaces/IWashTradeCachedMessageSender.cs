using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.WashTrade.Interfaces
{
    public interface IWashTradeCachedMessageSender
    {
        int Flush(ISystemProcessOperationRunRuleContext ruleCtx);
        void Send(IWashTradeRuleBreach ruleBreach);
    }
}