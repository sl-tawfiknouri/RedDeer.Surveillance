using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.Layering.Interfaces
{
    public interface ILayeringAlertSender
    {
        void Send(ILayeringRuleBreach breach, ISystemProcessOperationRunRuleContext opCtx);
    }
}