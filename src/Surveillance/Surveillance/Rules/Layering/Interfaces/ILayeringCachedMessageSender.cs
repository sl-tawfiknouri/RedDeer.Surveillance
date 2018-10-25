using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.Layering.Interfaces
{
    public interface ILayeringCachedMessageSender
    {
        int Flush(ISystemProcessOperationRunRuleContext ruleCtx);
        void Send(ILayeringRuleBreach ruleBreach);
    }
}