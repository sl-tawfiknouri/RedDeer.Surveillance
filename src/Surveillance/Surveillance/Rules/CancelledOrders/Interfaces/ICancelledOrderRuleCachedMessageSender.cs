
// ReSharper disable UnusedMember.Global

using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules.CancelledOrders.Interfaces
{
    public interface ICancelledOrderRuleCachedMessageSender
    {
        int Flush(ISystemProcessOperationRunRuleContext opCtx);
        void Send(ICancelledOrderRuleBreach ruleBreach);
    }
}