using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.WashTrade.Interfaces
{
    public interface IWashTradeRuleMessageSender
    {
        void Send(IWashTradeRuleBreach breach, ISystemProcessOperationRunRuleContext opCtx);
    }
}
