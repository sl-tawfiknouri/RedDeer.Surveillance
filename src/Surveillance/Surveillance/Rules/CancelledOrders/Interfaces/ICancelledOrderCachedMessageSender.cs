using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.CancelledOrders.Interfaces
{
    public interface ICancelledOrderCachedMessageSender
    {
        void Send(ICancelledOrderRuleBreach ruleBreach, ISystemProcessOperationRunRuleContext opCtx);
    }
}