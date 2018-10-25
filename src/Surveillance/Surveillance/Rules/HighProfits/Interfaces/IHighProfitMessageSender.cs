using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.HighProfits.Interfaces
{
    public interface IHighProfitMessageSender
    {
        void Send(IHighProfitRuleBreach ruleBreach, ISystemProcessOperationRunRuleContext ruleCtx);
    }
}