using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.CancelledOrders.Interfaces
{
    public interface ICancelledOrderMessageSender
    {
        void Send(ICancelledOrderRuleBreach ruleBreach, ISystemProcessOperationRunRuleContext opCtx);
    }
}