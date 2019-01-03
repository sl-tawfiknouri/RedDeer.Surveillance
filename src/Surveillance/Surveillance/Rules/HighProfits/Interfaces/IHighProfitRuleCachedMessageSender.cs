using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Rules.HighProfits.Interfaces
{
    public interface IHighProfitRuleCachedMessageSender
    {
        int Flush(ISystemProcessOperationRunRuleContext ruleCtx);
        void Send(IHighProfitRuleBreach ruleBreach);
        void Remove(ITradePosition position);
        void Delete();
    }
}