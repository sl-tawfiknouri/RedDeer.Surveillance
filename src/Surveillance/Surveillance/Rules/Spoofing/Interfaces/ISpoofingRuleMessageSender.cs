using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.Spoofing.Interfaces
{
    public interface ISpoofingRuleMessageSender
    {
        void Send(ISpoofingRuleBreach ruleBreach, ISystemProcessOperationRunRuleContext opCtx);
    }
}